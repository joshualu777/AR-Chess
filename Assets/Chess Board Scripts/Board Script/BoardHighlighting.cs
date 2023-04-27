using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;

public class BoardHighlighting : MonoBehaviour
{
    public GameObject highlightPrefab;
    private List<GameObject> highlights;
    private float scale;

    private static BoardHighlighting _instance;

    public static BoardHighlighting Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<BoardHighlighting>();
            }

            return _instance;
        }
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        highlights = new List<GameObject>();
        scale = highlightPrefab.transform.localScale.x * BoardManager.Instance.tileSize;
    }

    private GameObject GetHighlightObject()
    {
        GameObject go = highlights.Find(g => !g.activeSelf);
        if (go == null)
        {
            go = Instantiate(highlightPrefab);
            highlights.Add(go);
        }
        return go;
    }

    public void HighlightAllowedMoves(bool[,] moves)
    {
        for (int r = 0; r < 8; r++)
        {
            for (int c = 0; c < 8; c++)
            {
                if (moves[r, c])
                {
                    float tileSize = BoardManager.Instance.tileSize;
                    Debug.Log(r + " " + c);
                    GameObject go = GetHighlightObject();
                    go.SetActive(true);
                    float offSet = 0.5f * tileSize;
                    go.transform.position = new Vector3(c * tileSize + offSet, 0, (7 - r) * tileSize + offSet);

                    go.transform.localScale = new Vector3(scale, scale / 100, scale);
                }
            }
        }
    }

    public void HideHighlights()
    {
        foreach (GameObject go in highlights) go.SetActive(false);
    }
}
