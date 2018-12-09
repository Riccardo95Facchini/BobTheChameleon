using System.Collections;
using UnityEngine;

public class Patrol : MonoBehaviour
{
    [SerializeField]
    private Transform groundDetection;
    [SerializeField]
    private Transform playerDetection;
    [SerializeField]
    private Enemy enemyData;

    public LayerMask whatIsPlayer;
    public Animator animator;
    public AudioManager audioManager;

    private float walkSpeed;
    private float chargeSpeed;
    private float lineOfSight;
    private string soundName;

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
        soundName = enemyData.sound;
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
        if(!charging)
        {
            charging = IsPlayerInSight();
            animator.SetBool("Attacking", false);
            return walkSpeed;
        }
        else
        {
            FollowPlayer();
            animator.SetBool("Attacking", true);
            if (!audioManager.IsPlaying(soundName))
                audioManager.Play(soundName);

            return chargeSpeed;
        }
    }

    /// <summary>
    /// If the player jumps behind during chase, flip sprite
    /// </summary>
    private void FollowPlayer()
    {
        RaycastHit2D playerBehind = Physics2D.Raycast(playerDetection.position, -rayDirection, Mathf.Infinity, whatIsPlayer);

        if(playerBehind.collider != false)
            FlipSprite();
    }

    /// <summary>
    /// Checks if the player is in line of sight, if so start charging
    /// </summary>
    private bool IsPlayerInSight()
    {
        RaycastHit2D playerInSight = Physics2D.Raycast(playerDetection.position, rayDirection, lineOfSight, whatIsPlayer);

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
    private void VerticalGroundCheck()
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
    private void HorizontalObstacleCheck()
    {
        rayDirection = movingLeft ? Vector2.left : Vector2.right;

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
    }
}
