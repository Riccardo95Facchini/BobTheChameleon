using UnityEngine;

public class PredatorKillsBob : MonoBehaviour
{
    private PlayerInLineOfSight spottedCheck;
    private Camouflage playerState;

    private void Awake()
    {
        spottedCheck = GetComponent<PlayerInLineOfSight>();
        playerState = null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == Names.Tags.Player.ToString())
        {
            if(playerState == null)
                playerState = collision.GetComponent<Camouflage>();

            if(spottedCheck.WasSpottedBeforeCamouflage() || !playerState.IsCamouflaged())
            {
                EventManager.TriggerEvent(Names.Events.PlayerHit);
                spottedCheck.SetToDefault();
            }
        }
    }
}
