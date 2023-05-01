using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameButtonController : MonoBehaviour
{
    public int index;
    public bool quality;
    public string gameText;
    public TextMeshPro textMeshPro;

    public void ClickGame()
    {
        if (!DatabaseController.Instance.GetHasStarted())
        {
            BoardManager.Instance.ResetBoard();
            if (quality) DatabaseController.Instance.ChooseQualityGame(index);
            else DatabaseController.Instance.ChooseAllGame(index);
        }
    }
    public void InitializeMoveButton(int index, bool quality, string game)
    {
        this.index = index;
        this.quality = quality;
        gameText = game;
        textMeshPro.text = gameText;
    }
}
