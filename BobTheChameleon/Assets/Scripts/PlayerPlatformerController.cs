using UnityEngine;

public class PlayerPlatformerController : PhysicsObject
{
    [Header("Player speed and Jump")]
    public float maxSpeed = 7;
    public float jumpTakeOffSpeed = 7;

    private SpriteRenderer spriteRenderer;
    private Animator animator;

    void Awake()
    {
        //spriteRenderer = GetComponent<SpriteRenderer>();
        //animator = GetComponent<Animator>();
    }

    protected override void ComputeVelocity()
    {
        Vector2 move = Vector2.zero;

        move.x = Input.GetAxis("Horizontal");

        //Doesn't allow double jump
        if(Input.GetButtonDown("Jump") && grounded)
            velocity.y = jumpTakeOffSpeed;

        else if(Input.GetButtonUp("Jump")) //Stop jumping if key is let go
        {
            if(velocity.y > 0)
                velocity.y = velocity.y * 0.5f;
        }

        /*
         * bool flipSprite = (spriteRenderer.flipX ? (move.x > 0.01f) : (move.x < -0.01f));

        if(flipSprite)
            spriteRenderer.flipX = !spriteRenderer.flipX;

        //animator.SetBool("grounded", grounded);
        animator.SetFloat("velocityX", Mathf.Abs(velocity.x)/maxSpeed);
        */

        targetVelocity = move * maxSpeed;
    }
}
