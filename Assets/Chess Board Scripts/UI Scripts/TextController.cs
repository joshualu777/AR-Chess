using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextController : MonoBehaviour
{
    private TextMeshPro textMeshPro;


    void Start()
    {
        textMeshPro = GetComponent<TextMeshPro>();
        textMeshPro.text = " ";
    }

    public void UpdateText(string text)
    {
        textMeshPro.text = text;
    }
}
