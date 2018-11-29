using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProvaACrepare : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.GetComponent<HealthManager>().Die();
    }
}
