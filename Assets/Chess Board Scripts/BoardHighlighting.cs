using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardHighlighting : MonoBehaviour
{
    public GameObject highlightPrefab;
    private List<GameObject> highlights;

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
                    Debug.Log(r + " " + c);
                    GameObject go = GetHighlightObject();
                    go.SetActive(true);
                    go.transform.position = new Vector3(c + 0.5f, 0, (7 - r) + 0.5f);
                }
            }
        }
    }

    public void HideHighlights()
    {
        foreach (GameObject go in highlights) go.SetActive(false);
    }
}
