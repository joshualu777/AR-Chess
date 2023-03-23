using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

public class LegalMoveManager : MonoBehaviour
{
    private int kingR;
    private int kingC;

    private int[] dr_knight = { -2, -2, -1, 1, 2, 2, 1, -1 };
    private int[] dc_knight = { -1, 1, 2, 2, 1, -1, -2, -2 };

    private int[] dr_bishop = { -1, -1, 1, 1 };
    private int[] dc_bishop = { -1, 1, -1, 1 };

    private int[] dr_rook = { -1, 0, 1, 0 };
    private int[] dc_rook = { 0, 1, 0, -1 };

    private static LegalMoveManager _instance;

    public static LegalMoveManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<LegalMoveManager>();
            }

            return _instance;
        }
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public bool CheckLegal()
    {
        ChessFigure[,] board = BoardManager.Instance.figurePositions;
        FindKing(board);
        
        //knight moves
        for (int i = 0; i < dr_knight.Length; i++)
        {

        }

        return true;
    }
    
    private void FindKing(ChessFigure[,] board)
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (board[i, j] != null && 
                    board[i, j].isWhite == BoardManager.Instance.isWhiteTurn &&
                    board[i, j].pieceType == PieceType.King)
                {
                    kingR = i;
                    kingC = j;
                    return;
                }
            }
        }
    }
}
