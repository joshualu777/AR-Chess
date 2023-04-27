using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : ChessFigure
{
    public override bool[,] PossibleMove()
    {
        bool[,] result = new bool[8, 8];

        if (isWhite)
        {
            //Capture left
            if (CurrentC != 0)
            {
                ChessFigure c = BoardManager.Instance.figurePositions[CurrentR + 1, CurrentC - 1];
                if (c != null && !c.isWhite)
                {
                    result[CurrentR + 1, CurrentC - 1] = true;
                }
                if (CurrentR == 4) //en passant
                {
                    int p_r = BoardManager.Instance.powerR;
                    int p_c = BoardManager.Instance.powerC;
                    if (p_r == CurrentR && p_c == CurrentC - 1)
                    {
                        result[CurrentR + 1, CurrentC - 1] = true;
                        //must also take the piece
                    }
                }
            }

            //Capture right
            if (CurrentC != 7)
            {
                ChessFigure c = BoardManager.Instance.figurePositions[CurrentR + 1, CurrentC + 1];
                if (c != null && !c.isWhite)
                {
                    result[CurrentR + 1, CurrentC + 1] = true;
                }
                if (CurrentR == 4) //en passant
                {
                    int p_r = BoardManager.Instance.powerR;
                    int p_c = BoardManager.Instance.powerC;
                    if (p_r == CurrentR && p_c == CurrentC + 1)
                    {
                        result[CurrentR + 1, CurrentC + 1] = true;
                        //must also take the piece
                    }
                }
            }

            //Two steps forward
            if (CurrentR == 1)
            {
                ChessFigure c1 = BoardManager.Instance.figurePositions[CurrentR + 1, CurrentC];
                ChessFigure c2 = BoardManager.Instance.figurePositions[CurrentR + 2, CurrentC];
                if (c1 == null && c2 == null)
                {
                    result[CurrentR + 2, CurrentC] = true;
                }

                //mark en passant
            }

            //One step forward
            if (true) //always true
            {
                ChessFigure c = BoardManager.Instance.figurePositions[CurrentR + 1, CurrentC];
                if (c == null)
                {
                    result[CurrentR + 1, CurrentC] = true;
                }
            }
        }
        else //black pieces
        {
            //Capture left
            if (CurrentC != 0)
            {
                ChessFigure c = BoardManager.Instance.figurePositions[CurrentR - 1, CurrentC - 1];
                if (c != null && c.isWhite)
                {
                    result[CurrentR - 1, CurrentC - 1] = true;
                }
                if (CurrentR == 3) //en passant
                {
                    int p_r = BoardManager.Instance.powerR;
                    int p_c = BoardManager.Instance.powerC;
                    if (p_r == CurrentR && p_c == CurrentC - 1)
                    {
                        result[CurrentR - 1, CurrentC - 1] = true;
                        //must also take the piece
                    }
                }
            }

            //Capture right
            if (CurrentC != 7)
            {
                ChessFigure c = BoardManager.Instance.figurePositions[CurrentR - 1, CurrentC + 1];
                if (c != null && c.isWhite)
                {
                    result[CurrentR - 1, CurrentC + 1] = true;
                }
                if (CurrentR == 3) //en passant
                {
                    int p_r = BoardManager.Instance.powerR;
                    int p_c = BoardManager.Instance.powerC;
                    if (p_r == CurrentR && p_c == CurrentC + 1)
                    {
                        result[CurrentR - 1, CurrentC + 1] = true;
                        //must also take the piece
                    }
                }
            }

            //Two steps forward
            if (CurrentR == 6)
            {
                ChessFigure c1 = BoardManager.Instance.figurePositions[CurrentR - 1, CurrentC];
                ChessFigure c2 = BoardManager.Instance.figurePositions[CurrentR - 2, CurrentC];
                if (c1 == null && c2 == null)
                {
                    result[CurrentR - 2, CurrentC] = true;
                }

                //mark en passant
            }

            //One step forward
            if (true) //always true
            {
                ChessFigure c = BoardManager.Instance.figurePositions[CurrentR - 1, CurrentC];
                if (c == null)
                {
                    result[CurrentR - 1, CurrentC] = true;
                }
            }
        }

        return result;
    }
}
