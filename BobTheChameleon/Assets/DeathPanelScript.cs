using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathPanelScript : MonoBehaviour
{
    public HealthManager healthManager;

    public void MenuAfterDeath()
    {
        
        EventManager.TriggerEvent(Names.Events.Respawn);
        SceneManager.LoadScene("Menu");
    }

    
}
