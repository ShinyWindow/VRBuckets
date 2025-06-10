using UnityEngine;
using UnityEngine.InputSystem;
using Normal.Realtime;
using System.Collections;

public class ScoreResetManager : MonoBehaviour
{
    private InputAction _resetAction;
    private ResetSignal _resetSignal;

    [Header("Ball Respawn")]
    public string basketballPrefabName = "Basketball"; // Must match Resources prefab name
    public Transform spawnPoint; // Set in inspector

    void OnEnable()
    {
        _resetAction = new InputAction(binding: "<Keyboard>/space");
        _resetAction.performed += ctx => StartCoroutine(ResetGame());
        _resetAction.Enable();

        _resetSignal = FindFirstObjectByType<ResetSignal>();
    }

    void OnDisable()
    {
        _resetAction.Disable();
    }

    private IEnumerator ResetGame()
    {
        if (_resetSignal == null || _resetSignal.SignalModel == null)
        {
            Debug.LogWarning("[ScoreResetManager] ResetSignal model not found.");
            yield break;
        }

        // 🔥 Destroy all basketballs
        var basketballs = FindObjectsOfType<Ball>();
        foreach (var ball in basketballs)
        {
            var rt = ball.GetComponent<RealtimeTransform>();
            if (rt != null && rt.isOwnedLocallySelf)
            {
                Realtime.Destroy(ball.gameObject);
            }
        }

        // 🕒 Wait 1 frame to ensure destruction is synced
        yield return null;

        // 🆕 Respawn one new basketball
        if (spawnPoint != null)
        {
            Realtime.Instantiate(basketballPrefabName, spawnPoint.position, spawnPoint.rotation);
        }

        // 📢 Send reset signal
        _resetSignal.SignalModel.resetCounter++;
        Debug.Log("[ScoreResetManager] All basketballs removed and reset signal sent.");
    }
}
