using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MoveReaderController : MonoBehaviour
{
    private Dictionary<char, PieceType> pieceMap;
    
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

    private static MoveReaderController _instance;

    public static MoveReaderController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<MoveReaderController>();
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
        pieceMap = new Dictionary<char, PieceType>();
        pieceMap.Add('K', PieceType.King);
        pieceMap.Add('Q', PieceType.Queen);
        pieceMap.Add('R', PieceType.Rook);
        pieceMap.Add('B', PieceType.Bishop);
        pieceMap.Add('N', PieceType.Knight);
    }

    public void PlayMove(string move)
    {
        if (move.Length == 0)
        {
            return;
        }
        Debug.Log(move);

        ChessFigure[,] board = BoardManager.Instance.figurePositions;
        bool color = BoardManager.Instance.isWhiteTurn;

        if (move[0] == 'O')
        {
            if (color)
            {
                if (move.Length == 3)
                {
                    BoardManager.Instance.MakeManualMove(0, 3, 0, 1);
                }
                else
                {
                    BoardManager.Instance.MakeManualMove(0, 3, 0, 5);
                }
            }
            else
            {
                if (move.Length == 3)
                {
                    BoardManager.Instance.MakeManualMove(7, 3, 7, 1);
                }
                else
                {
                    BoardManager.Instance.MakeManualMove(7, 3, 7, 5);
                }
            }
            return;
        }

        int last;
        for (last = move.Length - 1; last >= 0; last--)
        {
            if (char.IsDigit(move[last]))
            {
                break;
            }
        }

        int endR = move[last] - '1';
        int endC = 7 - (move[last - 1] - 'a');

        int startSquare = -1;
        if (char.IsUpper(move[0])) //piece
        {
            char specialLocation = move[last - 2];
            if (specialLocation == 'x')
            {
                specialLocation = move[last - 3];
            }
            if (char.IsLetter(specialLocation) && char.IsUpper(specialLocation))
            {
                startSquare = FindPiece(pieceMap[move[0]], color, endR, endC, board);
            }
            else
            {
                startSquare = FindPiece(pieceMap[move[0]], color, endR, endC, board, specialLocation);
            }
        }
        else //pawn
        {
            if (color)
            {
                if (move[1] == 'x') //capture piece
                {
                    int startC = 7 - (move[0] - 'a');
                    startSquare = 8 * (endR - 1) + startC;
                }
                else
                {
                    if (board[endR - 1, endC] == null)
                    {
                        startSquare = 8 * (endR - 2) + endC;
                    }
                    else
                    {
                        startSquare = 8 * (endR - 1) + endC;
                    }
                }
            }
            else
            {
                if (move[1] == 'x') //capture piece
                {
                    int startC = 7 - (move[0] - 'a');
                    startSquare = 8 * (endR + 1) + startC;
                }
                else
                {
                    if (board[endR + 1, endC] == null)
                    {
                        startSquare = 8 * (endR + 2) + endC;
                    }
                    else
                    {
                        startSquare = 8 * (endR + 1) + endC;
                    }
                }
            }
        }
        BoardManager.Instance.MakeManualMove(startSquare / 8, startSquare % 8, endR, endC);
    }

    private int FindPiece(PieceType type, bool color, int endR, int endC, 
        ChessFigure[,] board, char optional = ' ')
    {
        for (int r = 0; r < 8; r++)
        {
            if (char.IsDigit(optional))
            {
                int locationR = optional - '1';
                if (r != locationR)
                {
                    continue;
                }
            }
            for (int c = 0; c < 8; c++)
            {
                if (char.IsLetter(optional))
                {
                    int locationC = 7 - (optional - 'a');
                    if (c != locationC)
                    {
                        continue;
                    }
                }
                if (board[r, c] != null && board[r, c].isWhite == color && 
                    board[r, c].pieceType == type)
                {
                    bool found = false;
                    if (type == PieceType.King)
                    {
                        found = TestPiece(r, c, endR, endC, board, dr_king, dc_king, false);
                    }
                    else if (type == PieceType.Queen)
                    {
                        found = TestPiece(r, c, endR, endC, board, dr_king, dc_king, true); //king moves work for queen
                    }
                    else if (type == PieceType.Rook)
                    {
                        found = TestPiece(r, c, endR, endC, board, dr_rook, dc_rook, true);
                    }
                    else if (type == PieceType.Bishop)
                    {
                        found = TestPiece(r, c, endR, endC, board, dr_bishop, dc_bishop, true);
                    }
                    else if (type == PieceType.Knight)
                    {
                        found = TestPiece(r, c, endR, endC, board, dr_knight, dc_knight, false);
                    }

                    if (found)
                    {
                        return 8 * r + c;
                    }
                }
            }
        }
        Debug.Log("Piece not found: " + type + ", " + endR + ", " + endC + ", " + optional);
        return -1;
    }

    private bool TestPiece(int r, int c, int endR, int endC, 
        ChessFigure[,] board, int[] dr, int[] dc, bool continuous)
    {
        for (int i = 0; i < dr.Length; i++)
        {
            int nr = endR + dr[i];
            int nc = endC + dc[i];

            while (nr >= 0 && nr < 8 && nc >= 0 && nc < 8)
            {
                if (board[nr, nc] != null)
                {
                    if (nr == r && nc == c)
                    {
                        return true;
                    }
                    else
                    {
                        break;
                    }
                }
                if (!continuous)
                {
                    break;
                }
                nr += dr[i];
                nc += dc[i];
            }
        }
        return false;
    }
}