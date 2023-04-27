using Microsoft.MixedReality.Toolkit;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : IComparable<Move>
{
    private const int BREAK_TEXT = 50;

    private string move;

    private string preText;
    private string postText;
    private string annotation;

    private int moveNumber;
    private bool isWhite;
    private bool userEdited;

    private Move previousMove;
    private Move nextMove;
    private List<Move> variations;

    public Move(string move, Move previousMove)
    {
        this.move = move;
        SetPreviousMove(previousMove);
        if (previousMove.GetNextMove() == null)
        {
            previousMove.SetNextMove(this);
        }
        Initialize();
        SetSequence();
    }

    public Move()
    {
        this.move = "Start";
        moveNumber = 0;
        isWhite = false;
        Initialize();
    }

    public String GetMove() { return move; }
    public String GetPreText() { return FormatText(preText); }
    public String GetPostText() { return FormatText(postText); }
    public String GetAnnotation() { return annotation; }
    public int GetMoveNumber() { return moveNumber; }
    public bool GetIsWhite() { return isWhite; }
    public bool GetUserEdited() { return userEdited; }
    public Move GetPreviousMove() { return previousMove; }
    public Move GetNextMove() {  return nextMove; }
    public List<Move> GetVariations() { return variations; }

    public void SetPreText(string text) { preText += text; }
    public void SetPostText(string text) {  postText += text; }
    public void SetAnnotation(string text) { annotation += text; }
    public void SetUserEdited(bool byUser) { userEdited = byUser; }
    public void SetPreviousMove(Move move) { previousMove = move; }
    public void SetNextMove(Move move) {  nextMove = move; }

    public void AddVariation(Move newMove) { variations.Add(newMove); }
    public void DeleteVariation(Move move) { variations.Remove(move); }
    public bool ContainsMove(Move move) { return nextMove.Equals(move) || nextMove.GetVariations().Contains(move); }

    public int CompareTo(Move other)
    {
        if (this.GetMoveNumber() != other.GetMoveNumber())
        {
            return this.GetMoveNumber() - other.GetMoveNumber();
        }
        return this.GetMove().CompareTo(other.GetMove());
    }

    public string GetFormattedMove()
    {
        string result = "";
        if (preText.Length != 0)
        {
            result += GetPreText() + "\n\n";
        }
        result += SimpleMove();
        if (postText.Length != 0)
        {
            result += "\n\n" + GetPostText();
        }
        if (nextMove != null)
        {
            result += "\n\nNext move in the current line:";
            result += "\n\t" + this.GetNextMove().SimpleMove();
            if (this.GetNextMove().GetVariations().Count != 0)
            {
                result += "\n\n" + "Here are some alternative variations:";
                int index = 1;
                foreach (Move move in this.GetNextMove().GetVariations())
                {
                    result += "\n\t" + index++ + ") " + move.SimpleMove();
                }
            }
        }
        else
        {
            result += "\n\nEnd of sequence";
        }
        return result;
    }
    private string SimpleMove()
    {
        string printedMove = "" + GetMoveNumber();
        if (isWhite)
        {
            printedMove += ". " + GetMove();
        }
        else
        {
            printedMove += "... " + GetMove();
        }
        return printedMove;
    }

    private void Initialize()
    {
        preText = "";
        postText = "";
        annotation = "";
        userEdited = false;
        variations = new List<Move>();
    }
    private void SetSequence()
    {
        if (previousMove.isWhite)
        {
            isWhite = false;
            moveNumber = previousMove.GetMoveNumber();
        }
        else
        {
            isWhite = true;
            moveNumber = previousMove.GetMoveNumber() + 1;
        }
    }
    private string FormatText(string text)
    {
        string[] paragraph = text.Split(' ');
        string result = "";
        int charCount = 0;
        for (int i = 0; i < paragraph.Length; i++) {
            if (charCount + paragraph[i].Length <= BREAK_TEXT)
            {
                result += paragraph[i] + " ";
                charCount += paragraph[i].Length + 1;
            }
            else
            {
                if (charCount != 0) result += "\n";
                charCount = 0;
                result += paragraph[i] + " ";
                charCount += paragraph[i].Length + 1;
            }
        }
        return result;
    }
}
