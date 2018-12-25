using UnityEngine;

public class CharacterController2D : MonoBehaviour
{
    [SerializeField] private float m_JumpForce = 700f;                          // Amount of force added when the player jumps.
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = 0.05f; // How much to smooth out the movement
    [SerializeField] private bool m_AirControl = false;                         // Whether or not a player can steer while jumping;
    [SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
    [SerializeField] private Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.

    [SerializeField] private Animator animator;

    private const float groundCheckRadius = 0.27f; // Radius of the overlap circle to determine if grounded
    private float originalGravity;

    private bool isGrounded;            // Whether or not the player is grounded
    private bool isFacingRight = true;  // For determining which way the player is currently facing.
    private bool jumped, doubleJumped;

    private Rigidbody2D m_Rigidbody2D;
    private DistanceJoint2D tongueJoint;
    private Vector3 velocity = Vector3.zero;

    public AudioManager audioManager;

    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        tongueJoint = GetComponent<DistanceJoint2D>();
        originalGravity = m_Rigidbody2D.gravityScale;
    }

    private void Update()
    {
        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, groundCheckRadius, m_WhatIsGround);

        if(colliders.Length == 0)
            isGrounded = false;
        else if(!jumped || m_Rigidbody2D.velocity.y == 0)
        {
            jumped = false;
            isGrounded = true;
            doubleJumped = false;
        }
    }

    public void Move(float horizontal, bool jump, bool onLadder)
    {
        //only control the player if grounded or airControl is turned on
        if((isGrounded || m_AirControl) && !onLadder)
        {
            // Move the character by finding the target velocity
            Vector3 targetVelocity = Vector3.zero;
            if(tongueJoint.enabled)
            {
                jumped = false;
                doubleJumped = false;
                targetVelocity = new Vector2(horizontal * 15f, m_Rigidbody2D.velocity.y);
            }
            else
                targetVelocity = new Vector2(horizontal * 10f, m_Rigidbody2D.velocity.y);

            // And then smoothing it out and applying it to the character
            m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref velocity, m_MovementSmoothing);

            // If the input is moving the player right and the player is facing left...
            if(horizontal > 0 && !isFacingRight)
                Flip();
            // Otherwise if the input is moving the player left and the player is facing right...
            else if(horizontal < 0 && isFacingRight)
                Flip();
        }

        if(onLadder)
            handleLadder();
        else
        {
            m_Rigidbody2D.gravityScale = originalGravity;
            //Can jump only if not on a ladder
            if(jump)
                CheckAndJump();
        }
        HandleAnimation(horizontal);
    }

    private void HandleAnimation(float horizontal)
    {
        if(isGrounded)
        {
            if(horizontal != 0f)
            {
                if(!audioManager.IsPlaying("walk"))
                    audioManager.Play("walk");

                animator.SetBool("Jumping", false);
                animator.SetBool("Moving", true);
            }
            else
            {
                audioManager.Stop("walk");
                animator.SetBool("Jumping", false);
                animator.SetBool("Moving", false);
            }
        }
        else
        {
            if(!animator.GetBool("Jumping"))
                animator.SetBool("Jumping", true);

            animator.SetBool("Moving", false);
            audioManager.Stop("walk");
        }
    }

    private void handleLadder()
    {
        float speed = 10;
        m_Rigidbody2D.gravityScale = 0;
        m_Rigidbody2D.velocity = Vector2.zero;

        Vector2 direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        transform.Translate(direction * (speed * Time.deltaTime));
    }

    /// <summary>
    /// Checks if the character can jump and executes the movement.
    /// </summary>
    private void CheckAndJump()
    {
        if(tongueJoint.enabled)
            EventManager.TriggerEvent(Names.Events.TongueIn);

        if(isGrounded || !jumped)
        {
            m_Rigidbody2D.velocity = Vector3.zero;
            m_Rigidbody2D.angularVelocity = 0;

            m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce * 1f));
            audioManager.Play("jump1");
            jumped = true;
            isGrounded = false;
        }
        else if(!doubleJumped)
        {
            m_Rigidbody2D.velocity = Vector3.zero;
            m_Rigidbody2D.angularVelocity = 0;

            m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce * 1));
            audioManager.Play("jump2");
            doubleJumped = true;
        }
    }


    public void Flip()
    {
        // Switch the way the player is labelled as facing.
        isFacingRight = !isFacingRight;

        if(isGrounded)
            EventManager.TriggerEvent(Names.Events.TongueIn);

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    public bool getFacingRight()
    {
        return isFacingRight;
    }

    public bool getGrounded()
    {
        return isGrounded;
    }
}