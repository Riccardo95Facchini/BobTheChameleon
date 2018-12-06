using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public int health;
    public int maxHealth;
    public Transform player;

    void Awake()
    {
        health = maxHealth;
        EventManager.StartListening(Names.Events.PlayerHit, PlayerHit);
    }

    void Update()
    {
        CheckFall();
    }

    public void Respawn()
    {
        // TODO: checkpoint system? 
        EventManager.TriggerEvent(Names.Events.Respawn);
        health = maxHealth;
    }

    private void CheckFall()
    {
        if(player.position.y < -6)
            EventManager.TriggerEvent(Names.Events.PlayerDead);
    }

    #region EventManager
    private void PlayerHit()
    {
        health--;

        if(health < 1)
            EventManager.TriggerEvent(Names.Events.PlayerDead);

        if(health > maxHealth)
            health = maxHealth;
    }
    #endregion

}
