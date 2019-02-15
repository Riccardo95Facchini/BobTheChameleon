using System.Collections;
using UnityEngine;

public class PreyPatrol : MonoBehaviour
{
    [SerializeField] private Prey preyData;

    private float movementRange;
    private float preySpeed;
    private float newPositionInterval;

    private Vector2 startingPosition;
    private Vector2 nextPosition;

    private SpriteRenderer spriteRenderer;

    private bool isEscaped;
    private bool isCaught;
    private bool isEaten;
    private bool isLookingRight;

    public BonusManager bonusManager;

    /// <summary>
    /// Chaching and coroutine starting
    /// </summary>
    private void OnEnable()
    {
        isEscaped = false;
        isCaught = false;
        isEaten = false;
        isLookingRight = true;
        //Scriptable object data
        movementRange = preyData.movementRange;
        preySpeed = preyData.preySpeed;
        newPositionInterval = preyData.newPositionInterval;

        startingPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(RandomMovement());
        StartCoroutine(NewPosition());
    }

    /// <summary>
    /// Moves the prey towards a random position
    /// </summary>
    /// <returns>null</returns>
    private IEnumerator RandomMovement()
    {
        while(true)
        {
            transform.position = Vector2.MoveTowards(transform.position, nextPosition, preySpeed * Time.deltaTime);
            yield return null;
        }
    }

    /// <summary>
    /// Generates a new random position in the range, each interval.
    /// </summary>
    /// <returns>After time newPositionInterval starts again</returns>
    private IEnumerator NewPosition()
    {
        while(true)
        {
            nextPosition = new Vector2(
                startingPosition.x + Random.Range(-movementRange, movementRange),
                startingPosition.y + Random.Range(-movementRange, movementRange));

            CheckFlip(nextPosition.x, transform.position.x);

            yield return new WaitForSeconds(newPositionInterval);
        }
    }

    /// <summary>
    /// Flips the sprite if the fly goes the other way
    /// </summary>
    /// <param name="nextX">Value of the X after moving</param>
    /// <param name="previousX">Value of the X before moving</param>
    private void CheckFlip(float nextX, float previousX)
    {
        if(nextX < previousX && isLookingRight)
        {
            spriteRenderer.flipX = !spriteRenderer.flipX;
            isLookingRight = false;
        }
        else if(nextX > previousX && !isLookingRight)
        {
            spriteRenderer.flipX = !spriteRenderer.flipX;
            isLookingRight = true;
        }
    }

    /// <summary>
    /// Flees 
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    private IEnumerator Flee(Transform player)
    {
        Vector2 dir;
        float previousX;
        while(!isEscaped)
        {
            previousX = transform.position.x;
            dir = (transform.position - player.position).normalized;
            transform.Translate(dir * preySpeed * Time.deltaTime);
            CheckFlip(transform.position.x, previousX);
            yield return null;
        }
        gameObject.SetActive(false);
        yield return null;
    }

    /// <summary>
    /// Reels the prey to Bob
    /// </summary>
    /// <param name="mouth">Position of the mouth</param>
    /// <returns>null</returns>
    private IEnumerator GetReeled(Transform mouth)
    {
        while(!isEaten)
        {
            transform.position = Vector2.MoveTowards(transform.position, mouth.position, Time.deltaTime * preySpeed * 10);
            yield return null;
        }

        gameObject.SetActive(false);
        bonusManager.Gain(10); //to be adjusted 
        
        

    }

    /// <summary>
    /// Stops random movement and new position generation, starts Flee coroutine
    /// </summary>
    public void StartFlee(Transform player)
    {
        StopAllCoroutines();
        StartCoroutine(Flee(player));
    }

    /// <summary>
    /// Called when the raycast in TongueRenderer hits the prey
    /// </summary>
    /// <param name="mouth">Position of the mouth</param>
    public void Caught(Transform mouth)
    {
        StopAllCoroutines();
        isCaught = true;
        StartCoroutine(GetReeled(mouth));
        

    }

    /// <summary>
    /// If caught and collides with the player, it's eaten
    /// </summary>
    /// <param name="collision">What collided with the prey</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == Names.Tags.Player.ToString())
            isEaten = isCaught;
    }

    public bool IsCaught()
    {
        return isCaught;
    }

    private void OnBecameInvisible()
    {
        isEscaped = true;
    }

    private void OnBecameVisible()
    {
        isEscaped = false;
    }
}
