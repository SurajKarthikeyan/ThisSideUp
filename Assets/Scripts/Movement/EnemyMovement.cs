using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("How fast this enemy moves")]
    [SerializeField] private float enemySpeed = 2.0f;
    [Tooltip("Max enemy speed")]
    [SerializeField] private float maxSpeed = 300.0f;
    [SerializeField] private bool m_FacingRight = true;

    [Header("Objectives")]
    [Tooltip("Game object that this enemy will track")]
    [SerializeField] private GameObject objective;
    [Tooltip("Proximity to objective at which point this enemy is considered to have arrived")]
    [SerializeField] private float proximityThreshold = 1.0f;

    [Header("Boid Settings")]
    [Tooltip("Min distance to keep from other enemies")]
    [SerializeField] private float separation = 2.0f;
    [Tooltip("Amount of repuslive force used when too close to another enemy")]
    [SerializeField] private float repulsiveForce = 85.0f;
    [Tooltip("After stealing a package, wait this long before despawning")]
    [SerializeField] private float secondsBeforeDespawn = 3.5f;

    [Header("Package Interaction Settings")]
    [Tooltip("Offset on the y-axis for where this enemy will carry any packages it steals")]
    [SerializeField] private float packageCarryOffset = 4f;

    [Header("Jetpack Settings")]
    [Tooltip("Offset for particle effect after sprite is flipped")]
    [SerializeField] private float fireVFXFlipOffsetAmount = 5.0f;

    private Rigidbody2D rigidbody;
    private PackageInteraction pi;
    private bool hasReachedObjective = false;
    private bool isStunned = false;
    private UNIT unit;
    private SpriteRenderer sr;
    private Vector3 previousMovVector;
    private Transform fireVFX;

    // Start is called before the first frame update
    void Start()
    {
        objective = GameObject.FindGameObjectWithTag("Player");
        rigidbody = GetComponent<Rigidbody2D>();
        unit = GetComponent<UNIT>();
        pi = GetComponent<PackageInteraction>();
        sr = GetComponent<SpriteRenderer>();
        previousMovVector = transform.position;
        fireVFX = transform.GetChild(0);
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void Update()
    {
        // If we have stolen the package, stop moving for awhile, then despawn
        if (pi.packageScript && !isStunned)
        {
            isStunned = true;
            Object.Destroy(this.gameObject, secondsBeforeDespawn);
        }
        
    }



    private bool IsAtObjective()
    {
        if (Vector2.Distance(objective.transform.position, transform.position) < proximityThreshold)
        {
            hasReachedObjective = true;
            return true;
        }
        return false;
    }

    private void HandleMovement()
    {
        if (!isStunned)
        {
            // When we arrive at the player
            if (IsAtObjective())
            {
                StealPackageFromPlayer();
            }
            // If still moving toward player
            else if (objective)
            {
                previousMovVector = transform.position;
                Vector3 movVec = Vector3.MoveTowards(transform.position, unit.curWaypoint, maxSpeed);
                // If facing right but moving left, flip
                if (movVec.x < previousMovVector.x && m_FacingRight) { Flip(); }
                // If facing left but moving right, flip
                else if (movVec.x > previousMovVector.x && !m_FacingRight) { Flip(); }
                transform.position = movVec;
            }
        }
    }

    private void StealPackageFromPlayer()
    {
        PackageInteraction objectiveInteractor = objective.GetComponent<PackageInteraction>();
        if (objectiveInteractor && objectiveInteractor.packageScript) 
        {
            print("Ima borrow that package rq");
            // Force player to release package
            objectiveInteractor.packageScript.movable.ReleasePackage();
            // Steal package from player
            objectiveInteractor.GivePackage(pi, objectiveInteractor.packageScript);
            // Remove hitbox for package so the player can no longer interact with it
            //pi.packageScript.GetComponent<BoxCollider2D>().enabled = false;
            pi.packageScript.movable.enabled = false;
            pi.packageScript.GetComponent<Rigidbody2D>().isKinematic = true;
            // Child package to enemy so they "carry" it around with them
            pi.packageScript.transform.position = new Vector3(transform.position.x, transform.position.y + packageCarryOffset, transform.position.z);
            pi.packageScript.transform.SetParent(transform);
        }
    }

    public IEnumerator Stun(float seconds)
    {
        isStunned = true;
        yield return new WaitForSeconds(seconds);
        isStunned = false;
    }

    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;
        sr.flipX = !sr.flipX;
        if (!m_FacingRight)
        {
            fireVFX.localPosition = new Vector3(fireVFX.localPosition.x + fireVFXFlipOffsetAmount, fireVFX.localPosition.y, fireVFX.localPosition.z);
        }
        else
        {
            fireVFX.localPosition = new Vector3(fireVFX.localPosition.x - fireVFXFlipOffsetAmount, fireVFX.localPosition.y, fireVFX.localPosition.z);
        }
        
    }

    public void BeginJetpackThrust()
    {
        fireVFX.gameObject.SetActive(true);
    }

    public void EndJetpackThrust()
    {
        fireVFX.gameObject.SetActive(false);
    }

    /*private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, separation);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, proximityThreshold);
    }*/


}
