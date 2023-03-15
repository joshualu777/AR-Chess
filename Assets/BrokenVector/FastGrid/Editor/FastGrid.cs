using UnityEngine;
using UnityEditor;
using BrokenVector.FastGrid.Utils;

namespace BrokenVector.FastGrid
{
    public class FastGrid : EditorWindow
    {
        private static AssetPrefs prefs = new AssetPrefs(Constants.ASSET_NAME);
        private static Texture2D logo;

        public static bool SnapEnabled = false;
        public static bool DrawEnabled = true;

        private static float gridSize = Constants.DEFAULT_GRID_SIZE;
        private static float rotateSize = Constants.DEFAULT_ROTATE_SIZE;

        private static Vector3 posBefore = Vector3.zero;
        private static Vector3 scaleBefore = Vector3.zero;
        private static Quaternion rotBefore;
        private Transform[] selectionBefore = Selection.transforms;

        private static bool drawPosX, drawPosY, drawPosZ;
        private static bool drawScaleX, drawScaleY, drawScaleZ;

        private static bool forceRotationMode;
        private static PivotRotation rotationModeBefore;

        public static float GridSize
        {
            get
            {
                return gridSize;
            }
            set
            {
                gridSize = value;
                prefs.Set(Constants.PREF_GRIDSIZE, gridSize);
            }
        }

        [MenuItem(Constants.WINDOW_PATH), MenuItem(Constants.WINDOW_PATH_ALT)]
        public static void OpenWindow()
        {
            CreateWindow();
        }

        private static void CreateWindow()
        {
            var window = EditorWindow.GetWindow<FastGrid>();
		
			#if UNITY_5_4_OR_NEWER
			window.titleContent = new GUIContent(Constants.ASSET_NAME);
			#else
				window.title = Constants.ASSET_NAME;
			#endif
				window.Show();

            window.minSize = Constants.WINDOW_MIN_SIZE;
            window.maxSize = window.minSize;
        }

        void OnEnable()
        {
            LoadSettings();
        }

        void OnDestroy()
        {
            SnapEnabled = false;
        }

        void OnGUI()
        {
            var skin = GUI.skin;
            GUI.skin.label.fontStyle = FontStyle.Bold;
            GUILayout.BeginVertical();

            if (logo == null)
            {
                logo = Base64.FromBase64(Constants.LOGO_BASE64);
            }

            if (logo != null)
            {
                GUI.DrawTexture(Constants.LOGO_RECT, logo, ScaleMode.ScaleToFit);
                GUILayout.Space(55);
            }

            #region snap toggle
            bool snapBefore = SnapEnabled;
            SnapEnabled = DrawToggle(SnapEnabled, "Snap", "Enables Snapping");
            if (SnapEnabled != snapBefore)
            {
                prefs.Set(Constants.PREF_SNAP, SnapEnabled);
            }
            #endregion

            GUILayout.Space(5);

            #region grid toggle
            bool drawBefore = DrawEnabled;
            DrawEnabled = DrawToggle(DrawEnabled && SnapEnabled, "Grid", "shows/hides the grid");
            if (DrawEnabled != drawBefore)
            {
                prefs.Set(Constants.PREF_DRAW, DrawEnabled);
            }
            #endregion

            GUILayout.Space(10);

            #region grid size
            GUILayout.BeginVertical();
            GUILayout.Label(new GUIContent("Snap Value", "the distance between two snap points"));
            float tempGridSize = EditorGUILayout.FloatField(GridSize);
            if (tempGridSize != 0 && !float.IsInfinity(tempGridSize) && tempGridSize >= 0.01f)
                GridSize = tempGridSize;
            GUILayout.EndVertical();
            #endregion

            #region rotation snap
            GUILayout.BeginVertical();
            GUILayout.Label(new GUIContent("Rotation Snap", "the snap angle in degrees"));
            float tempRotateSize = EditorGUILayout.FloatField(rotateSize);
            if (tempRotateSize != 0 && !float.IsInfinity(tempRotateSize))
            {
                rotateSize = tempRotateSize;
                prefs.Set(Constants.PREF_ROTATESIZE, rotateSize);
            }
            GUILayout.EndVertical();
            #endregion

            GUILayout.EndHorizontal();

            GUI.skin = skin;
        }

        void Update()
        {
            if (Selection.transforms.Length <= 0) return;
            Transform first = Selection.transforms[0];

            if (!SnapEnabled)
            {
                UpdateLastSelection(first);
                return;
            }

            bool changed = false;
            if (Selection.transforms.Length != selectionBefore.Length)
                changed = true;
            else
            {
                for (int i = 0; i < selectionBefore.Length; i++)
                {
                    if (Selection.transforms[i] != selectionBefore[i])
                    {
                        changed = true;
                        break;
                    }
                }
            }

            if (changed)
            {
                UpdateLastSelection(first);
            }

            switch (Tools.current)
            {
                case Tool.Move:
                case Tool.Rect:
                    if (posBefore != first.position)
                    {
                        SnapToGrid(Selection.transforms);
                        posBefore = first.position;
                    }
                    break;
                case Tool.Scale:
                    if (scaleBefore != first.localScale)
                    {
                        ScaleToGrid(Selection.transforms);
                        scaleBefore = first.localScale;
                    }
                    break;
                case Tool.Rotate:
                    if (rotBefore != first.rotation)
                    {
                        RotateToAngle(Selection.transforms);
                        rotBefore = first.rotation;
                    }
                    break;
            }

            if (Tools.current == Tool.Rotate)
            {
                if (!forceRotationMode)
                {
                    rotationModeBefore = Tools.pivotRotation;
                    Tools.pivotRotation = PivotRotation.Local;
                    forceRotationMode = true;
                }
            }
            else if (forceRotationMode)
            {
                forceRotationMode = false;
                Tools.pivotRotation = rotationModeBefore;
            }

        }

