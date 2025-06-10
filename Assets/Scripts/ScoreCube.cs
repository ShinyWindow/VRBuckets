using UnityEngine;
using UnityEngine.InputSystem; // <-- Required for the new input system
using Normal.Realtime;
using System.Collections.Generic;

public class ScoreCube : MonoBehaviour
{
    private readonly HashSet<int> _scoredPlayers = new();
    private InputAction scoreAction;

    private void OnEnable()
    {
        scoreAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/space");
        scoreAction.performed += OnScoreAction;
        scoreAction.Enable();
    }

    private void OnDisable()
    {
        scoreAction.performed -= OnScoreAction;
        scoreAction.Disable();
    }

    private void OnScoreAction(InputAction.CallbackContext ctx)
    {
        var localPlayer = GetLocalPlayer();
        if (localPlayer != null)
        {
            var score = localPlayer.GetComponent<PlayerScore>();
            if (score != null)
            {
                score.AddScore(1);
                Debug.Log($"[Test] Spacebar pressed. Score increased by 1. New total: {score.GetScore()}");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("PlayerHand")) return;

        var player = other.GetComponentInParent<RealtimeView>();
        if (player == null || _scoredPlayers.Contains(player.ownerID)) return;

        var score = player.GetComponent<PlayerScore>();
        if (score != null)
        {
            score.AddScore(1);
            _scoredPlayers.Add(player.ownerID);
            Debug.Log($"Player {player.ownerID} scored! New score: {score.GetScore()}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("PlayerHand")) return;

        var player = other.GetComponentInParent<RealtimeView>();
        if (player != null)
        {
            _scoredPlayers.Remove(player.ownerID);
        }
    }

    private GameObject GetLocalPlayer()
    {
        var manager = FindObjectOfType<RealtimeAvatarManager>();
        foreach (var kvp in manager.avatars)
        {
            if (kvp.Value.isLocalAvatar)
                return kvp.Value.gameObject;
        }
        return null;
    }
}
