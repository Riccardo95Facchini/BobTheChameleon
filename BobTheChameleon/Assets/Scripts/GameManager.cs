using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject player;
    [SerializeField] private GameObject DeathPanel;
    [SerializeField] private float slowMotionValue;

    [SerializeField, Header("Note: Will be affeted by slowMotionValue"), Tooltip("If slowMotionValue = 0.5f, the effect duration will be double")]
    private float slowMotionDuration;

    private bool isPlayerDead;
    private static bool inMenu;

    private static int currentLevel = 0;

    private void Awake()
    {
        EventManager.StartListening(Names.Events.PlayerDead, PlayerDead);
        SceneManager.sceneLoaded += OnLoadCallback;
        if(Instance == null)
        {
            Instance = this;
            isPlayerDead = false;
            DontDestroyOnLoad(gameObject);

            //currentLevel = 0;
            //inMenu = true;
            //Load(currentLevel);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        player = GameObject.FindWithTag(Names.Tags.Player.ToString());
    }

    void OnLoadCallback(Scene scene, LoadSceneMode sceneMode)
    {
        player = GameObject.FindWithTag(Names.Tags.Player.ToString());
    }

    void Update()
    {
        //Debug.Log(currentLevel);

        if(!inMenu)
        {
            if(!isPlayerDead)
            {
                if(Input.GetKeyDown(KeyCode.Escape))
                {
                    inMenu = true;
                    SceneManager.LoadScene(0);
                }
                if(Input.GetKeyDown(KeyCode.Return))
                    Load(currentLevel);

                if(Input.GetKeyDown(KeyCode.Mouse0))
                    EventManager.TriggerEvent(Names.Events.TongueOut);
                else if(Input.GetKeyUp(KeyCode.Mouse0))
                    EventManager.TriggerEvent(Names.Events.TongueIn);
            }
        }
    }

    /// <summary>
    /// Load a specific level
    /// </summary>
    /// <param name="sceneId">Id of the level</param>
    public void Load(int sceneId)
    {
        if(sceneId != 0)
            inMenu = false;

        currentLevel = sceneId;
        SceneManager.LoadScene(currentLevel);
    }

    /// <summary>
    /// Loads the next level in the ArrayList
    /// </summary>
    public void LoadNext()
    {
        currentLevel++;
        //if(currentLevel > 8)
         //   currentLevel = 0;
        SceneManager.LoadScene(currentLevel);
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
        Load(currentLevel);

        isPlayerDead = false;
        DeathPanel.SetActive(false);
        player.SetActive(true);
        EventManager.StopListening(Names.Events.Respawn, Respawn);
    }
    #endregion
}
