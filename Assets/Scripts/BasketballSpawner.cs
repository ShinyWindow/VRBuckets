using UnityEngine;
using Normal.Realtime;

// Spawns a basketball at the start if no networked basketball is already present.
public class BasketballSpawner : MonoBehaviour
{
    [Header("Basketball Settings")]
    public string basketballPrefabName = "Basketball"; // Must match Resources prefab name
    public Transform spawnPoint;

    private Realtime _realtime;

    private void Start()
    {
        _realtime = FindFirstObjectByType<Realtime>();
        if (_realtime != null)
            _realtime.didConnectToRoom += OnDidConnectToRoom;
    }

    private void OnDidConnectToRoom(Realtime realtime)
    {
        // Only the host should spawn, and we need a spawn point
        if (realtime.clientID != 0 || spawnPoint == null)
            return;

        var balls = Object.FindObjectsByType<Ball>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        foreach (var ball in balls)
        {
            var view = ball.GetComponent<RealtimeView>();
            if (view != null && !view.isSceneView)
                return;
        }

        var options = new Realtime.InstantiateOptions
        {
            ownedByClient = true,
            preventOwnershipTakeover = false,
            useInstance = realtime
        };

        Realtime.Instantiate(
            basketballPrefabName,
            spawnPoint.position,
            spawnPoint.rotation,
            options
        );
    }
}
