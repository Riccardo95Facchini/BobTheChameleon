using UnityEngine;

public class FallingSpikes : MonoBehaviour
{

    [SerializeField]
    private BoxCollider2D trigger;
    [SerializeField]
    private BoxCollider2D boxCollider;
    [SerializeField]
    private readonly float fallDelay;

    private Rigidbody2D spikes;

    void Awake()
    {
        spikes = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if(transform.position.y < -6f)
            gameObject.SetActive(false);
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
        spikes.isKinematic = false;
    }
}
