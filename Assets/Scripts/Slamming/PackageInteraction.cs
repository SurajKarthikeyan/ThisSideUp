using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackageInteraction : MonoBehaviour
{
    [Header("Package-Actor Interaction Settings")]
    [Tooltip("How many packages this object can hold on to")]
    [SerializeField] public int numHoldablePackages = 1;

    [HideInInspector] public Dictionary<int, GameObject> currentPackages = new Dictionary<int, GameObject>();

    [HideInInspector] public GameObject package;
    [HideInInspector] public Package packageScript;

    // Start is called before the first frame update
    void Start()
    {
        if (package) { packageScript = package.GetComponent<Package>(); }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReceivePackage(Package p)
    {
        //package = null;
        packageScript = p;
        packageScript.currentInteractor = this;
        //currentPackages.Add(package.GetInstanceID(), package);
    }

    public void GivePackage(PackageInteraction pi, Package p)
    {
        packageScript = null;
        //currentPackages.Remove(package.GetInstanceID());
        pi.ReceivePackage(p);

    }
}
