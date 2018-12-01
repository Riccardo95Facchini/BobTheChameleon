using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredatorKillsBob : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}



     void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name.Equals("Player")) {
            collision.GetComponent<HealthManager>().Die();
                }
    }
    /* protected virtual void Kill(Collider2D collision)
     {
         Debug.Log("Enemy attack");

         HealthManager healthManager = collision.GetComponent<HealthManager>();

         if (!healthManager) return;
         else
         {

             healthManager.Die();

         }
     }*/
}
