using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticTruck : MonoBehaviour
{
    [Header("Package Settings")]
    [Tooltip("Amount of packages carried in the truck for this level")]
    [SerializeField] private int numPackages = 10;
    [Tooltip("Amount of starting damage on the packages")]
    [SerializeField] private int startingDamage = 0;
    [Tooltip("Package object to spawn")]
    [SerializeField] private GameObject package;

    [Header("Truck Settings")]
    [Tooltip("How close the player has to be to grab a package")]
    [SerializeField] private float playerProximityRadius = 10.0f;

    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GivePackageToPlayer()
    {
        // Before player grabs this package, apply our starting damage to it
        var playerPI = player.GetComponent<PackageInteraction>();
        if (player && !playerPI.packageScript && numPackages > 0)
        {
            if (!package) { Debug.LogWarning("No package is assigned in the Static Truck GameObject"); }
            var newPackage = Instantiate(package);
            newPackage.transform.position = player.transform.position;
            newPackage.GetComponent<Package>().health -= startingDamage;
            newPackage.GetComponent<Movable>().SetDragOrigin(player.transform);
            playerPI.ReceivePackage(newPackage.GetComponent<Package>());
            numPackages--;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            player.GetComponent<CharacterController2D>().isInTruckRange = true;
            print("player in range of truck");
        }
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            player.GetComponent<CharacterController2D>().isInTruckRange = false;
            print("player no longer in range of truck");
        }
        
    }

    public int GetNumPackages()
    {
        return numPackages;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, playerProximityRadius);
    }
}
