using UnityEngine;

public class Patrol : MonoBehaviour
{

    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float chargeSpeed;
    [SerializeField]
    private float lineOfSight;
    [SerializeField]
    private Transform groundDetection;

    public LayerMask whatIsPlayer;

    private bool movingLeft = true;
    private bool charging;

    private Vector2 rayDirection;
    private Transform player = null;

    void Update()
    {
        float speed;

        HorizontalCheck();
        VerticalCheck();

        if(!charging)
        {
            speed = walkSpeed;
            charging = PlayerInSight();
        }
        else
        {
            speed = chargeSpeed;
            FollowPlayer();
        }

        transform.Translate(Vector2.left * speed * Time.deltaTime);
    }

    /// <summary>
    /// If the player jumps behind during chase, flip sprite
    /// </summary>
    private void FollowPlayer()
    {
        RaycastHit2D playerBehind = Physics2D.Raycast(transform.localPosition, -rayDirection, Mathf.Infinity, whatIsPlayer);

        if(playerBehind.collider != false)
            FlipSprite();
    }

    /// <summary>
    /// Checks if the player is in line of sight, if so start charging
    /// </summary>
    private bool PlayerInSight()
    {
        RaycastHit2D playerInSight = Physics2D.Raycast(transform.localPosition, rayDirection, lineOfSight, whatIsPlayer);

        if(playerInSight.collider != false)
        {
            if(player == null)
                player = playerInSight.collider.transform;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Checks if the enemy is about to fall from a platform and flips it if so
    /// </summary>
    private void VerticalCheck()
    {
        RaycastHit2D verticalCheck = Physics2D.Raycast(groundDetection.position, Vector2.down, 0.5f);

        if(verticalCheck.collider == false)
        {
            charging = false;
            FlipSprite();
        }
    }

    /// <summary>
    /// Checks if the enemy is about to bump into an obstacle and flips it if so
    /// </summary>
    private void HorizontalCheck()
    {
        if(movingLeft)
            rayDirection = Vector2.left;
        else
            rayDirection = Vector2.right;

        RaycastHit2D horizontalCheck = Physics2D.Raycast(groundDetection.position, rayDirection, 0.2f);

        if(horizontalCheck.collider != false)
        {
            if(horizontalCheck.collider.tag != Names.Tags.Player.ToString())
            {
                charging = false;
                FlipSprite();
            }
        }
    }

    private void FlipSprite()
    {
        //charging = PlayerInSight();

        if(movingLeft)
        {
            transform.eulerAngles = new Vector3(0, -180, 0);
            movingLeft = false;
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
            movingLeft = true;
        }
    }
}
