using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*using UnityEngine.InputSystem;


public class PlayerMovement : MonoBehaviour
{
    public InputAction LRMovement;
    public InputAction jump = new InputAction(binding: "<Keyboard>/space");
    public Transform groundCheck;
    public LayerMask groundObjects;
    public float movementSpeed = 5.5f;
    public Rigidbody rb;
    Vector2 moveDirection = Vector2.zero;

    private bool isGrounded = true;
    private bool isJumping = false;

    [SerializeField] private float groundCheckRadius = 1.0f;

    [Header("Movement Settings")]
    [Tooltip("Amount of force to jump upwards with")]
    [SerializeField] private float jumpForce = 25.0f;

    private void OnEnable()
    {
        LRMovement.Enable();
        jump.Enable();
    }
    private void OnDisable()
    {
        LRMovement.Disable();
        jump.Disable();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isGrounded && collision.gameObject.layer == 7) { print("Grounding"); isGrounded = true; }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        
}

    private void Update()
    {
        moveDirection.x = LRMovement.ReadValue<Vector2>().x;
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(moveDirection.x * movementSpeed, moveDirection.y * movementSpeed);
        if (isGrounded)
        {
            if (jump.IsPressed())
            {
                print("Starting jump");
                Jump();
            } 
        }
    }
    
    private void Jump()
    {
        rb.AddForce(new Vector2(0f, jumpForce));
        isGrounded = false;
    }


}*/
