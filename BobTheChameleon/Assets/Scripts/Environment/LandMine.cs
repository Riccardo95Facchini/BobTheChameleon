using UnityEngine;

public class LandMine : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == Names.Tags.Player.ToString())
            EventManager.TriggerEvent(Names.Events.PlayerDead);
    }
}
