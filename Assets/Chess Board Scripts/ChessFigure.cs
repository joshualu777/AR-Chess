using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ChessFigure : MonoBehaviour
{
    public int CurrentR { get; set; }
    public int CurrentC { get; set; }
    public bool isWhite;

    public PieceType pieceType;

    public void SetPosition(int r, int c)
    {
        CurrentR = r;
        CurrentC = c;
    }

    public void CheckPossibleMove()
    {
        bool[,] legal = PossibleMove();
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                
            }
        }
    }

    public virtual bool[,] PossibleMove()
    {
        return new bool[8, 8];
    }
}
