using UnityEngine;
using Normal.Realtime;

public class ThrowMetadata : MonoBehaviour
{
    public PlayerScore thrower;        // Local reference to the player
    public Vector3 throwPosition;      // Where the ball was released
    public int zoneThrownFrom = 1;     // Calculated when thrown
    public bool scored = false;        // Set true when it goes in
}
