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

    private void Update()
    {
        if(transform.position.y < -6f)
            gameObject.SetActive(false);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.tag == Names.Tags.Player.ToString())
            Invoke("MakeDynamic", fallDelay);
    }

    void MakeDynamic()
    {
        collidedObject.isKinematic = false;
    }
}
