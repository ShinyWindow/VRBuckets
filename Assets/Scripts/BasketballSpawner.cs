using UnityEngine;
using Normal.Realtime;

public class BasketballSpawner : MonoBehaviour
{
    [Header("Basketball Settings")]
    public string basketballPrefabName = "Basketball"; // Must match Resources prefab name
    public Transform spawnPoint; // Assign in inspector

    private Realtime _realtime;

    void Start()
    {
        _realtime = FindFirstObjectByType<Realtime>();

        if (_realtime == null)
        {
            Debug.LogError("[BasketballSpawner] No Realtime instance found.");
            return;
        }

        _realtime.didConnectToRoom += OnDidConnectToRoom;
    }

    private void OnDidConnectToRoom(Realtime realtime)
    {
        Debug.Log($"[BasketballSpawner] Connected with clientID: {realtime.clientID}");

        if (realtime.clientID != 0)
        {
            Debug.Log("[BasketballSpawner] Not the host. Skipping basketball spawn.");
            return;
        }

        var balls = FindObjectsOfType<Ball>();
        foreach (var ball in balls)
        {
            var view = ball.GetComponent<RealtimeView>();
            if (view != null && !view.isSceneView)
            {
                Debug.Log("[BasketballSpawner] Basketball already exists. No need to spawn.");
                return;
            }
        }

        if (spawnPoint != null)
        {
            Realtime.Instantiate(basketballPrefabName, spawnPoint.position, spawnPoint.rotation);
            Debug.Log("[BasketballSpawner] No basketball found. Spawned new one.");
        }
        else
        {
            Debug.LogWarning("[BasketballSpawner] No spawn point assigned.");
        }
    }
}
