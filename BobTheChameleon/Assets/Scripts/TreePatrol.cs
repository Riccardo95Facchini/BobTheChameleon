using System.Collections;
using UnityEngine;

public class TreePatrol : MonoBehaviour
{
    [SerializeField]
    private Transform leftPosition;
    [SerializeField]
    private Transform rightPosition;
    [SerializeField]
    private Enemy enemyData;
    [SerializeField]
    private LayerMask WhatIsObstacleAndPlayer;
    [SerializeField]
    private CircleCollider2D sightTrigger;
    [SerializeField]
    private CircleCollider2D hitTrigger;

    private float flipInterval;
    private float chargeSpeed;

    private Transform playerPosition;
    private IEnumerator currentCoroutine;

    private bool isLookingLeft;
    private bool isOnTree;

    void Awake()
    {
        flipInterval = enemyData.flipInterval;
        chargeSpeed = enemyData.chargeSpeed;

        playerPosition = null;
        isLookingLeft = true;
        isOnTree = true;
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
    /// Starts following the player as long as it's in sight
    /// </summary>
    /// <returns>null</returns>
    private IEnumerator FollowPlayer()
    {
        sightTrigger.enabled = false;
        hitTrigger.enabled = true;
        while(PlayerInSight())
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
    private IEnumerator GoBackToTree(Transform target)
    {
        while(transform.position != target.position)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.position, chargeSpeed * Time.deltaTime);
            yield return null;
        }
        sightTrigger.enabled = true;
        hitTrigger.enabled = false;
        isOnTree = true;
        currentCoroutine = ChangeSide();
        StartCoroutine(currentCoroutine);
        yield return null;
    }

    /// <summary>
    /// If player in the collision, checks if is in sight and start coroutine to follow, otherwise go back to the tree.
    /// </summary>
    private bool PlayerInSight()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position,
                    (playerPosition.position - transform.position).normalized,
                    Vector2.Distance(transform.position, playerPosition.position), WhatIsObstacleAndPlayer);

        if(hit.collider.tag == Names.Tags.Player.ToString())
        {
            if(isOnTree)
            {
                StopCoroutine(currentCoroutine);
                isOnTree = false;
                currentCoroutine = FollowPlayer();
                StartCoroutine(currentCoroutine);
            }
            return true;
        }
        else
        {
            SetBackToTree();
            return false;
        }
    }

    /// <summary>
    /// Sets the values for going back to the tree
    /// </summary>
    private void SetBackToTree()
    {
        playerPosition = null;
        Transform startPosition = isLookingLeft ? leftPosition.transform : rightPosition.transform;
        StopCoroutine(currentCoroutine);
        currentCoroutine = GoBackToTree(startPosition);
        StartCoroutine(currentCoroutine);
    }

    //Trigger for the collision when following
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!isOnTree)
        {
            if(collision.tag == Names.Tags.Player.ToString())
            {
                EventManager.TriggerEvent(Names.Events.PlayerHit);
                SetBackToTree();
            }
        }
    }

    //Trigger for the line of sight
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(isOnTree)
        {
            if(collision.tag == Names.Tags.Player.ToString())
            {
                if(!playerPosition)
                    playerPosition = collision.transform;

                PlayerInSight();
            }
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