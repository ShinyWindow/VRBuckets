using UnityEngine;
using UnityEngine.InputSystem;

public class GameModeToggle : MonoBehaviour
{
    private InputAction _toggleAction;
    private GameManager _gameManager;

    void OnEnable()
    {
        _toggleAction = new InputAction(binding: "<Keyboard>/enter");
        _toggleAction.performed += ctx => ToggleGameMode();
        _toggleAction.Enable();

    }

    void OnDisable()
    {
        _toggleAction.Disable();
    }

    private void ToggleGameMode()
    {
        Debug.Log("[GameModeToggle] Enter key pressed. Trying to toggle mode.");

        if (_gameManager == null)
            _gameManager = GameManager.Instance;

        if (_gameManager == null)
        {
            Debug.LogWarning("[GameModeToggle] GameManager is still null.");
            return;
        }

        Debug.Log($"[GameModeToggle] Current mode: {_gameManager.CurrentGameMode}");

        if (_gameManager.CurrentGameMode == GameMode.FreePlay)
            _gameManager.SetGameMode(GameMode.TimeAttack);
        else
            _gameManager.SetGameMode(GameMode.FreePlay);
    }


}
