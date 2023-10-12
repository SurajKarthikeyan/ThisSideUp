using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardApplicator : MonoBehaviour
{
    [Header("Hazard Settings")]
    [Tooltip("Which hazard to apply")]
    [SerializeField] private PackageStatus.Status status;
    [Tooltip("Hazard volume size")]
    [SerializeField] private float volumeSize = 8.0f;

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
        if (collision.gameObject.layer == 9)
        {
            Package package = collision.gameObject.GetComponent<Package>();
            Package packageParent = collision.GetComponentInParent<Package>();
            if (package)
            {
                StartCoroutine(package.packageStatus.ApplyStatus(status));
            }
            else if (packageParent)
            {
                StartCoroutine(packageParent.packageStatus.ApplyStatus(status));
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 9)
        {
            Package package = other.GetComponent<Package>();
            if (package)
            {
                package.packageStatus.ApplyStatus(status);
            }
        }
    }

    private void OnDrawGizmos()
    {
        switch (status)
        {
            case PackageStatus.Status.Burnt:
                Gizmos.color = Color.red;
                break;
            case PackageStatus.Status.Waterlogged:
                Gizmos.color = Color.cyan;
                break;
            case PackageStatus.Status.Zapped:
                Gizmos.color = Color.blue;
                break;
            default:
                print("No hazard type assigned, gizmo not drawn");
                break;
        }

        Gizmos.DrawWireCube(transform.position, new Vector3(volumeSize, volumeSize, volumeSize));
    }
}
