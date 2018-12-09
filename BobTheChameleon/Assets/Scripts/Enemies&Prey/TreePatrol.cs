using System.Collections;
using UnityEngine;

public class TreePatrol : MonoBehaviour
{
    [SerializeField] private Transform leftPosition;
    [SerializeField] private Transform rightPosition;
    [SerializeField] private Enemy enemyData;
    [SerializeField] private PlayerInLineOfSight sightCheck;

    private float flipInterval;
    private float chargeSpeed;

    private Transform playerPosition;
    private IEnumerator currentCoroutine;

    private bool isLookingLeft;

    void Awake()
    {
        flipInterval = enemyData.flipInterval;
        chargeSpeed = enemyData.chargeSpeed;
        playerPosition = null;
        isLookingLeft = true;
    }

    private void OnEnable()
    {
        currentCoroutine = ChangeSide();
        StartCoroutine(currentCoroutine);
    }

    /// <summary>
    /// Moves the eagle to the other side of the tree
    /// </summary>
    private IEnumerator ChangeSide()
    {
        while(true)
        {
            FlipSprite();
            transform.position = isLookingLeft ? leftPosition.position : rightPosition.position;
            yield return new WaitForSeconds(flipInterval);
        }
    }

    /// <summary>
    /// Starts following the player, it can be stopped only by the sight script calling the stop
    /// </summary>
    /// <returns>null</returns>
    private IEnumerator FollowPlayer()
    {
        while(true)
        {
            transform.position = Vector2.MoveTowards(transform.position, playerPosition.position, chargeSpeed * Time.deltaTime);
            yield return null;
        }
    }

    /// <summary>
    /// Starts to go back to the tree and resume looking for the player
    /// </summary>
    /// <param name="target">Position to reach</param>
    /// <returns>null</returns>
    private IEnumerator GoBackToTree(Vector3 target)
    {
        if(isLookingLeft && transform.position.x < target.x)
            FlipSprite();
        else if(!isLookingLeft && transform.position.x > target.x)
            FlipSprite();

        while(transform.position != target)
        {
            transform.position = Vector2.MoveTowards(transform.position, target, chargeSpeed * Time.deltaTime);
            yield return null;
        }
        currentCoroutine = ChangeSide();
        StartCoroutine(currentCoroutine);
        yield return null;
    }

    /// <summary>
    /// Called when the player is in sight
    /// </summary>
    public void StartAttack(Transform playerPosition)
    {
        this.playerPosition = playerPosition;
        StopCoroutine(currentCoroutine);
        currentCoroutine = FollowPlayer();
        StartCoroutine(currentCoroutine);
    }

    /// <summary>
    /// Used to call SetBackToTree without exposing it
    /// </summary>
    public void StopAttack()
    {
        SetBackToTree();
    }

    /// <summary>
    /// Sets the values for going back to the tree
    /// </summary>
    private void SetBackToTree()
    {
        playerPosition = null;
        Vector3 startPosition = isLookingLeft ? leftPosition.transform.position : rightPosition.transform.position;
        StopCoroutine(currentCoroutine);
        currentCoroutine = GoBackToTree(startPosition);
        StartCoroutine(currentCoroutine);
    }

    //On collision with the player, call PlayerHit and go back to the tree
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.tag == Names.Tags.Player.ToString())
        {
            EventManager.TriggerEvent(Names.Events.PlayerHit);
            SetBackToTree();
        }
    }

    /// <summary>
    /// Flips the sprite and changes the boolean of the looking direction
    /// </summary>
    private void FlipSprite()
    {
        if(isLookingLeft)
            transform.eulerAngles = new Vector3(0, -180, 0);
        else
            transform.eulerAngles = new Vector3(0, 0, 0);

        isLookingLeft = !isLookingLeft;
    }
}