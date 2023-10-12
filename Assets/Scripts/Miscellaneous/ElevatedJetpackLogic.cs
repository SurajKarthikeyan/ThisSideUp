using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ElevatedJetpackLogic : MonoBehaviour
{
    private TilemapCollider2D tmCollider;

    private void Awake()
    {
        tmCollider = GetComponent<TilemapCollider2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var enemyCheck = collision.GetComponent<EnemyMovement>();
        if (enemyCheck)
        {
            enemyCheck.BeginJetpackThrust();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var enemyCheck = collision.GetComponent<EnemyMovement>();
        if (enemyCheck)
        {
            enemyCheck.EndJetpackThrust();
        }
    }
}
