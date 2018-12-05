using UnityEngine;

public class PreySight : MonoBehaviour
{
    [SerializeField]
    private PreyPatrol preyPatrol;
    [SerializeField]
    private LayerMask WhatIsObstacleAndPlayer;

    private Transform playerPosition = null;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == Names.Tags.Player.ToString())
        {
            if(playerPosition == null)
                playerPosition = collision.transform;

            if(IsPlayerInSight())
            {
                preyPatrol.StartFlee();     //Calls method to start fleeing
                gameObject.SetActive(false);//Disables trigger that serves no purpose now
            }
        }
    }

    /// <summary>
    /// If player in the collision, checks if it's in sight
    /// </summary>
    /// <returns>ture if player is in sight, false otherwise</returns>
    private bool IsPlayerInSight()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position,
                    (playerPosition.position - transform.position).normalized,
                    Vector2.Distance(transform.position, playerPosition.position), WhatIsObstacleAndPlayer);

        if(hit.collider.tag == Names.Tags.Player.ToString())
            return true;
        else
            return false;
    }
}
