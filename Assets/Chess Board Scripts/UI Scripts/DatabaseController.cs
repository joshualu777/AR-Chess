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
    
    void Start()
    {
        database = new Database();
        processor = new PGNProcessor(database);
        isReady = true;
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
    }

    public void SortAdded()
    {
        database.AddedSort();
    }
    public void SortScore()
    {
        database.ScoreSort();
    }

    public void ChooseAllGame(int index)
    {
        RunGame(database.LoadGame(index, true));
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
        }
    }

    public void NextMove()
    {
        if (isReady) return;
        iterator.NextMove();
        annotationText.GetComponent<TextController>().UpdateText(iterator.GetCurrentMove().GetFormattedMove());
        MoveReaderController.Instance.PlayMove(iterator.GetCurrentMove().GetMove());
    }
    public void PreviousMove()
    {
        if (isReady) return;
        iterator.PreviousMove();
        BoardManager.Instance.UndoMove();
    }
}
