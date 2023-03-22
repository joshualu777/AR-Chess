using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ChessFigure : MonoBehaviour
{
    public int CurrentR { get; set; }
    public int CurrentC { get; set; }
    public bool isWhite;

    public void SetPosition(int r, int c)
    {
        CurrentR = r;
        CurrentC = c;
    }

    public virtual bool[,] PossibleMove()
    {
        return new bool[8, 8];
    }
}
