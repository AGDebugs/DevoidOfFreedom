using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class WeaponDamage : MonoBehaviour
{
    // Performs knockback check
    private Vector3 currpos;
    private Vector3 laterpos;
    
    // Used to deal damage
    private GameObject enemy;
    [Tooltip("Knockback dealt to the enemy. Defaults set in inspector.")]
    [SerializeField] private int KnockbackValue = 1;
    [Tooltip("Damage dealt to the enemy. Defaults set in inspector.")]
    [SerializeField] private int damage;
    [Tooltip("Decides whether it's a melee weapon or a bullet. Defaults set in inspector.")]
    [SerializeField] private bool bullet = false;

    void Start()
    {
        if (bullet) // Fires the bullet if it's a bullet and then destroys it later to save memory
        {
            Invoke("lifetime", 3);
            GetComponent<Rigidbody>().AddForce(transform.forward*20, ForceMode.Impulse);
        }
            
    }
    void lifetime()
    {
        Destroy(this.gameObject);
    }
    
    // Applies knockback and damages the enemy
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "enemy")
        {
            currpos = transform.position;
            enemy = other.gameObject;
            Invoke("applyknockback", 0.05f);
        }
        
    }

    // Detects how far the knockback will be and in which direction, then damages them
    void applyknockback()
    {
        laterpos = transform.position;
        enemy.GetComponent<Rigidbody>().AddForce((laterpos - currpos)*KnockbackValue, ForceMode.Impulse);
        enemy.GetComponent<EnemyAI>().takedamage(KnockbackValue, damage, false);
        enemy = null;
        if(bullet)
            Destroy(this.gameObject);
    }
}
