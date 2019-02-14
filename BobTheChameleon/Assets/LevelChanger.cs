using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelChanger : MonoBehaviour
{
    public Animator animator;
    private int levelToLoad;
    public GameObject finishingFlag;


    // Update is called once per frame
    void Update()
    {
        //FadeToNextLevel();

        if (finishingFlag.GetComponent<NextLevel>().getCollided()) {
        //}


        // (Input.GetMouseButton(0)) {          //useful to quickly test the change of the level
            FadeToNextLevel();
        }
    }


    public void FadeToNextLevel() {
        FadeToLevel(SceneManager.GetActiveScene().buildIndex+1);

    }


    public void FadeToLevel(int levelIndex) {

        levelToLoad = levelIndex;
        animator.SetTrigger("FadeOut");
    }



    public void OnFadeComplete() {

        SceneManager.LoadScene(levelToLoad);
    }
}
