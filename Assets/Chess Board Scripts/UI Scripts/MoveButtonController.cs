using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoveButtonController : MonoBehaviour
{
    public int index;
    public string moveText;
    public TextMeshPro moveTextPro;

    public void ClickBranch()
    {
        DatabaseController.Instance.VariationClicked(index);
    }
    public void InitializeMoveButton(int index, string move)
    {
        this.index = index;
        moveText = move;
        moveTextPro.text = moveText;
    }
}
