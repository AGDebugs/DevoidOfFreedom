using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walls : MonoBehaviour
{
    // You smack enemies into them, and this function checks for a smack and deals damage. Static 30 for now.
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "enemy")
        {
            if (other.gameObject.GetComponent<EnemyAI>().knockedback == true)
            {
                other.gameObject.GetComponent<EnemyAI>().takedamage(0, 30, true);
            }
        }
    }
}
