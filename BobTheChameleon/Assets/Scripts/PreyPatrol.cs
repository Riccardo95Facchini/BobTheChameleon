using System.Collections;
using UnityEngine;

public class PreyPatrol : MonoBehaviour
{

    private float movementRange = 2f; // TODO: take it from scriptable object
    private float preySpeed = 5; // TODO: take it from scriptable object
    private float newPositionInterval = 1f; // TODO: take it from scriptable object

    private Vector2 startingPosition;
    private Vector2 nextPosition;

    /// <summary>
    /// Chaching and coroutine starting
    /// </summary>
    private void OnEnable()
    {
        startingPosition = transform.position;
        StartCoroutine(RandomMovement());
        StartCoroutine(NewPosition());
    }

    /// <summary>
    /// Stops random movement and new position generation, starts Flee coroutine
    /// </summary>
    public void StartFlee()
    {
        StopAllCoroutines();
        StartCoroutine(Flee());
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

    private IEnumerator Flee()
    {
        yield return null;
    }
}
