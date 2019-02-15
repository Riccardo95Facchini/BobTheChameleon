
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public Text scoreText;
    public BonusManager bonusManager;
    
    

    

    // Update is called once per frame
    void Update()
    {

        scoreText.text = bonusManager.bonus.ToString();
        
    }



}
