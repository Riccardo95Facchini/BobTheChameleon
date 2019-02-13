using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public void PlayGame() {
        SceneManager.LoadScene("0.PlayMenu");
    }

    public void LoadMenu() {
        SceneManager.LoadScene("Menu");
    }

    public void QuitGame() {
        Debug.Log("QUIT!");
        Application.Quit();
    }
}
