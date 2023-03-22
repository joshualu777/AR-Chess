using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : ChessFigure
{
    private int[] dr = { -1, 0, 1, 0, -1, -1, 1, 1 };
    private int[] dc = { 0, 1, 0, -1, -1, 1, -1, 1 };

    public override bool[,] PossibleMove()
    {
        bool[,] result = new bool[8, 8];

        for (int i = 0; i < dr.Length; i++) //iterates through the 8 directions
        {
            int r = CurrentR + dr[i];
            int c = CurrentC + dc[i];

            while (r >= 0 && r < 8 && c >= 0 && c < 8)
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
                    break;
                }
                r += dr[i];
                c += dc[i];
            }
        }

        return result;
    }
}
