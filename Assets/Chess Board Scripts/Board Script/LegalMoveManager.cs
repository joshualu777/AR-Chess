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

    private int[] dr_king = { -1, 0, 1, 0, -1, -1, 1, 1 };
    private int[] dc_king = { 0, 1, 0, -1, -1, 1, -1, 1 };

    private int[] dr_pawn = { -1, -1, 1, 1 };
    private int[] dc_pawn = { -1, 1, -1, 1 };

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
        bool isWhiteTurn = BoardManager.Instance.isWhiteTurn;
        FindKing(board);
        
        //knight moves
        for (int i = 0; i < dr_knight.Length; i++)
        {
            int nr = kingR + dr_knight[i];
            int nc = kingC + dc_knight[i];
            if (nr >= 0 && nr < 8 && nc >= 0 && nc < 8)
            {
                if (CheckSquare(nr, nc, !isWhiteTurn, board, PieceType.Knight))
                {
                    return false;
                }
            }
        }

        //enemy king moves
        for (int i = 0; i < dr_king.Length; i++)
        {
            int nr = kingR + dr_king[i];
            int nc = kingC + dc_king[i];
            if (nr >= 0 && nr < 8 && nc >= 0 && nc < 8)
            {
                if (CheckSquare(nr, nc, !isWhiteTurn, board, PieceType.King))
                {
                    return false;
                }
            }
        }

        //pawn moves
        for (int i = 0; i < dr_pawn.Length; i++)
        {
            int nr = kingR + dr_pawn[i];
            int nc = kingC + dc_pawn[i];
            if (nr >= 0 && nr < 8 && nc >= 0 && nc < 8)
            {
                if (isWhiteTurn)
                {
                    if (kingR < nr)
                    {
                        if (CheckSquare(nr, nc, !isWhiteTurn, board, PieceType.Pawn))
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    if (kingR > nr)
                    {
                        if (CheckSquare(nr, nc, !isWhiteTurn, board, PieceType.Pawn))
                        {
                            return false;
                        }
                    }
                }
            }
        }

        //bishop moves + queen
        for (int i = 0; i < dr_bishop.Length; i++)
        {
            int nr = kingR + dr_bishop[i];
            int nc = kingC + dc_bishop[i];

            while (nr >= 0 && nr < 8 && nc >= 0 && nc < 8)
            {
                if (board[nr, nc] != null)
                {
                    if (board[nr, nc].isWhite == isWhiteTurn)
                    {
                        break;
                    }
                    else
                    {
                        if (CheckSquare(nr, nc, !isWhiteTurn, board, PieceType.Bishop) ||
                            CheckSquare(nr, nc, !isWhiteTurn, board, PieceType.Queen))
                        {
                            return false;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                nr += dr_bishop[i];
                nc += dc_bishop[i];
            }
        }

        //rook moves + queen
        for (int i = 0; i < dr_rook.Length; i++)
        {
            int nr = kingR + dr_rook[i];
            int nc = kingC + dc_rook[i];

            while (nr >= 0 && nr < 8 && nc >= 0 && nc < 8)
            {
                if (board[nr, nc] != null)
                {
                    if (board[nr, nc].isWhite == isWhiteTurn)
                    {
                        break;
                    }
                    else
                    {
                        if (CheckSquare(nr, nc, !isWhiteTurn, board, PieceType.Rook) ||
                            CheckSquare(nr, nc, !isWhiteTurn, board, PieceType.Queen))
                        {
                            return false;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                nr += dr_rook[i];
                nc += dc_rook[i];
            }
        }

        return true;
    }
    
    private void FindKing(ChessFigure[,] board)
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (CheckSquare(i, j, BoardManager.Instance.isWhiteTurn, board, PieceType.King))
                {
                    kingR = i;
                    kingC = j;
                    return;
                }
            }
        }
    }
    private bool CheckSquare(int r, int c, bool color, ChessFigure[,] board, PieceType type)
    {
        if (board[r, c] != null &&
            board[r, c].isWhite == color &&
            board[r, c].pieceType == type)
        {
            return true;
        }
        return false;
    }
}
