using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InterLevelMenu : MonoBehaviour
{

    private int levelToLoad; 

    public void GoToNextLevel()
    {
        levelToLoad = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(levelToLoad);
    }
}
