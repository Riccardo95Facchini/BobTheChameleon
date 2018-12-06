using System.Collections;
using UnityEngine;

public class PreyPatrol : MonoBehaviour
{
    private float movementRange = 2f; // TODO: take it from scriptable object
    private float preySpeed = 5; // TODO: take it from scriptable object
    private float newPositionInterval = 1f; // TODO: take it from scriptable object

    private Vector2 startingPosition;
    private Vector2 nextPosition;

    private bool isEscaped;
    private bool isCaught;
    private bool isEaten;

    /// <summary>
    /// Chaching and coroutine starting
    /// </summary>
    private void OnEnable()
    {
        isEscaped = false;
        isCaught = false;
        isEaten = false;
        startingPosition = transform.position;
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
            yield return new WaitForSeconds(newPositionInterval);
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
        while(!isEscaped)
        {
            dir = (transform.position - player.position).normalized;
            transform.Translate(dir * preySpeed * Time.deltaTime);
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
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.tag == Names.Tags.Player.ToString())
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
