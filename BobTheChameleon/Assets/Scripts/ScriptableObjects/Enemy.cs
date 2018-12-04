using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy")]
public class Enemy : ScriptableObject
{
    public float walkSpeed;     //Ground enemies only
    public float chargeSpeed;   //All moving enemies
    public float lineOfSight;   //Ground enemies only
    public float flipInterval;  //Birds only
}
