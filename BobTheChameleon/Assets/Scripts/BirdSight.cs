using System.Collections;
using UnityEngine;

public class BirdSight : MonoBehaviour
{

    [SerializeField]
    private TreePatrol birdPatrol;
    [SerializeField]
    private PlayerInLineOfSight sightCheck;

    private Transform playerPosition = null;
    private bool isAiming;
    private bool isAttacking;

    private void Awake()
    {
        isAiming = false;
        isAttacking = false;
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
                StartCoroutine(Aim());
            }
        }
    }

    /// <summary>
    /// If it can see the player, start aiming for it and attack
    /// </summary>
    /// <returns></returns>
    private IEnumerator Aim()
    {
        while(true)
        {
            if(sightCheck.IsPlayerInSight(playerPosition.position, Vector2.Distance(transform.position, playerPosition.position)))
            {
                birdPatrol.StartAttack(playerPosition);
                isAttacking = true;
            }
            else if(isAttacking)
            {
                birdPatrol.StopAttack();
                isAiming = false;
                StopAllCoroutines();
            }
            yield return null;
        }
    }
}
