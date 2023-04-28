using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game
{
    private readonly string[] KEY_INFO = { "Event", "Date", "TimeControl", "Annotator", 
        "White", "WhiteElo", "Black", "BlackElo", "Result" };
    private readonly int REQUIRED_INFO_START = 4;

    private List<string> gameInformation;
    private Dictionary<string, string> keyInfo;
    private int code;

    private Move firstMove;

    public Game(List<string> info)
    {
        gameInformation = info;
        FindKeyInfo();
        GeneratedCode();
        firstMove = null;
    }

    public int GetCode() { return code; }
    public void GeneratedCode()
    {
        const int MOD = 1_000_000_007;
        const int PRIME = 31;

        int hash = 0;
        int power = 1;
        for (int i = 0; i < KEY_INFO.Length; i++)
        {
            string cur = keyInfo[KEY_INFO[i]];
            if (cur == null) continue;
            
            for (int j = 0; j < cur.Length; j++)
            {
                hash = (hash + (cur[j] - 'a' + 1) * power) % MOD;
                power = (power * PRIME) % MOD;
            }
        }
        this.code = hash;
    }

    public Move GetFirstMove() { return firstMove; }
    public void SetFirstMove(Move firstMove) { this.firstMove = firstMove; }

    public string GetFormattedGameInfo()
    {
        string result = "";
        for (int i = 0; i < REQUIRED_INFO_START; i++)
        {
            if (keyInfo[KEY_INFO[i]] != null)
            {
                result += KEY_INFO[i] + ": " + keyInfo[KEY_INFO[i]] + "\n";
            }
        }
        result += "\n" + GetQuickFormat() + "\n";
        result += "Result: " + keyInfo["Result"];
        return result;
    }
    public string GetQuickFormat()
    {
        string playerInfo = "White: " + keyInfo["White"] + " (" +
            keyInfo["WhiteElo"] + ") vs\n" +
            "Black: " + keyInfo["Black"] + " (" +
            keyInfo["BlackElo"] + ")";
        return playerInfo;
    }

    private void FindKeyInfo()
    {
        InitializeMap();
        for (int i = 0; i < gameInformation.Count; i++)
        {
            string curInfo = gameInformation[i];
            int firstSpaceIndex = curInfo.IndexOf(' ');
            string first = curInfo.Substring(0, firstSpaceIndex);
            string second = curInfo.Substring(firstSpaceIndex + 1);
            Debug.Log(first + ": " + second);
            if (keyInfo.ContainsKey(first))
            {
                keyInfo[first] = second;
            }
        }
        if (!char.IsDigit(keyInfo["WhiteElo"][0]))
        {
            keyInfo["WhiteElo"] = "???";
        }
        if (!char.IsDigit(keyInfo["BlackElo"][0]))
        {
            keyInfo["BlackElo"] = "???";
        }
    }
    private void InitializeMap()
    {
        keyInfo = new Dictionary<string, string>();
        for (int i = 0; i < KEY_INFO.Length; i++)
        {
            keyInfo[KEY_INFO[i]] = " ";
        }
    }
}
