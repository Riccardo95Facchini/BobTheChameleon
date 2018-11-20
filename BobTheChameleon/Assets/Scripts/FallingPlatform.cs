using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour {

    Rigidbody2D collidedObject;

	// Use this for initialization
	void Start () {
        collidedObject = GetComponent<Rigidbody2D>();
	}

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name.Equals ("Player")) {
            Invoke("MakeDynamic", 1f);
            Destroy(gameObject, 4f);
        }
    }

    void MakeDynamic() {
        collidedObject.isKinematic = false;
    }
}
