using System.Collections;
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

    public Vector3 checkpoint;

    private static int currentLevel;

    private static ArrayList levels = new ArrayList()
{
    "0.MainMenu", "1.SampleSceneJump", "2.SampleSceneBush",
        "3.SampleScenePrey", "4.SampleSceneAnchor", "5.SampleSceneAcrobatics",
        "6.SampleScenePlatforms", "7.SampleSceneSpikes", "8.SampleSceneSnake",
        "9.SampleSceneEagle", "Alpha Demo Level"
};

    private void OnLevelWasLoaded(int level)
    {
        player = GameObject.FindWithTag(Names.Tags.Player.ToString());
        if(player != null)
            checkpoint = player.transform.position;
    }

    private void Awake()
    {
        EventManager.StartListening(Names.Events.PlayerDead, PlayerDead);
        if(Instance == null)
        {
            Instance = this;
            isPlayerDead = false;
            DontDestroyOnLoad(gameObject);

            currentLevel = 0;
            inMenu = true;
            Load(currentLevel);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        player = GameObject.FindWithTag(Names.Tags.Player.ToString());
        if(player != null)
            checkpoint = player.transform.position;
    }

    void Update()
    {
        if(!inMenu)
        {
            if(!isPlayerDead)
            {
                if(Input.GetKeyDown(KeyCode.Escape))
                {
                    inMenu = true;
                    SceneManager.LoadScene("0.MainMenu");
                }
                if(Input.GetKeyDown(KeyCode.Return))
                    Load(currentLevel);

                if(Input.GetKeyDown(KeyCode.Mouse0))
                    EventManager.TriggerEvent(Names.Events.TongueOut);
                else if(Input.GetKeyUp(KeyCode.Mouse0))
                    EventManager.TriggerEvent(Names.Events.TongueIn);

                if(Input.GetKeyDown(KeyCode.Mouse1))
                {
                    Time.timeScale = slowMotionValue;
                    Invoke("CancelSlowMotion", slowMotionDuration);
                }
                else if(Input.GetKeyUp(KeyCode.Mouse1))
                {
                    CancelInvoke();
                    Time.timeScale = 1f;
                }

                if(Input.GetKeyDown(KeyCode.KeypadEnter))
                    Respawn(); // TODO: only for prototype
            }
        }
    }

    private void CancelSlowMotion()
    {
        Time.timeScale = 1f;
    }

    /// <summary>
    /// Load a specific level in the Arraylist levels
    /// </summary>
    /// <param name="sceneId">Id of the level</param>
    public void Load(int sceneId)
    {
        if(sceneId != 0)
            inMenu = false;

        currentLevel = sceneId;
        SceneManager.LoadScene(levels[sceneId].ToString());
    }

    /// <summary>
    /// Loads the next level in the ArrayList
    /// </summary>
    public void LoadNext()
    {
        currentLevel++;
        SceneManager.LoadScene(levels[currentLevel].ToString());
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
        if(currentLevel != 10) //TODO: only prototype
            Load(currentLevel);
        else
        {
            player.GetComponent<PlayerMovement>().SetRespawn(checkpoint);
        }
        isPlayerDead = false;
        DeathPanel.SetActive(false);
        player.SetActive(true);
        EventManager.StopListening(Names.Events.Respawn, Respawn);
    }
    #endregion
}
