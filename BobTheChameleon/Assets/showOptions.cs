using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class showOptions : MonoBehaviour
{
    public void ShowOptions()
    {
        SceneManager.LoadScene("OptionsMenu");
    }
}
