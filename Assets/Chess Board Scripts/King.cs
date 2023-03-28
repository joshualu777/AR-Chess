using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : ChessFigure
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

        if (!hasMoved)
        {
            //making sure king is currently not in check
            if (LegalMoveManager.Instance.CheckLegal())
            {
                ChessFigure[,] board = BoardManager.Instance.figurePositions;
                int kingR;
                if (isWhite)
                {
                    kingR = 0;
                }
                else
                {
                    kingR = 7;
                }

                //king is at position [kingR, 3]

                /*
                 * check kingside castle
                 * 1) make sure squares are empty
                 * 2) make sure rook has not moved
                 */
                if (board[kingR, 2] == null && board[kingR, 1] == null &&
                    (board[kingR, 0] != null &&
                    board[kingR, 0].pieceType == PieceType.Rook &&
                    !board[kingR, 0].hasMoved))
                {
                    //king cannot castle through check
                    board[kingR, 2] = board[kingR, 3]; board[kingR, 3] = null;
                    bool legal1 = LegalMoveManager.Instance.CheckLegal();
                    board[kingR, 1] = board[kingR, 2]; board[kingR, 2] = null;
                    bool legal2 = LegalMoveManager.Instance.CheckLegal();

                    if (legal1 && legal2)
                    {
                        result[kingR, 1] = true;
                        //mark as special
                    }

                    board[kingR, 3] = board[kingR, 1]; board[kingR, 1] = null;
                }

                /*
                 * check queenside castle
                 * 1) make sure squares are empty
                 * 2) make sure rook has not moved
                 */
                if (board[kingR, 4] == null && board[kingR, 5] == null &&
                    (board[kingR, 7] != null &&
                    board[kingR, 7].pieceType == PieceType.Rook &&
                    !board[kingR, 7].hasMoved))
                {
                    //king cannot castle through check
                    board[kingR, 4] = board[kingR, 3]; board[kingR, 3] = null;
                    bool legal1 = LegalMoveManager.Instance.CheckLegal();
                    board[kingR, 5] = board[kingR, 4]; board[kingR, 4] = null;
                    bool legal2 = LegalMoveManager.Instance.CheckLegal();

                    if (legal1 && legal2)
                    {
                        result[kingR, 5] = true;
                        //mark as special
                    }

                    board[kingR, 3] = board[kingR, 5]; board[kingR, 5] = null;
                }
            }
        }

        return result;
    }
}
