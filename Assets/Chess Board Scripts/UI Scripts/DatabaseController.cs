using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DatabaseController : MonoBehaviour
{
    public TextAsset[] gamesTextFiles;
    public TextMeshPro annotationText;

    public GameObject variationPanelPrefab;
    public Transform panelLocation;
    public GameObject variationButtonPrefab;

    public GameObject gamePanel;
    public GameObject gameButtonPrefab;

    private Database database;
    private PGNProcessor processor;
    private bool isReady;

    private GameIterator iterator;
    private TextController textController;

    private GameObject currentPanel;

    private static DatabaseController _instance;

    public static DatabaseController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<DatabaseController>();
            }

            return _instance;
        }
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

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
            result = "All games displayed, please select the one you would like to study";
            for (int i = 0; i < database.GetAllGameCount(); i++)
            {
                gamePanel.transform.GetChild(i).GetComponent<GameButtonController>().InitializeMoveButton(i, false,
                    database.GetAllGames()[i].GetGame().GetQuickFormat());
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
        if (!isReady) return;
        RunGame(database.LoadGame(index, false));
    }
    public void ChooseQualityGame(int index)
    {
        if (!isReady) return;
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
        Move startMove = iterator.GetCurrentMove();
        Move endMove = iterator.NextMove();
        textController.UpdateText(iterator.GetCurrentMove().GetFormattedMove());
        if (!startMove.Equals(endMove))
        {
            MoveReaderController.Instance.PlayMove(iterator.GetCurrentMove().GetMove());
        }
        CheckVariations();
    }
    public void PreviousMove()
    {
        if (isReady) return;
        iterator.PreviousMove();
        textController.UpdateText(iterator.GetCurrentMove().GetFormattedMove());
        BoardManager.Instance.UndoMove();
        CheckVariations();
    }
    public void ReturnMove()
    {
        if (isReady) return;
        Move startMove = iterator.GetCurrentMove();
        Move endMove = iterator.ReturnLine();
        int steps = CalculateReturn(endMove, startMove);
        textController.UpdateText(iterator.GetCurrentMove().GetFormattedMove());
        for (int i = 0; i < steps; i++)
        {
            BoardManager.Instance.UndoMove();
        }
        CheckVariations();
    }
    private int CalculateReturn(Move returnedPosition, Move startPosition)
    {
        int steps = (startPosition.GetMoveNumber() - returnedPosition.GetMoveNumber()) * 2;
        if (returnedPosition.GetIsWhite() && !startPosition.GetIsWhite())
        {
            steps++;
        }
        else if (!returnedPosition.GetIsWhite() && startPosition.GetIsWhite())
        {
            steps--;
        }
        return steps;
    }
    private void CheckVariations()
    {
        if (currentPanel != null)
        {
            Destroy(currentPanel);
        }
        if (iterator.GetCurrentMove().GetNextMove() != null &&
            iterator.GetCurrentMove().GetNextMove().GetVariations().Count > 0)
        {
            ShowVariations();
        }
    }
    private void ShowVariations()
    {
        Debug.Log("showing variations");
        currentPanel = Instantiate(variationPanelPrefab);
        currentPanel.transform.position = panelLocation.position;
        currentPanel.transform.rotation = panelLocation.rotation;
        for (int i = 0; i < iterator.GetCurrentMove().GetNextMove().GetVariations().Count; i++)
        {
            GameObject button = Instantiate(variationButtonPrefab);
            button.GetComponent<MoveButtonController>().InitializeMoveButton(i,
                iterator.GetCurrentMove().GetNextMove().GetVariations()[i].SimpleMove());
            button.transform.SetParent(currentPanel.transform);
        }
    }
    public void VariationClicked(int index)
    {
        if (isReady) return;
        iterator.NextMove(index);
        textController.UpdateText(iterator.GetCurrentMove().GetFormattedMove());
        MoveReaderController.Instance.PlayMove(iterator.GetCurrentMove().GetMove());
        CheckVariations();
    }

    public void QuitGame()
    {
        if (isReady) return;
        textController.UpdateText(" ");
        Destroy(currentPanel);
        BoardManager.Instance.ResetBoard();
        isReady = true;
    }
}
