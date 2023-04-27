using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class GameData : IComparable<GameData>
{
    private Game game;
    private int id;
    private int score;

    private bool quality;
    private bool viewed;

    public GameData(Game game, int id, int score)
    {
        this.game = game;
        this.id = id;
        this.score = score;
        quality = false;
        viewed = false;
    }

    public Game GetGame() { return game; }
    public int GetId() { return id; }
    public int GetScore() { return score; }

    public bool GetQuality() { return quality; }
    public void MarkQuality() { quality = true; }
    public void UnmarkQuality() { quality = false; }

    public bool GetViewed() { return viewed; }
    public void MarkViewed() {  viewed = true; }
    public void UnmarkViewed() { viewed = false; }

    public int CompareTo(GameData other)
    {
        return other.GetId() - this.GetId();
    }
}
