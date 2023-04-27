using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DatabaseController : MonoBehaviour
{
    public TextAsset[] gamesTextFiles;
    public TextMeshPro annotationText;

    private Database database;
    private PGNProcessor processor;
    private bool isReady;

    private GameIterator iterator;
    private TextController textController;
    
    void Start()
    {
        database = new Database();
        processor = new PGNProcessor(database);
        isReady = true;
        textController = annotationText.GetComponent<TextController>();
    }

    public void ProcessFiles()
    {
        for (int i = 0; i < gamesTextFiles.Length; i++)
        {
            string gamePGN = "";
            string[] lines = gamesTextFiles[i].text.Split('\n');
            for (int j = 0; j < lines.Length; j++)
            {
                if (lines[j].Length > 0)
                {
                    gamePGN += lines[j] + " ";
                }
            }
            processor.ProcessGame(gamePGN);
        }
        textController.UpdateText("All files have been processed");
    }

    public void SortAdded()
    {
        database.AddedSort();
        textController.UpdateText("Database has been sorted by most recently added");
    }
    public void SortScore()
    {
        database.ScoreSort();
        textController.UpdateText("Database has been sorted by quality score");
    }

    public void ShowAllCount()
    {
        textController.UpdateText("Total number of games in database: " + 
            database.GetAllGameCount());
    }
    public void ShowQualityCount()
    {
        textController.UpdateText("Total number of quality games in database: " +
            database.GetQualityGameCount());
    }

    public void DisplayAllGames()
    {
        string result = "There are no games in the database\n";
        if (database.GetAllGameCount() > 0)
        {
            result = "Here are all the games in the database: \n\n";
            for (int i = 0; i < database.GetAllGameCount(); i++)
            {
                result += (i + 1) + ") " + 
                    database.GetAllGames()[i].GetGame().GetQuickFormat() + "\n";
            }
        }
        textController.UpdateText(result);
    }
    public void DisplayQualityGames()
    {
        string result = "There are no quality games in the database\n";
        if (database.GetQualityGameCount() > 0)
        {
            result = "Here are all the quality games in the database: \n\n";
            for (int i = 0; i < database.GetQualityGameCount(); i++)
            {
                result += (i + 1) + ") " +
                    database.GetQualityGames()[i].GetGame().GetQuickFormat() + "\n";
            }
        }
        textController.UpdateText(result);
    }

    public void ChooseAllGame(int index)
    {
        RunGame(database.LoadGame(index, false));
    }
    public void ChooseQualityGame(int index)
    {
        RunGame(database.LoadGame(index, true));
    }

    private void RunGame(Game game)
    {
        if (isReady)
        {
            isReady = false;
            Debug.Log("created iterator");
            iterator = new GameIterator(game);
            textController.UpdateText(game.GetFormattedGameInfo());
        }
    }

    public void NextMove()
    {
        if (isReady) return;
        iterator.NextMove();
        textController.UpdateText(iterator.GetCurrentMove().GetFormattedMove());
        MoveReaderController.Instance.PlayMove(iterator.GetCurrentMove().GetMove());
        if (iterator.GetCurrentMove().GetVariations().Count > 0)
        {
            ShowVariations();
        }
    }
    public void PreviousMove()
    {
        if (isReady) return;
        iterator.PreviousMove();
        textController.UpdateText(iterator.GetCurrentMove().GetFormattedMove());
        BoardManager.Instance.UndoMove();
        if (iterator.GetCurrentMove().GetVariations().Count > 0)
        {
            ShowVariations();
        }
    }
    public void ReturnMove()
    {
        if (isReady) return;
        int returnCount = iterator.ReturnLine();
        textController.UpdateText(iterator.GetCurrentMove().GetFormattedMove());
        for (int i = 0; i < returnCount; i++)
        {
            BoardManager.Instance.UndoMove();
        }
        if (iterator.GetCurrentMove().GetVariations().Count > 0)
        {
            ShowVariations();
        }
    }
    private void ShowVariations()
    {

    }

    public void QuitGame()
    {
        if (isReady) return;
        textController.UpdateText(" ");
        BoardManager.Instance.ResetBoard();
        isReady = true;
    }
}
