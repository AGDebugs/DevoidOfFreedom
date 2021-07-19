using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour
{
    private Transform player;
    
    // Enemy stats
    [Tooltip("Health value of the Enemy. Default: 100")]
    [SerializeField] private int health = 100;
    private int healthtot;
    private Text healthval;
    private Image healthbar;
    
    // Enemy Movement
    [Tooltip("Indicates detection radius of the Enemy. Default: 60")]
    [SerializeField] private float walkingDistance = 60.0f;
    [Tooltip("Indicates attack range of the Enemy. Default: 20")]
    [SerializeField] private float attackDistance = 20f;
    [HideInInspector] public bool knockedback = false;
    private float smoothTime = 1.0f;
    private Vector3 smoothVelocity = Vector3.zero;
    
    // Enemy Attacking
    private Animator myAnim;
    [HideInInspector] public bool chasing = false;
    private Material damaged;
    private Material original;
    private float distance = 0;
    [Tooltip("Indicates attack speed of the Enemy. Default: 2")]
    [SerializeField] private float attacktime = 2;
    private bool attacking = false;

    private void Start()
    {
        healthtot = health;
        myAnim = GetComponent<Animator>();
        damaged = Resources.Load("Materials/BlueDamaged", typeof(Material)) as Material;
        original = Resources.Load("Materials/Blue", typeof(Material)) as Material;
        healthval = transform.GetChild(3).transform.GetChild(0).transform.GetChild(1).gameObject.GetComponent<Text>();
        healthbar = transform.GetChild(3).transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<Image>();
    }

    // Sets the player so if they take damage outside of detection, they will agro (does not work with bullets atm)
    public void setPlayer(GameObject theplayer)
    {
        player = theplayer.transform;
    }
    
    // Knockback detection to see if they come in contact with a wall for extra damage.
    void notknocked()
    {
        knockedback = false;
    }
    void notdamaged()
    {
        GetComponent<MeshRenderer>().material = original;
        
    }
    void gotdamaged() // changes color to indicate damage
    {
        GetComponent<MeshRenderer>().material = damaged;
        Invoke("notdamaged", 0.4f);
    }
    // Function to calculate health and pwn them if they get below zero. Also detects player if they are sneaky and damage from outside detection.
    public void takedamage(int kbval, int dmg, bool iswall)
    {
        health -= dmg;
        chasing = true;
        if (player == null)
        {
            Debug.Log("here");
            RaycastHit[] hit;
            hit = Physics.SphereCastAll(transform.position, 100, Vector3.forward);
            for(int i=0; i<hit.Length; i++)
            {
                Debug.Log(i+") "+hit[i].collider.gameObject.name);
                if (hit[i].collider.gameObject.tag == "Player")
                    player = hit[i].collider.gameObject.transform;
            }
        }
        healthval.text = health.ToString();
        float diff = (float) health / (float) healthtot;
        gotdamaged();
        healthbar.fillAmount = (float)diff;
        if (kbval > 3)
        {
            knockedback = true;
            Invoke("notknocked", kbval/6);
        }
        if (health <= 0)
        {
            Destroy(this.gameObject); //Can add fancy animations and maybe item drops later on, for now they disappear
        }

        if (iswall)
        {
            chasing = false;
            Invoke("startchase", 1);
        }
    }

    // He's on your tail now!
    void startchase()
    {
        chasing = true;
        
    }
    void stopattack()
    {
        attacking = false;
    }
    
    // Keeps them looking at you and chasing you
    void Update()
    {
        if (chasing)
        {
            if(!attacking)
                transform.LookAt(player);
           distance = Vector3.Distance(transform.position, player.position);
           if(distance < walkingDistance && distance > attackDistance)
           {
               transform.position = Vector3.SmoothDamp(transform.position, player.position, ref smoothVelocity, smoothTime);
           }
           else if (distance <= attackDistance)
           {
               if (attacking == false)
               {
                   attacking = true;
                   
                   myAnim.Play("Attack");
                   Invoke("stopattack", attacktime);
               }
               
           }
        }
        
    }
}
