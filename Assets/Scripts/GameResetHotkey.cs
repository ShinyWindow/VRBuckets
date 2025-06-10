using UnityEngine;
using UnityEngine.InputSystem;

public class GameResetHotkey : MonoBehaviour
{
    private Keyboard _keyboard;

    private void Awake()
    {
        _keyboard = Keyboard.current;
    }

    private void Update()
    {
        if (_keyboard == null) return;

        if (_keyboard.oKey.wasPressedThisFrame)
        {
            var gameManager = GameManager.Instance;
            if (gameManager != null && gameManager.realtime.clientID == 0)
            {
                Debug.Log("[GameResetHotkey] Resetting game state.");
                gameManager.SetGameOver(false);
                gameManager.SetWinner("");
                // Optional: Reset scores, respawn ball, etc.
            }
        }
    }
}
