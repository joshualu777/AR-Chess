using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : ChessFigure
{
    private int[] dr = { -1, 0, 1, 0 };
    private int[] dc = { 0, 1, 0, -1 };

    public override bool[,] PossibleMove()
    {
        bool[,] result = new bool[8, 8];

        for (int i = 0; i < dr.Length; i++) //iterates through the 4 directions
        {
            int r = CurrentR;
            int c = CurrentC;

            while (r >= 0 && r < 8 && c >= 0 && c < 8)
            {
                if (r == CurrentR && c == CurrentC)
                {
                    continue;
                }

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
