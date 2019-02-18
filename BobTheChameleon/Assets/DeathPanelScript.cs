using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathPanelScript : MonoBehaviour
{
    

    public void MenuAfterDeath()
    {
        
        Debug.Log("respawn");
        EventManager.TriggerEvent(Names.Events.Respawn);
        SceneManager.LoadScene("Menu");
    }

    
}
