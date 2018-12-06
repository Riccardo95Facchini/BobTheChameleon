using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField]
    private GameObject player;
    [SerializeField]
    private GameObject DeathPanel;

    private bool isPlayerDead;

    private void Awake()
    {
        EventManager.StartListening(Names.Events.PlayerDead, PlayerDead);
        if(Instance == null)
        {
            Instance = this;
            isPlayerDead = false;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if(!isPlayerDead)
        {
            if(Input.GetKeyDown(KeyCode.Mouse0))
                EventManager.TriggerEvent(Names.Events.TongueOut);

            if(Input.GetKeyUp(KeyCode.Mouse0))
                EventManager.TriggerEvent(Names.Events.TongueIn);
        }
    }

    #region EventManager
    private void PlayerDead()
    {
        isPlayerDead = true;
        DeathPanel.SetActive(true);
        player.SetActive(false);
        EventManager.StartListening(Names.Events.Respawn, Respawn);
        EventManager.TriggerEvent(Names.Events.TongueIn);   //Useful if player dies while swinging
    }
    private void Respawn()
    {
        isPlayerDead = false;
        DeathPanel.SetActive(false);
        player.SetActive(true);
        EventManager.StopListening(Names.Events.Respawn, Respawn);
    }
    #endregion
}
