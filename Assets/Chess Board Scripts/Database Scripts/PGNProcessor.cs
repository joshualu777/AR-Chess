using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class PGNProcessor
{
    private Database database;

    private int index;

    public PGNProcessor(Database database)
    {
        this.database = database;
    }

    public Database GetDatabase() { return database; }

    public void ProcessGame(string gameString)
    {
        ResetIndex();
        char[] gameArray = gameString.ToCharArray();
        Game game = new Game(ProcessHeader(gameArray));
        Move start = new Move();
        Move previous = start;
        string pretext = "";
        bool startLine = true;
        Stack<Move> diverge = new Stack<Move>();
        Stack<int> prefix = new Stack<int>();
        int depth = 0;

        Debug.Log(gameString);
        while (index < gameArray.Length)
        {
            if (gameArray[index] == '*') break;

            string checkResult = "" + gameArray[index] + gameArray[index + 1] + gameArray[index + 2];
            if (checkResult.Equals("1-0") || checkResult.Equals("0-1") || checkResult.Equals("1/2-1/2")) break;

            if (char.IsDigit(gameArray[index]) || char.IsLetter(gameArray[index]))
            {
                if (!char.IsLetter(gameArray[index]))
                {
                    index = NextCharacter(gameArray, index, ' ') + 1;
                }
                int end = NextCharacter(gameArray, index, ' ', ')') - 1;
                string move = BuildString(gameArray, index, end);

                if (startLine && diverge.Count > 0)
                {
                    Move curMove = new Move(move, diverge.Peek().GetPreviousMove());
                    previous.AddVariation(curMove);
                    curMove.SetPreText(pretext);
                    pretext = "";
                    previous = curMove;
                }
                else
                {
                    Move curMove = new Move(move, previous);
                    previous = curMove;
                    Debug.Log(curMove.GetMoveNumber());
                }

                if (gameArray[end + 1] == ')')
                {
                    index = end + 1;
                }
                else
                {
                    index = end + 2;
                }
                startLine = false;

                if (game.GetFirstMove() == null)
                {
                    game.SetFirstMove(previous);
                }
            }
            else if (gameArray[index] == '{')
            {
                index++;
                if (gameArray[index] == '[')
                {
                    index = NextCharacter(gameArray, index + 1, ']') + 2;
                }
                int end = NextCharacter(gameArray, index - 2, '}') - 1;
                string text = BuildString(gameArray, index, end);
                if (startLine)
                {
                    pretext = text;
                }
                else
                {
                    previous.SetPostText(text);
                }

                if (gameArray[end + 2] == ')')
                {
                    index = end + 2;
                }
                else
                {
                    index = end + 3;
                }
            }
            else if (gameArray[index] == '(')
            {
                startLine = true;
                index++;
                diverge.Push(previous);
                prefix.Push(++depth);
            }
            else if (gameArray[index] == ')')
            {
                depth--;
                if (gameArray[index + 2] != '(')
                {
                    while (prefix.Count > 0 && prefix.Peek() > depth)
                    {
                        prefix.Pop();
                        previous = diverge.Pop();
                    }
                }
                if (gameArray[index + 1] == ')')
                {
                    index++;
                }
                else
                {
                    index += 2;
                }
            }
            else if (gameArray[index] == '$' || gameArray[index] == '/')
            {
                int end = NextCharacter(gameArray, index, ' ', ')') - 1;
                string annotation = BuildString(gameArray, index, end);
                previous.SetAnnotation(annotation);
                if (gameArray[end + 1] == ')')
                {
                    index = end + 1;
                }
                else
                {
                    index = end + 2;
                }
            }
        }
        database.AddGame(game);
    }
    private List<string> ProcessHeader(char[] game)
    {
        List<string> header = new List<string>();
        while (index < game.Length)
        {
            if (game[index] == '[')
            {
                string line = "";
                index++;
                int keyWordEnd = NextCharacter(game, index, ' ') - 1;
                line += BuildString(game, index, keyWordEnd);
                line += " ";
                int textStart = keyWordEnd + 3;
                int textEnd = NextCharacter(game, textStart, '\"') - 1;
                line += BuildString(game, textStart, textEnd);
                header.Add(line);
                index = textEnd + 4;
                Debug.Log(line);
            }
            else
            {
                break;
            }
        }
        return header;
    }
    private int NextCharacter(char[] array, int start, char search)
    {
        int location = start;
        while (location < array.Length)
        {
            if (array[location] == search)
            {
                return location;
            }
            location++;
        }
        return -1;
    }
    private int NextCharacter(char[] array, int start, char search1, char search2)
    {
        int location = start;
        while (location < array.Length)
        {
            if (array[location] == search1 || array[location] == search2)
            {
                return location;
            }
            location++;
        }
        return -1;
    }
    private string BuildString(char[] array, int start, int end)
    {
        string result = "";
        for (int i = start; i <= end; i++)
        {
            result += array[i];
        }
        return result;
    }
    private void ResetIndex()
    {
        index = 0;
    }
}
