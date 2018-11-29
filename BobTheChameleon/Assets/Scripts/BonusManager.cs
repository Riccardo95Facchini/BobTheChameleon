using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusManager : MonoBehaviour {

    public int bonus;
    public int minBonus;
    





    public void Gain(int takenGain)
    {
        

        if (bonus < minBonus) { bonus= minBonus; }

    }

    // Use this for initialization
    void Start()
    {
        bonus = minBonus;
        
    }

    // Update is called once per frame
    void Update()
    {

    }


 
}
