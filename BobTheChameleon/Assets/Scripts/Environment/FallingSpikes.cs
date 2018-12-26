using UnityEngine;

public class FallingSpikes : MonoBehaviour
{

    [SerializeField] private BoxCollider2D trigger;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private readonly float fallDelay;
    [SerializeField] private float respawnDelay = 5f;

    private Rigidbody2D rb;
    private Vector2 startPosition;
    private SpriteRenderer spikeSprite;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spikeSprite = GetComponent<SpriteRenderer>();
        startPosition = transform.position;
    }

    private void OnBecameInvisible()
    {
        if(!rb.isKinematic)
        {
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
            transform.position = startPosition;
            spikeSprite.enabled = false;
            boxCollider.enabled = false;
            trigger.enabled = false;
            Invoke("Respawn", respawnDelay);
        }
    }

    private void Respawn()
    {
        spikeSprite.enabled = true;
        boxCollider.enabled = true;
        trigger.enabled = true;
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.tag == Names.Tags.Player.ToString())
        {
            EventManager.TriggerEvent(Names.Events.PlayerHit);
            boxCollider.enabled = false;
        }
        else if(collision.collider.tag == Names.Tags.Ground.ToString())
            boxCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == Names.Tags.Player.ToString())
        {
            trigger.enabled = false;
            Invoke("MakeDynamic", fallDelay);
        }
    }

    void MakeDynamic()
    {
        rb.isKinematic = false;
    }
}
