using UnityEngine;

public class CharacterController2D : MonoBehaviour
{
    [SerializeField] private float m_JumpForce = 700f;                          // Amount of force added when the player jumps.
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
    [SerializeField] private bool m_AirControl = false;                         // Whether or not a player can steer while jumping;
    [SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
    [SerializeField] private Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.
    [SerializeField] private GameObject mouth;

    private const float groundCheckRadius = .25f; // Radius of the overlap circle to determine if grounded
    private float originalGravity;

    private bool m_Grounded;            // Whether or not the player is grounded
    private bool m_FacingRight = true;  // For determining which way the player is currently facing.
    private bool jumped, doubleJumped;

    private Rigidbody2D m_Rigidbody2D;
    private DistanceJoint2D tongueJoint;
    private Vector3 velocity = Vector3.zero;

    public AudioManager audioManager;
    public Animator animator;


    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        tongueJoint = GetComponent<DistanceJoint2D>();
        originalGravity = m_Rigidbody2D.gravityScale;
    }

    private void FixedUpdate()
    {
        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, groundCheckRadius, m_WhatIsGround);

        if(colliders.Length == 0)
            m_Grounded = false;

        for(int i = 0; i < colliders.Length; i++)
        {
            if(colliders[i].gameObject != gameObject)
            {
                Debug.Log(colliders[i].gameObject.name);
                m_Grounded = true;
                jumped = false;
                doubleJumped = false;
            }
        }
    }

    public void Move(float horizontal, bool jump, bool onLadder)
    {
        //only control the player if grounded or airControl is turned on
        if((m_Grounded || m_AirControl) && !onLadder)
        {
            // Play stepping sound if the playing is walking on the floor

            HandleAnimation(horizontal);

            // Move the character by finding the target velocity
            Vector3 targetVelocity = Vector3.zero;
            if(tongueJoint.enabled)
                targetVelocity = new Vector2(horizontal * 15f, m_Rigidbody2D.velocity.y);
            else
                targetVelocity = new Vector2(horizontal * 10f, m_Rigidbody2D.velocity.y);

            // And then smoothing it out and applying it to the character
            m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref velocity, m_MovementSmoothing);

            // If the input is moving the player right and the player is facing left...
            if(horizontal > 0 && !m_FacingRight)
                Flip();
            // Otherwise if the input is moving the player left and the player is facing right...
            else if(horizontal < 0 && m_FacingRight)
                Flip();
        }

        if(onLadder)
            handleLadder();
        else
        {
            m_Rigidbody2D.gravityScale = originalGravity;
            //Can jump only if not on a ladder
            if(jump)
            {
                    CheckAndJump();
            }
        }

    }

    private void HandleAnimation(float horizontal)
    {
        if(horizontal != 0f && (m_Grounded))
        {
            if(!audioManager.IsPlaying("walk"))
                audioManager.Play("walk");
        }
        if(horizontal == 0 || !m_Grounded)
            audioManager.Stop("walk");

        if(!m_Grounded)
        {
            animator.SetBool("Jumping", true);
        }
        if(horizontal != 0 && m_Grounded)
        {
            animator.SetBool("Jumping", false);
            animator.SetBool("Moving", true);
        }

        else if(m_Grounded)
        {
            animator.SetBool("Jumping", false);
            animator.SetBool("Moving", false);
        }
    }

    private void handleLadder()
    {
        float speed = 10;
        m_Rigidbody2D.gravityScale = 0;
        m_Rigidbody2D.velocity = Vector2.zero;

        Vector2 direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        transform.Translate(direction * (speed * Time.deltaTime));

        //if(Input.GetKey(KeyCode.W))
        //{
        //    m_Rigidbody2D.velocity = new Vector2(0, speed * Time.deltaTime);
        //}
        //else if(Input.GetKey(KeyCode.S))
        //{
        //    m_Rigidbody2D.velocity = new Vector2(0, -speed * Time.deltaTime);
        //}
        //else
        //{
        //    m_Rigidbody2D.velocity = Vector2.zero;
        //}
    }

    /// <summary>
    /// Checks if the character can jump and executes the movement.
    /// </summary>
    private void CheckAndJump()
    {
        if(tongueJoint.enabled)
            EventManager.TriggerEvent(Names.Events.TongueIn);

        if(m_Grounded)
        {
            m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce * 1f));
            audioManager.Play("jump1");
        }
        else if(!doubleJumped)
        {
            m_Rigidbody2D.velocity = Vector3.zero;
            m_Rigidbody2D.angularVelocity = 0;

            m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce * 1));
            audioManager.Play("jump2");
            doubleJumped = true;
        }
        m_Grounded = false;

    }


    public void Flip()
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;

        if(m_Grounded)
            EventManager.TriggerEvent(Names.Events.TongueIn);

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    public bool getFacingRight()
    {
        return m_FacingRight;
    }

    public bool getGrounded()
    {
        return m_Grounded;
    }
}