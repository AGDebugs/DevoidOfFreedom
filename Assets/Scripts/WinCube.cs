using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinCube : MonoBehaviour
{
    [SerializeField] private GameObject wingame;
    
    // Touch to win.
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            wingame.SetActive(true);
            Application.targetFrameRate = 0;
        }
    }
}
