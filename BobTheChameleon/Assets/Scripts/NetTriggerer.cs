using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetTriggerer : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}




    void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log("in this moment bob can start climbing ");

        if (Input.GetKeyDown(KeyCode.W) && collision.tag == "Player")
        {

            collision.GetComponent<PlayerMovement>().SetIsOnLadder(true);



        }
    }


    void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("outgoing collision with" + collision.gameObject.name);

        collision.GetComponent<PlayerMovement>().SetIsOnLadder(false);
    }
}
