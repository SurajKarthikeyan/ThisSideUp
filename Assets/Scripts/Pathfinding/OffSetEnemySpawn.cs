using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffSetEnemySpawn : MonoBehaviour
{

    public GameObject enemyGO;
    public int numberOfEnemiesToSpawn;
    public const float orgTimeDelay = 2;
    private float timeDelay = orgTimeDelay;
    public Transform playerTransform;

    private void Update()
    {
        CreatePrefab();
    }

    private void CreatePrefab()
    {
        timeDelay -= Time.deltaTime;

        if (timeDelay <= 0 && numberOfEnemiesToSpawn != 0)
        {
            GameObject newEnemy = Instantiate(enemyGO, new Vector2(0, 0), Quaternion.identity);
            newEnemy.GetComponent<UNIT>().target = playerTransform;
            timeDelay = orgTimeDelay;
            numberOfEnemiesToSpawn -= 1;
        }
    }
}
