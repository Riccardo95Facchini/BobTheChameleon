using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour {

    public int health;
    public int maxHealth;
    public GameObject DeathPanel;
    public GameObject characterControllerObject;
    
    


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

    }


    public void Die() {

        Debug.Log("you're dead");


        characterControllerObject.GetComponent<CharacterController2D>().SetDead(true);

        DeathPanel.SetActive(true);
        Cursor.visible = true;
    }

    public void Respawn() {

        Cursor.visible = false;

        DeathPanel.SetActive(false);
        characterControllerObject.GetComponent<CharacterController2D>().SetDead(false);
        health = maxHealth;

    }
}
