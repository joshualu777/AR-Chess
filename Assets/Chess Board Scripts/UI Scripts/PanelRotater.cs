using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelRotater : MonoBehaviour
{
    public Camera cam;

    void Update()
    {
        transform.LookAt(cam.transform);
        transform.rotation = transform.rotation * Quaternion.Euler(0, 180, 0);
    }
}
