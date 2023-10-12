using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

// From https://github.com/Brackeys/2D-Character-Controller
public class CharacterController2D : MonoBehaviour
{
	[SerializeField] private float m_JumpForce = 400f;                          // Amount of force added when the player jumps.
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
	[SerializeField] private bool m_AirControl = false;                         // Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
	[SerializeField] private List<Transform> m_GroundChecks;                           // A position marking where to check if the player is grounded.
	[SerializeField] private List<Transform> m_CeilingChecks;                          // A position marking where to check for ceilings

	const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
	private bool m_Grounded;            // Whether or not the player is grounded.
	const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
	private Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;  // For determining which way the player is currently facing.
	private Vector3 m_Velocity = Vector3.zero;

	[HideInInspector] public bool isInTruckRange = false;
	private StaticTruck truck;
	private SpriteRenderer sr;
	private Animator anim;
	private bool isStunned = false;
	[HideInInspector] public bool isDisabled { get; set; }

	[Header("Effects")]
	[SerializeField, Tooltip("The stun particle effect")]
	private ParticleSystem stunParticles;
	[SerializeField, Tooltip("The Cinemachine Impulse source to trigger when stunned")]
	private Cinemachine.CinemachineImpulseSource impulseSource;

	public UnityEvent OnLandEvent;

	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
		sr = GetComponent<SpriteRenderer>();
		anim = GetComponent<Animator>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();
	}

	private void Start()
	{
		truck = GameObject.FindGameObjectWithTag("Truck").GetComponent<StaticTruck>();
	}

	private void FixedUpdate()
	{
		bool wasGrounded = m_Grounded;
		m_Grounded = false;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] colliders;
		colliders = Physics2D.OverlapCircleAll(m_GroundChecks[0].position, k_GroundedRadius, m_WhatIsGround);   // I have to do this otherwise I get an error, though this line is unnecessary 
		for (int i = 0; i < m_GroundChecks.Count; i++)
		{
			colliders = Physics2D.OverlapCircleAll(m_GroundChecks[i].position, k_GroundedRadius, m_WhatIsGround);
			if (colliders.Length > 0) { break; }
		}

		//Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				m_Grounded = true;
				if (!wasGrounded)
					OnLandEvent.Invoke();
			}
		}
	}

	private void Update()
	{
		if (isInTruckRange && Input.GetButtonDown("Fire1"))
		{
			truck.GivePackageToPlayer();
		}
	}

	public void Move(float move, bool crouch, bool jump)
	{
		//only control the player if grounded or airControl is turned on
		if (!isStunned && !isDisabled)
		{
			if (m_Grounded || m_AirControl)
			{
				// Move the character by finding the target velocity
				Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
				// And then smoothing it out and applying it to the character
				m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

				// If the input is moving the player right and the player is facing left...
				if (move > 0 && !m_FacingRight)
				{
					// ... flip the player.
					Flip();
				}
				// Otherwise if the input is moving the player left and the player is facing right...
				else if (move < 0 && m_FacingRight)
				{
					// ... flip the player.
					Flip();
				}
				anim.SetTrigger("isWalking");
			}
			// If the player should jump...
			if (m_Grounded && jump)
			{
				// Add a vertical force to the player.
				m_Grounded = false;
				m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
				anim.SetTrigger("isJumping");
			}
		}
	}

	public IEnumerator Stun(float seconds)
	{
		isStunned = true;
		var main = stunParticles.main;
		main.startLifetime = seconds;
		stunParticles.Play();
		impulseSource.GenerateImpulse();
		m_Rigidbody2D.velocity = Vector3.zero;
		yield return new WaitForSeconds(seconds);
		isStunned = false;
	}


	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;
		sr.flipX = !sr.flipX;
	}
}
