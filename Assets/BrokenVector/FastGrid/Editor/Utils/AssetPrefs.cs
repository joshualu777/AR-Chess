using UnityEditor;

namespace BrokenVector.FastGrid.Utils
{
	public class AssetPrefs 
	{
        private const char SEPERATOR = '_';

        private string asset;

        private string prefix
        {
            get
            {
                return asset + SEPERATOR;
            }
        }

        public AssetPrefs(string asset)
        {
            this.asset = asset;
        }

        #region string
        public void Set(string key, string val)
        {
            EditorPrefs.SetString(CreatePath(key), val);
        }

        public string Get(string key, string def)
        {
            return EditorPrefs.GetString(CreatePath(key), def);
        }
        #endregion

        #region float
        public void Set(string key, float val)
        {
            EditorPrefs.SetFloat(CreatePath(key), val);
        }

        public float Get(string key, float def)
        {
            return EditorPrefs.GetFloat(CreatePath(key), def);
        }
        #endregion

        #region bool
        public void Set(string key, bool val)
        {
            EditorPrefs.SetBool(CreatePath(key), val);
        }

        public bool Get(string key, bool def)
        {
            return EditorPrefs.GetBool(CreatePath(key), def);
        }
        #endregion

        private string CreatePath(string key)
        {
            return prefix + key;
        }
	}
}