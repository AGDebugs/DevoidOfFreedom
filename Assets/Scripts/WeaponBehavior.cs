using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBehavior : MonoBehaviour
{
    // How often you can attack and can you attack. 
    [Tooltip("How fast you can attack. Defaults set in inspector.")]
    [SerializeField] private float AttackRate = 1;
    [Tooltip("Don't use this for now. Not Implemented")]
    [SerializeField] private bool AutomaticWeapon = false; // For the rifle, not implemented as yet.
    private bool canfire = true;
    private Collider hitbox;
    private Animator myAnim;
    
    // Checks if it's a gun, and if so, fires a bullet instead.
    [SerializeField] private bool isgun = false;
    private GameObject bullet;
    
    
    void Start()
    {
        myAnim = GetComponent<Animator>();
        hitbox = transform.GetChild(0).GetComponent<Collider>();
        bullet = Resources.Load("Prefabs/Bullet", typeof(GameObject)) as GameObject;
    }
    
    // Keeps your trigger happy fingers in check with fire rate.
    void firerateflag()
    {
        canfire = true;
    }
    public void Fire()
    {
        if (canfire)
        {
           canfire = false; 
           myAnim.Play("Fire");
           if (!AutomaticWeapon)
           {
               Invoke("firerateflag", 100f / AttackRate);
           }

           // Fires a bullet
           if (isgun)
           {
               GameObject bul = Instantiate(bullet, transform.position, Quaternion.identity);
               bul.transform.rotation = transform.rotation;
           }
        }
    }

    // Triggered during animation itself in order to set the frames during which damage can be dealt.
    // Prevents player from being able to simply smoosh themselves on enemies for damage.
    public void attackon()
    {
        hitbox.enabled = true;
    }
    public void attackoff()
    {
        hitbox.enabled = false;
    }

    // Alternate fire animations
    public void Alternatefire()
    {
        if (canfire)
        {
            canfire = false; 
            myAnim.Play("AlternateFire");
            if (!AutomaticWeapon)
            {
                Invoke("firerateflag", 100f / AttackRate);
            }
        }
    }

    // Not used for now, stops the rifle from firing
    void FixedUpdate()
    {
        if (AutomaticWeapon && !canfire)
        {
            if (Input.GetMouseButtonUp(0))
            {
                myAnim.Play("Idle");
                canfire = true;
            }
        }
    }

    
}
