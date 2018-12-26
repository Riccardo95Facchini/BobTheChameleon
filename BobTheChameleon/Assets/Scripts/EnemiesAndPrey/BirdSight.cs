using System.Collections;
using UnityEngine;

public class BirdSight : MonoBehaviour
{

    [SerializeField]
    private TreePatrol birdPatrol;
    [SerializeField]
    private PlayerInLineOfSight sightCheck;
    [SerializeField]
    private LayerMask WhatIsObstacleAndPlayer;

    private Transform playerPosition = null;
    private bool isAiming;

    private void Awake()
    {
        isAiming = false;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == Names.Tags.Player.ToString())
        {
            if(playerPosition == null)
                playerPosition = collision.transform;

            if(!isAiming)
            {
                isAiming = true;
                sightCheck.SetToDefault();
                StartCoroutine(Aim());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        sightCheck.SetToDefault();
    }

    /// <summary>
    /// If it can see the player, start aiming for it and attack
    /// </summary>
    /// <returns></returns>
    private IEnumerator Aim()
    {
        if(sightCheck.IsPlayerInSight(playerPosition.position, Vector2.Distance(transform.position, playerPosition.position)))
        {
            birdPatrol.StartAttack(playerPosition);
        }
        else
        {
            isAiming = false;
            StopAllCoroutines();
        }

        RaycastHit2D hit;  // TODO: fix this botching you did for the prototype

        while(true)
        {
            hit = Physics2D.Raycast(transform.position,
                    (playerPosition.position - transform.position).normalized,
                    Vector2.Distance(playerPosition.position, transform.position), WhatIsObstacleAndPlayer);
            if(hit.collider != null && hit.collider.tag != Names.Tags.Player.ToString())
            {
                birdPatrol.StopAttack();
                isAiming = false;
                StopAllCoroutines();
            }
            yield return null;
        }
    }
}
