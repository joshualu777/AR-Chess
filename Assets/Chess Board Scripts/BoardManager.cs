using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public ChessFigure[,] figurePositions;
    public int powerR;
    public int powerC;

    private static BoardManager _instance;

    public static BoardManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<BoardManager>();
            }

            return _instance;
        }
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        figurePositions = new ChessFigure[8, 8];
        powerR = -1;
        powerC = -1;
    }

    void Update()
    {
        
    }
}
