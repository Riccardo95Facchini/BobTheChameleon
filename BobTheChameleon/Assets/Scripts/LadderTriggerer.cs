using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderTriggerer : MonoBehaviour {


  

     /*void OnTriggerEnter(Collider other)   
     {
         if (other.gameObject.tag=="Player")Debug.Log("ingoing collision with :  " + other.gameObject.name);

         //other.GetComponent<PlayerMovement>().SetIsOnLadder(true);
     }


     void OnTriggerExit(Collider other)
     {
         Debug.Log("outgoing collision with: " + other.gameObject.name);
     }*/





     void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("ingoing collision with: " + collision.gameObject.name);// funziona quando bob interseca la scala la collisione viene rilevata

        if (Input.GetKeyDown(KeyCode.W))
        {

            collision.GetComponent<PlayerMovement>().SetIsOnLadder(true);

        }
    }


     void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("outgoing collision with"+collision.gameObject.name);

        collision.GetComponent<PlayerMovement>().SetIsOnLadder(false);
    }


     void OnTriggerStay2D(Collider2D collision )
    {
        Debug.Log("in this moment bob can start climbing ");

        if (Input.GetKeyDown(KeyCode.W) && collision.tag=="Player") {

            collision.GetComponent<PlayerMovement>().SetIsOnLadder(true);



        }
    }

}
