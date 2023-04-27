using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreSort : IComparer<GameData>
{
    public int Compare(GameData game1, GameData game2)
    {
        return game2.GetScore() - game1.GetScore();
    }
}
