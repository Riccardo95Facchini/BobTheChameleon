using UnityEngine;

public class LadderTriggerer : MonoBehaviour
{

    private PlayerMovement player = null;

    void OnTriggerEnter2D(Collider2D collision)
    {
        //Chaching
        if(player == null && collision.tag == Names.Tags.Player.ToString())
            player = collision.GetComponent<PlayerMovement>();
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == Names.Tags.Player.ToString())
        {
            player.SetIsOnLadder(false);
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if(Input.GetKey(KeyCode.W) && collision.tag == Names.Tags.Player.ToString())
            player.SetIsOnLadder(true);
    }
}
