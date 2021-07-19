using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHPUI : MonoBehaviour
{
    private GameObject enemy;
    private Vector3 pos;
    void Start()
    {
        enemy = transform.parent.parent.gameObject;
    }

    // This keeps the HP above the enemies relative to the camera
    void Update()
    {
        pos = Camera.main.WorldToScreenPoint(enemy.transform.position);
        transform.position = new Vector3(pos.x,pos.y+35, pos.z);
    }
}
