using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    private EnemyAI enemyAI;

    void Start()
    {
        enemyAI = transform.parent.GetComponent<EnemyAI>();
    }

    // This is kinda the enemies 'vision', you could say. If you get in here, their life mission is now to hunt you down
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            enemyAI.setPlayer(other.gameObject);
            enemyAI.chasing = true;
            GetComponent<BoxCollider>().enabled = false;
        }
    }
}
