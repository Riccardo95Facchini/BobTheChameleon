using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Transform respPos; // TODO: only for prototype

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if(collision.tag == Names.Tags.Player.ToString())
        //    GameManager.Instance.checkpoint = respPos.position;
    }
}
