using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareButtonController : MonoBehaviour
{
    public int id;

    public void SquareClicked()
    {
        Debug.Log("clicked");
        BoardManager.Instance.UpdateSelection(id);
    }
}
