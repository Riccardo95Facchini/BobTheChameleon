using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingPrayPatrol : MonoBehaviour {



   
    [SerializeField]
    private float escapeSpeed;
    [SerializeField]
    private float radius;
    

     public GameObject player;


    private bool movingLeft = false;
    private bool escaping;


    void Update()
    {
        float dist = computeDistanceFromBob();
        bool danger = Escape(dist);

        if (danger)
        {
            //while (computeDistanceFromBob() < 2 * radius)
            {
                Run();
            }
        
        }
     
    }

    private float computeDistanceFromBob() {
        
        float xPlayer=player.GetComponent<Transform>().position.x;
        float yPlayer= player.GetComponent<Transform>().position.y;
        float xPray = transform.position.x;
        float yPray= transform.position.y;

        float d = Mathf.Sqrt((xPlayer - xPray)*(xPlayer-xPray) + (yPlayer - yPray)*(yPlayer-yPray));
        

        return d;
    }

    private bool Escape(float d) {

        if (d <= radius) return true;
        else return false;

    }

    private void Run() {

        

        float xPlayer = player.GetComponent<Transform>().position.x;

        float xPray = transform.position.x;

        if (xPlayer <= xPray) {

            
            transform.Translate(Vector2.right * escapeSpeed * Time.deltaTime);
        }
        else transform.Translate(Vector2.left * escapeSpeed * Time.deltaTime);


    }

    

}
