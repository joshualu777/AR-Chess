using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class Database
{
    private const int QUALITY_THRESHOLD = 5000;
    private const int MOVE_MULTIPLIER = 3000;

    private List<GameData> allGames;
    private HashSet<int> copies;
    private int gameCount;

    private List<GameData> qualityGames;
    private int qualityGameCount;

    public Database()
    {
        allGames = new List<GameData>();
        copies = new HashSet<int>();
        qualityGames = new List<GameData>();
        gameCount = 0;
        qualityGameCount = 0;
    }

    public List<GameData> GetGames() { return allGames; }
    public int GetGameCount() { return gameCount; }
    public  List<GameData> GetQualityGames() { return qualityGames; }
    public int GetQualityGameCount() { return qualityGameCount; }

    public void AddedSort()
    {
        allGames.Sort();
        qualityGames.Sort();
    }
    public void ScoreSort()
    {
        ScoreSort sort = new ScoreSort();
        allGames.Sort(sort);
        qualityGames.Sort(sort);
    }

    public string ShowGames()
    {
        if (allGames.Count == 0)
        {
            return "There are no games in the database\n";
        }
        string result = "Here are all the games in the database: \n\n";
        for (int i = 0; i < allGames.Count; i++)
        {
            result += (i + 1) + ") " + allGames[i].GetGame().GetQuickFormat() + "\n";
        }
        return result;
    }
    public string ShowQualityGames()
    {
        if (qualityGames.Count == 0)
        {
            return "There are no quality games in the database\n";
        }
        string result = "Here are the quality games in the database: \n\n";
        for (int i = 0; i < qualityGames.Count; i++)
        {
            result += (i + 1) + ") " + qualityGames[i].GetGame().GetQuickFormat() + "\n";
            result += "\tScore: " + qualityGames[i].GetScore() + "\n";
        }
        return result;
    }

    public Game LoadGame(int option, bool quality)
    {
        if (quality) return qualityGames[option].GetGame();
        else return allGames[option].GetGame();
    }
    public void AddGame(Game game)
    {
        if (copies.Contains(game.GetCode())) return;
        allGames.Add(new GameData(game, gameCount, CalculateScore(game)));
        if (allGames[gameCount].GetScore() >= QUALITY_THRESHOLD)
        {
            allGames[gameCount].MarkQuality();
            qualityGames.Add(allGames[gameCount]);
            qualityGameCount++;
        }
        copies.Add(game.GetCode());
        gameCount++;
    }

    private int CalculateScore(Game game)
    {
        int numMainMoves = CountMainMoves(game);
        GameIterator iter = new GameIterator(game);
        DataPair count = CountAll(iter, game);

        return count.text + MOVE_MULTIPLIER * count.moves / numMainMoves;
    }
    private int CountMainMoves(Game game)
    {
        GameIterator iter = new GameIterator(game);
        iter.NextMove();
        int cnt = 0;
        bool exit = false;
        while (!exit)
        {
            iter.NextMove();
            cnt++;
            if (iter.GetCurrentMove().GetNextMove() == null)
            {
                exit = true;
            }
        }
        Debug.Log(cnt);
        return cnt;
    }
    private DataPair CountAll(GameIterator iter, Game game)
    {
        DataPair info = new DataPair();
        if (iter.GetCurrentMove() == null) { return info; }
        if (iter.GetCurrentMove().GetMoveNumber() != 0)
        {
            info.text += iter.GetCurrentMove().GetPreText().Length +
                iter.GetCurrentMove().GetPostText().Length;
            info.moves++;
        }

        iter.NextMove();
        DataPair infoMain;
        if (iter.GetCurrentMove().GetNextMove() == null)
        {
            infoMain = new DataPair();
        }
        else
        {
            infoMain = CountAll(iter, game);
        }
        info.text += infoMain.text;
        info.moves += infoMain.moves;
        iter.PreviousMove();

        int varLength = iter.GetCurrentMove().GetNextMove().GetVariations().Count;
        for (int i = 0; i < varLength; i++)
        {
            iter.NextMove(i);
            DataPair infoVar;
            if (iter.GetCurrentMove().GetNextMove() == null)
            {
                infoVar = new DataPair();
            }
            else
            {
                infoVar= CountAll(iter, game);
            }
            info.text += infoVar.text;
            info.moves += infoVar.moves;
            iter.PreviousMove();
        }
        return info;
    }
}
