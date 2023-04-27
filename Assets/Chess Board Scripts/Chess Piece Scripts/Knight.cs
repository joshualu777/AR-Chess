using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : ChessFigure
{
    private int[] dr = { -2, -2, -1, 1, 2, 2, 1, -1 };
    private int[] dc = { -1, 1, 2, 2, 1, -1, -2, -2 };

    public override bool[,] PossibleMove()
    {
        bool[,] result = new bool[8, 8];
        Debug.Log("Original position: " + CurrentR + " " + CurrentC);
        for (int i = 0; i < dr.Length; i++) //iterates through the 8 directions
        {
            int r = CurrentR + dr[i];
            int c = CurrentC + dc[i];
            Debug.Log("Testing position: " + r + " " + c);

            if (r >= 0 && r < 8 && c >= 0 && c < 8)
            {
                ChessFigure piece = BoardManager.Instance.figurePositions[r, c];
                if (piece == null)
                {
                    result[r, c] = true;
                }
                else
                {
                    if (piece.isWhite != this.isWhite)
                    {
                        result[r, c] = true;
                    }
                }
            }
        }

        return result;
    }
}
