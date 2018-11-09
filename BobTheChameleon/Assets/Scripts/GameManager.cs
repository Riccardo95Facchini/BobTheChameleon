using UnityEngine;

public class GameManager : MonoBehaviour
{


    public static GameManager instance { get; private set; }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {

        if(Input.GetKeyDown(KeyCode.Mouse0))
            EventManager.TriggerEvent(Names.Events.TongueOut.ToString());

        if(Input.GetKeyUp(KeyCode.Mouse0))
            EventManager.TriggerEvent(Names.Events.TongueIn.ToString());
    }
}
