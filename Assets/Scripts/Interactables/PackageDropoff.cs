using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackageDropoff : MonoBehaviour
{
    [Header("Dropoff Settings")]
    [Tooltip("Box size in which package will be considered delivered")]
    [SerializeField] private float boxSize = 5.0f;
    [Tooltip("Checkmark prefab")]
    [SerializeField] GameObject checkmark;

    private PackageInteraction pi;
    //private Rigidbody2D rb;

    private void Awake()
    {
        pi = GetComponent<PackageInteraction>();
        //rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 9)
        {
            // If our collision is a package, tally its points and delete it
            var package = collision.GetComponent<Package>();
            var packageParent = collision.GetComponentInParent<Package>();
            if (package)
            {
                ScoreManager.S.TallyPoints((int)package.packageStatus.damage);
                //pi.ReceivePackage(package);
                package.currentInteractor.GivePackage(pi, package);
                collision.gameObject.SetActive(false);
                Destroy(collision.gameObject);
                Object.Destroy(this.gameObject);
            }
            else if (packageParent)
            {
                ScoreManager.S.TallyPoints((int)packageParent.packageStatus.damage);
                //pi.ReceivePackage(package);
                packageParent.currentInteractor.GivePackage(pi, packageParent);
                packageParent.gameObject.SetActive(false);
                Destroy(packageParent.gameObject.transform.parent);
                Object.Destroy(this.gameObject);
            }
            Instantiate(checkmark, transform.position, Quaternion.identity);
        }
    }

    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.green;
        //Gizmos.DrawCube(transform.position, new Vector3(boxSize, boxSize, boxSize));
    }


}
