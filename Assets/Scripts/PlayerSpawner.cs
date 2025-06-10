using UnityEngine;
using Normal.Realtime;

public class PlayerSpawner : MonoBehaviour
{
    private Realtime _realtime;

    void Start()
    {
        _realtime = FindFirstObjectByType<Realtime>();

        // Wait until we're connected to a room before spawning the player
        _realtime.didConnectToRoom += OnConnectedToRoom;
    }

    private void OnConnectedToRoom(Realtime realtime)
    {
        var options = new Realtime.InstantiateOptions
        {
            ownedByClient = true,
            preventOwnershipTakeover = true,
            useInstance = realtime
        };

        Realtime.Instantiate("NetworkedPlayer", Vector3.zero, Quaternion.identity, options);
    }
}
