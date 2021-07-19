using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    private GameObject player;
    [Tooltip("Damage dealt by the enemy.")]
    [SerializeField] private int damage;
    
    //Checks if it hit the player and then damages them
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            player = other.gameObject;
            Invoke("damageplayer", 0.1f);
        }
        
    }

    void damageplayer()
    {
        player.GetComponent<Character>().takedamage(damage);
    }
}
