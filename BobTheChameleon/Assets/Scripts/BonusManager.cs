using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusManager : MonoBehaviour {

    public int bonus;
    public int minBonus=0;
    





    public void Gain(int takenGain)
    {


        if (bonus < minBonus) { bonus = minBonus; }
        bonus = bonus + takenGain;

        Debug.Log("bonus: " + bonus);

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
