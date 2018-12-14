using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonsScript: MonoBehaviour {

    [SerializeField] GameObject image;

    public void ClickInstructions()
    {
        image.SetActive(true);
    }

    public void ClickBack()
    {
        image.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
