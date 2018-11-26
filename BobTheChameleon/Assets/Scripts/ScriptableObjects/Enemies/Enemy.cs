using UnityEngine;

public abstract class Enemy : ScriptableObject
{
    protected float speed;
    protected float attackSpeed;
    protected float lineOfSight;

    protected bool canBeEaten;
    protected bool isChasingBob;
}
