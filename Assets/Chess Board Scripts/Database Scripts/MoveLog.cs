using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLog
{
    public int index;

    public int startR;
    public int startC;
    public bool hasMoved;

    public int endR;
    public int endC;
    public GameObject capturedPieceObject;
    public ChessFigure capturedPieceFigure;

    public MoveLog(int index, int startR, int startC, bool hasMoved, int endR, int endC, 
        GameObject capturedPieceObject = null, ChessFigure capturedPieceFigure = null)
    {
        this.index = index;
        this.startR = startR;
        this.startC = startC;
        this.hasMoved = hasMoved;
        this.endR = endR;
        this.endC = endC;
        this.capturedPieceObject = capturedPieceObject;
        this.capturedPieceFigure = capturedPieceFigure;
    }
}