        private void LoadSettings()
        {
            logo = Base64.FromBase64(Constants.LOGO_BASE64);

            SnapEnabled = prefs.Get(Constants.PREF_SNAP, Constants.DEFAULT_SNAP_ENABLED);
            DrawEnabled = prefs.Get(Constants.PREF_DRAW, Constants.DEFAULT_DRAW_ENABLED);
            GridSize = prefs.Get(Constants.PREF_GRIDSIZE, Constants.DEFAULT_GRID_SIZE);
            rotateSize = prefs.Get(Constants.PREF_ROTATESIZE, Constants.DEFAULT_ROTATE_SIZE);
        }

        private void UpdateLastSelection(Transform first)
        {
            selectionBefore = Selection.transforms;
            posBefore = first.position;
            rotBefore = first.rotation;
            scaleBefore = first.localScale;
        }

        [DrawGizmo(GizmoType.Active)]
        private static void DrawGrid(Transform transform, GizmoType gizmoType)
        {
            if (!(SnapEnabled && DrawEnabled)) return;
            if (Tools.current != Tool.Move && Tools.current != Tool.Scale && Tools.current != Tool.Rect) return;
            if (transform != Selection.transforms[Selection.transforms.Length - 1]) return;

            if (!SceneView.lastActiveSceneView.in2DMode)
            {
                if (drawPosX || drawPosZ || drawScaleX || drawScaleZ)
                {
                    DrawGrid(transform.position, transform.position.x, Constants.COLOR_RED, Vector3.right, Vector3.forward);
                    DrawGrid(transform.position, transform.position.z, Constants.COLOR_BLUE, Vector3.forward, Vector3.right);
                }
                if (drawPosY || drawScaleY)
                {
                    Vector3 camRight = SceneView.currentDrawingSceneView.camera.transform.right;
                    DrawGrid(transform.position, transform.position.y, Constants.COLOR_GREEN, Vector3.up, camRight);
                }
            }
            else
            {
                if (drawPosX || drawPosY || drawScaleX || drawScaleZ)
                {
                    DrawGrid(transform.position, transform.position.x, Constants.COLOR_RED, Vector3.right, Vector3.up);
                    DrawGrid(transform.position, transform.position.y, Constants.COLOR_GREEN, Vector3.up, Vector3.right);
                }
            }
        }

        public static void DrawGrid(Vector3 pos, float dir, Color color, Vector3 levelX, Vector3 levelY)
        {
            for (float x = -Constants.GRID_LINES / 2 * GridSize; x <= Constants.GRID_LINES / 2 * GridSize; x += GridSize)
            {
                float xx = x + dir;
                color.a = (int)xx == xx ? Constants.COLOR_FULL : Constants.COLOR_LIGHT;

                Handles.color = color;
                Handles.DrawLine(pos + levelX * x - levelY * GridSize * Constants.GRID_LINES / 2, pos + levelX * x + levelY * GridSize * Constants.GRID_LINES / 2);
            }
        }

        private void SnapToGrid(Transform[] transforms)
        {
            foreach (Transform t in transforms)
            {
                Vector3 pos = t.position;

                Undo.RecordObject(t, "Snap to Grid");
                var vec = new Vector3(RoundToSnap(t.position.x, GridSize), RoundToSnap(t.position.y, GridSize), RoundToSnap(t.position.z, GridSize));

                drawPosX = pos.x != vec.x;
                drawPosY = pos.y != vec.y;
                drawPosZ = pos.z != vec.z;

                t.position = vec;
                EditorUtility.SetDirty(t);
            }
        }

        private void RotateToAngle(Transform[] transforms)
        {
            foreach (Transform t in transforms)
            {
                Quaternion rotQuat = t.rotation;
                Vector3 rot = rotQuat.eulerAngles;

                Undo.RecordObject(t, "Snap to Angle");

                rot.x = RoundToSnap(rot.x, rotateSize);
                rot.y = RoundToSnap(rot.y, rotateSize);
                rot.z = RoundToSnap(rot.z, rotateSize);

                t.rotation = Quaternion.Euler(rot);
                EditorUtility.SetDirty(t);
            }
        }

        private void ScaleToGrid(Transform[] transforms)
        {
            foreach (Transform t in transforms)
            {
                Vector3 scale = t.localScale;

                Undo.RecordObject(t, "Scale to Grid");
                var vec = new Vector3(RoundScaleToSnap(scale.x, gridSize), RoundScaleToSnap(scale.y, gridSize), RoundScaleToSnap(scale.z, gridSize));

                drawScaleX = scale.x != vec.x;
                drawScaleY = scale.y != vec.y;
                drawScaleZ = scale.z != vec.z;

                t.localScale = vec;
                EditorUtility.SetDirty(t);
            }
        }

        private float RoundToSnap(float input, float snap)
        {
            return (Mathf.Round(input / snap) * snap);
        }

        private float RoundScaleToSnap(float input, float snap)
        {
            return Mathf.Clamp(Mathf.Round(input / snap) * snap, snap, float.MaxValue);
        }

        private static bool DrawToggle(bool val, string title, string tooltip)
        {
            GUILayout.BeginVertical();
            GUILayout.Label(title);
            var col = GUI.backgroundColor;
            GUI.backgroundColor = val ? col : Color.grey;
            bool pressed = GUILayout.Button(new GUIContent(val ? "On" : "Off", tooltip));
            GUI.backgroundColor = col;
            GUILayout.EndVertical();
            return pressed ? !val : val;
        }
    }
}
