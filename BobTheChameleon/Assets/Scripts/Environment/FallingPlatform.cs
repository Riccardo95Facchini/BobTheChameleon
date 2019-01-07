using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    [SerializeField] private float fallDelay;
    [SerializeField] private float respawnDelay = 5f;

    private Rigidbody2D rb;
    private Vector2 startPosition;
    private BoxCollider2D platformCollider;
    private SpriteRenderer platformSprite;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        platformCollider = GetComponent<BoxCollider2D>();
        platformSprite = GetComponent<SpriteRenderer>();
        startPosition = transform.position;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.tag == Names.Tags.Player.ToString() && rb.isKinematic)
            Invoke("MakeDynamic", fallDelay);
    }

    void MakeDynamic()
    {
        rb.isKinematic = false;
    }

    private void OnBecameInvisible()
    {
        if(!rb.isKinematic)
        {
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
            transform.position = startPosition;
            platformSprite.enabled = false;
            platformCollider.enabled = false;
            Invoke("Respawn", respawnDelay);
        }
    }

    private void Respawn()
    {
        platformSprite.enabled = true;
        platformCollider.enabled = true;
    }
}
