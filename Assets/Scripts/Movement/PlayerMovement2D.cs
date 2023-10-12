using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement2D : MonoBehaviour
{
    public CharacterController2D controller;
    [SerializeField] private float runSpeed = 40.0f;

    private float horizontalMove = 0f;
    private bool isJumping = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        controller.Move(horizontalMove, false, isJumping);
        isJumping = false;
    }

    // Update is called once per frame
    void Update()
    {
        //controller.Move();
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

        if (Input.GetButtonDown("Jump"))
        {
            isJumping = true;
        }
    }
}
