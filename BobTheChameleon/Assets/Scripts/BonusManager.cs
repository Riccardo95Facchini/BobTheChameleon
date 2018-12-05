using UnityEngine;

public class BonusManager : MonoBehaviour
{
    public int bonus;

    public void Gain(int takenGain)
    {
        bonus = bonus + takenGain;
        Debug.Log("bonus: " + bonus);
    }
}
