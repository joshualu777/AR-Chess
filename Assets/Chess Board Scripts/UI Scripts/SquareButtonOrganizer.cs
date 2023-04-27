using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareButtonOrganizer : MonoBehaviour
{
    public GameObject squareButton;

    void Start()
    {
        for (int i = 0; i < 64; i++)
        {
            GameObject button = Instantiate(squareButton, this.transform);
            button.GetComponent<SquareButtonController>().id = i;
        }
    }
}
