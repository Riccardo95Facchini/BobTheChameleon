using UnityEngine;

public class HidingSpot : MonoBehaviour
{
    private Camouflage player;

    void Awake()
    {
        player = null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == Names.Tags.Player.ToString())
        {
            if(player == null)
                player = collision.GetComponent<Camouflage>();

            player.SetCamouflageFlag(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == Names.Tags.Player.ToString())
            player.SetCamouflageFlag(false);
    }
}