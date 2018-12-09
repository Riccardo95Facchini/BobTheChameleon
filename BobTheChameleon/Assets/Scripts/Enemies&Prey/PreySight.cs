using UnityEngine;

public class PreySight : MonoBehaviour
{
    [SerializeField]
    private PreyPatrol preyPatrol;
    [SerializeField]
    private PlayerInLineOfSight sightCheck;

    private Transform playerPosition = null;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(preyPatrol.IsCaught())
        {
            gameObject.SetActive(false);
        }
        else if(collision.tag == Names.Tags.Player.ToString())
        {
            if(playerPosition == null)
                playerPosition = collision.transform;

            if(sightCheck.IsPlayerInSight(playerPosition.position, Vector2.Distance(transform.position, playerPosition.position)))
            {
                preyPatrol.StartFlee(collision.transform);     //Calls method to start fleeing
                gameObject.SetActive(false);                   //Disables trigger that serves no purpose now
            }
        }
    }
}
