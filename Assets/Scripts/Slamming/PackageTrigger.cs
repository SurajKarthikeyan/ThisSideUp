using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackageTrigger : MonoBehaviour
{
    private Package package;
    private PackageStatus status;

    // Start is called before the first frame update
    void Start()
    {
        package = GetComponentInParent<Package>();
        status = package.packageStatus;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var enemy = collision.GetComponent<EnemyMovement>();
        // If the entity in the trigger is an enemy
        if (enemy)
        {
            // If our package is currently zapped, stun the enemy
            if (status.IsZapped())
            {
                enemy.StartCoroutine(enemy.Stun(package.zapTime));
            }
        }
    }
}
