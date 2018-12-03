using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour {

    public int health;
    public int maxHealth;
    public GameObject DeathPanel;
    public GameObject characterControllerObject;
    public GameObject tongueRendererObject;
    public GameObject player;
   

    
    
    


    public void Damage(int damageTaken)
    {
        if (health < 1)
        {
            Die();
        }

        if (health > maxHealth) { health = maxHealth; }

    }

    // Use this for initialization
    void Start()
    {
        health = maxHealth;
        DeathPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        checkFall();
    }


    public void Die() {

        Debug.Log("you're dead");

        
        characterControllerObject.GetComponent<CharacterController2D>().SetDead(true);
        tongueRendererObject.GetComponent<TongueRenderer>().SetOff(true);
        
        
        
        DeathPanel.SetActive(true);
        Cursor.visible = true;
    }

    public void Respawn() {

        
        //Cursor.visible = false;

        DeathPanel.SetActive(false);

        player.GetComponent<Transform>().position = new Vector2(1,3);//in order to translate bob at the beginning of the level or at last checkpoint 
        tongueRendererObject.GetComponent<TongueRenderer>().SetOff(false);
        characterControllerObject.GetComponent<CharacterController2D>().SetDead(false);
        health = maxHealth;

    }


    private void checkFall() {

        if (player.GetComponent<Transform>().position.y<-6) {
            Die();
        }

    }
    
}
