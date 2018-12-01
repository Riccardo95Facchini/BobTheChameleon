using UnityEngine;

public class CharacterController2D : MonoBehaviour
{
    [SerializeField] private const float m_JumpForce = 700f;                          // Amount of force added when the player jumps.
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
    [SerializeField] private bool m_AirControl = false;                         // Whether or not a player can steer while jumping;
    [SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
    [SerializeField] private Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.


    [SerializeField] private Transform m_CeilingCheck;                          // A position marking where to check for ceilings
    [SerializeField] private const float maxTongueLength = 5f;                             // Minimum tongue length
    [SerializeField] private const float minTongueLength = 0.25f;                             // A position marking where to check for ceilings
    [SerializeField] private readonly GameObject mouth;

    const float k_GroundedRadius = .1f; // Radius of the overlap circle to determine if grounded
    private bool m_Grounded;            // Whether or not the player is grounded.
    const float k_CeilingRadius = .1f; // Radius of the overlap circle to determine if the player can stand up
    private Rigidbody2D m_Rigidbody2D;
    private DistanceJoint2D tongueJoint;
    private bool m_FacingRight = true;  // For determining which way the player is currently facing.
    private Vector3 velocity = Vector3.zero;
    private bool doubleJumped;


    [SerializeField] private bool dead;//

    public void SetDead(bool value) {//
        dead = value;//
    }//


    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        tongueJoint = GetComponent<DistanceJoint2D>();
    }

    private void FixedUpdate()
    {
        m_Grounded = false;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        for(int i = 0; i < colliders.Length; i++)
        {
            if(colliders[i].gameObject != gameObject)
            {
                m_Grounded = true;
                doubleJumped = false;
            }
        }
    }


    public void Move(float horizontal, bool jump, bool onLadder)
    {
        if(!dead){
            
            //only control the player if grounded or airControl is turned on
            if ((m_Grounded || m_AirControl) && !onLadder)
            {
                // Move the character by finding the target velocity
                Vector3 targetVelocity = Vector3.zero;
                if (tongueJoint.enabled)
                    targetVelocity = new Vector2(horizontal * 15f, m_Rigidbody2D.velocity.y);
                else
                    targetVelocity = new Vector2(horizontal * 10f, m_Rigidbody2D.velocity.y);

                // And then smoothing it out and applying it to the character
                m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref velocity, m_MovementSmoothing);

                // If the input is moving the player right and the player is facing left...
                if (horizontal > 0 && !m_FacingRight)
                {
                    // ... flip the player.
                    Flip();
                }
                // Otherwise if the input is moving the player left and the player is facing right...
                else if (horizontal < 0 && m_FacingRight)
                {
                    // ... flip the player.
                    Flip();
                }
            }

            if (onLadder)
            {//

                float speed = 5;

                Vector2 direction = new Vector2(0, 0);
                float yMovement = Input.GetAxis("Vertical");
                float xMovement = Input.GetAxis("Horizontal");

                if (yMovement != 0) { direction.y = yMovement; }
                if (xMovement != 0) { direction.x = xMovement; }

                transform.Translate(direction * (speed * Time.deltaTime));

                if (Input.GetKey(KeyCode.W))
                {

                    m_Rigidbody2D.velocity = new Vector2(0, speed);
                }
                else if (Input.GetKey(KeyCode.S))
                {

                    m_Rigidbody2D.velocity = new Vector2(0, -speed);
                }

                else
                {
                    m_Rigidbody2D.velocity = new Vector2(0, 3 / 4);
                }
            }

            if (jump)
                CheckAndJump();
        }
    }

    /// <summary>
    /// Checks if the character can jump and executes the movement.
    /// </summary>
    private void CheckAndJump()
    {
        if(!dead){
            if (tongueJoint.enabled)
                EventManager.TriggerEvent(Names.Events.TongueIn.ToString());

            if (m_Grounded)
            {
                m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce * 1f));
            }
            else if (!doubleJumped)
            {


                m_Rigidbody2D.velocity = Vector3.zero;
                m_Rigidbody2D.angularVelocity = 0;

                m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce * 1));
                doubleJumped = true;
            }

            m_Grounded = false;
        }
    }


    public void Flip()
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;

        if(m_Grounded)
            EventManager.TriggerEvent(Names.Events.TongueIn.ToString());

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
