using UnityEngine;

public class NextLevel : MonoBehaviour
{
    private bool collided = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == Names.Tags.Player.ToString())
        {
            if(!collided)
                GameManager.Instance.LoadNext();
            collided = true;
        }
    }
}