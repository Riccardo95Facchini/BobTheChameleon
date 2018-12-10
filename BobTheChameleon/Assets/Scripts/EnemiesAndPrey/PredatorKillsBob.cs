using UnityEngine;

public class PredatorKillsBob : MonoBehaviour
{
    private PlayerInLineOfSight camouflageCheck;

    private void Awake()
    {
        camouflageCheck = GetComponent<PlayerInLineOfSight>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == Names.Tags.Player.ToString() && camouflageCheck.WasSpottedBeforeCamouflage())
        {
            EventManager.TriggerEvent(Names.Events.PlayerHit);
        }
    }
}
