using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Scene used to transition from light to dark for second level

public class ChangeScene : MonoBehaviour
{
    [SerializeField] private GameObject door;
    [SerializeField] private Light scenelighting;
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            GetComponent<BoxCollider>().enabled = false; // to ensure it isn't triggered multiple times
            door.SetActive(true);
            scenelighting.GetComponent<Animator>().enabled = true;
        }
    }
}
