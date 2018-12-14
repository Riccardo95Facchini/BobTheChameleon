using UnityEngine;

public class FallingPlatform : MonoBehaviour
{

    [SerializeField]
    private readonly float fallDelay;

    private Rigidbody2D collidedObject;

    void Awake()
    {
        collidedObject = GetComponent<Rigidbody2D>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.tag == Names.Tags.Player.ToString())
            Invoke("MakeDynamic", fallDelay);
    }

    private void OnBecameInvisible()
    {
        if(!collidedObject.isKinematic)
            gameObject.SetActive(false);
    }

    void MakeDynamic()
    {
        collidedObject.isKinematic = false;
    }
}
