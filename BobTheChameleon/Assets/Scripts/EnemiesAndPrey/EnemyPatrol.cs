using System.Collections;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [SerializeField] private Transform groundDetection;
    [SerializeField] private Enemy enemyData;
    [SerializeField] private PlayerInLineOfSight sightCheck;

    public LayerMask whatIsPlayer;
    public LayerMask whatIsGround;
    public Animator animator;

    private float walkSpeed;
    private float chargeSpeed;
    private float lineOfSight;

    private bool movingLeft = true;
    private bool charging;

    private Vector2 rayDirection;
    private Transform player = null;

    //Caching values from the ScriptableObject
    private void Awake()
    {
        walkSpeed = enemyData.walkSpeed;
        chargeSpeed = enemyData.chargeSpeed;
        lineOfSight = enemyData.lineOfSight;
    }

    private void OnEnable()
    {
        StartCoroutine(StartPatrol());
    }

    private IEnumerator StartPatrol()
    {
        while(true)
        {
            HorizontalObstacleCheck();
            VerticalGroundCheck();

            //Movement using transform and speed taken from method
            transform.Translate(Vector2.left * SpeedSet() * Time.deltaTime);
            yield return null;
        }
    }

    /// <summary>
    /// If not charging checks if next frame it should, otherwise checks if player is behind
    /// </summary>
    /// <returns>walkSpeed when not charging, chargeSpeed when charging</returns>
    private float SpeedSet()
    {
        rayDirection = movingLeft ? Vector2.left : Vector2.right;
        if(!charging)
        {
            charging = IsPlayerInSightSimple();
            return walkSpeed;
        }
        else
        {
            FollowPlayer();
            return chargeSpeed;
        }
    }

    /// <summary>
    /// If the player jumps behind during chase, flip sprite
    /// </summary>
    private void FollowPlayer()
    {
        RaycastHit2D playerBehind = Physics2D.Raycast(transform.position, -rayDirection, lineOfSight, whatIsPlayer);

        if(playerBehind.collider != false)
        {
            FlipSprite();
            charging = IsPlayerInSightSimple();
        }
    }

    /// <summary>
    /// Checks if the player is in line of sight, if so start charging
    /// </summary>
    private bool IsPlayerInSightSimple()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection, lineOfSight, whatIsPlayer);

        if(hit.collider != null && sightCheck.SpottedCheck(hit.collider))
        {
            if(player == null)
                player = hit.collider.transform;
            animator.SetBool("Attacking", true);
            return true;
        }
        sightCheck.Reset();
        return false;
    }

    /// <summary>
    /// Checks if the enemy is about to fall from a platform and flips it if so
    /// </summary>
    private void VerticalGroundCheck()
    {
        RaycastHit2D verticalCheck = Physics2D.Raycast(groundDetection.position, Vector2.down, 0.2f);

        if(verticalCheck.collider == false)
        {
            charging = false;
            FlipSprite();
        }
    }

    /// <summary>
    /// Checks if the enemy is about to bump into an obstacle and flips it if so
    /// </summary>
    private void HorizontalObstacleCheck()
    {
        rayDirection = movingLeft ? Vector2.left : Vector2.right;

        RaycastHit2D horizontalCheck = Physics2D.Raycast(groundDetection.position, rayDirection, 0.2f, whatIsGround);

        if(horizontalCheck.collider != false)
        {
            if(horizontalCheck.collider.tag != Names.Tags.Player.ToString())
            {
                charging = false;
                FlipSprite();
            }
        }
    }

    /// <summary>
    /// Turns the sprite 180 degrees on itself and changes the movingLeft boolean
    /// </summary>
    private void FlipSprite()
    {
        if(movingLeft)
            transform.eulerAngles = new Vector3(0, -180, 0);
        else
            transform.eulerAngles = new Vector3(0, 0, 0);

        movingLeft = !movingLeft;

        animator.SetBool("Attacking", false);
    }
}
