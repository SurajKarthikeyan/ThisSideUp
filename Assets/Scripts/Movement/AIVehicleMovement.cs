using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIVehicleMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Start point of the car")]
    [SerializeField] private Transform startPoint;
    [Tooltip("End point of the car")]
    [SerializeField] private Transform endPoint;
    [Tooltip("Car speed")]
    [SerializeField] private float speed = 8.0f;
    [Tooltip("Max distance per FixedUpdate call")]
    [SerializeField] private float maxDistance = 50.0f;
    [SerializeField] private bool m_FacingRight = true;

    [Header("Player Interaction Settings")]
    [Tooltip("Amount of seconds to stun the player for on hit")]
    [SerializeField] private float secondsToStun = 3.0f;

    private Transform currentObjective;
    private float distanceThreshold = 15.0f;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;

    private void Awake()
    {
        transform.position = startPoint.position;
        currentObjective = startPoint;
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        anim.SetTrigger("isDriving");
    }

    private void FixedUpdate()
    {
        rb.AddForce((currentObjective.position - transform.position).normalized * speed * Time.deltaTime);
    }

    // Update is called once per frame
    void Update()
    {
        // Once we reach the other side of the map
        if (Vector3.Distance(transform.position, currentObjective.position) <= distanceThreshold)
        {
            rb.velocity = new Vector2(0f, 0f);
            currentObjective = currentObjective.position == endPoint.position ? startPoint : endPoint;
            Flip();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If we hit a player, stun them
        if (collision.tag == "Player")
        {
            var playerController = collision.GetComponent<CharacterController2D>();
            playerController.StartCoroutine(playerController.Stun(secondsToStun));
        }
    }
    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;
        sr.flipX = !sr.flipX;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (currentObjective != null) { Gizmos.DrawLine(transform.position, currentObjective.position); }
    }
}
