using UnityEngine;

[CreateAssetMenu(fileName = "New Prey", menuName = "Prey")]
public class Prey : ScriptableObject
{
    public float movementRange;
    public float preySpeed;
    public float newPositionInterval;
}
