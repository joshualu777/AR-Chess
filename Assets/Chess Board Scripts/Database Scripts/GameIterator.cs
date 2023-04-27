using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameIterator : MonoBehaviour
{
    private Move currentMove;
    private Game game;

    private Stack<Move> diverge;

    public GameIterator(Game game)
    {
        this.game = game;
        currentMove = new Move();
        currentMove.SetNextMove(game.GetFirstMove());
        diverge = new Stack<Move>();
    }

    public Move GetCurrentMove() { return currentMove; }

    public Move NextMove()
    {
        if (currentMove.GetMoveNumber() == 0)
        {
            currentMove = game.GetFirstMove();
        }
        else if (currentMove.GetNextMove() != null)
        {
            currentMove = currentMove.GetNextMove();
        }
        return currentMove;
    }
    public Move NextMove(int options)
    {
        if (currentMove.GetMoveNumber() == 0)
        {
            if (options < game.GetFirstMove().GetVariations().Count)
            {
                currentMove = game.GetFirstMove().GetVariations()[options];
            }
        }
        else if (currentMove.GetNextMove() != null)
        {
            if (options < currentMove.GetNextMove().GetVariations().Count)
            {
                diverge.Push(currentMove);
                currentMove = currentMove.GetNextMove().GetVariations()[options];
            }
        }
        return currentMove;
    }
    public Move PreviousMove()
    {
        if (currentMove.GetMoveNumber() != 0)
        {
            currentMove = currentMove.GetPreviousMove();
            if (diverge.Count > 0 && currentMove.Equals(diverge.Peek()))
            {
                diverge.Pop();
            }
        }
        return currentMove;
    }
    public Move returnLine()
    {
        if (diverge.Count > 0)
        {
            currentMove = diverge.Pop();
        }
        return currentMove;
    }

    public bool AddMove(Move move)
    {
        if (!currentMove.ContainsMove(move))
        {
            move.SetUserEdited(true);
            if (currentMove.GetNextMove() != null)
            {
                currentMove.GetNextMove().AddVariation(move);
            }
            else
            {
                currentMove.SetNextMove(move);
            }
            return true;
        }
        return false;
    }
    public bool DeleteMove()
    {
        if (currentMove.GetUserEdited())
        {
            if (currentMove.GetPreviousMove().GetNextMove().Equals(currentMove))
            {
                currentMove.GetPreviousMove().SetNextMove(null);
            }
            else
            {
                currentMove.GetPreviousMove().DeleteVariation(currentMove);
            }
            currentMove = currentMove.GetPreviousMove();
            return true;
        }
        return false;
    }
}
