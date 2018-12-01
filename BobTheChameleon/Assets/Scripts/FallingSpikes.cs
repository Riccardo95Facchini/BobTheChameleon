using UnityEngine;

public class FallingSpikes : MonoBehaviour
{

    Rigidbody2D spikes;

    // Use this for initialization
    void Start()
    {
        spikes = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name.Equals("Player"))
            Invoke("MakeDynamic", 0f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("u ded");
        
    }

    void MakeDynamic()
    {
        spikes.isKinematic = false;
    }
}
