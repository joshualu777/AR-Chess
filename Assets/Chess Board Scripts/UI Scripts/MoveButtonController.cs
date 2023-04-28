using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoveButtonController : MonoBehaviour
{
    public int index;
    public string moveText;
    public TextMeshPro textMeshPro;

    public void ClickBranch()
    {
        if (index == -1) return;
        DatabaseController.Instance.VariationClicked(index);
    }
    public void InitializeMoveButton(int index, string move)
    {
        this.index = index;
        moveText = move;
        textMeshPro.text = moveText;
    }
}
