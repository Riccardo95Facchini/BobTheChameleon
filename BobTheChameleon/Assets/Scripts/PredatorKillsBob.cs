using UnityEngine;

public class PredatorKillsBob : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.tag == Names.Tags.Player.ToString())
            EventManager.TriggerEvent(Names.Events.PlayerHit);
    }
}
