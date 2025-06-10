using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Normal.Realtime;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class GameModeHoverButton : MonoBehaviour
{
    public GameMode modeToSet;
    public Transform ballSpawnPoint;
    public string basketballPrefabName = "Basketball";

    private GameManager _gameManager;
    private ResetSignal _resetSignal;
    private ScoreboardManager _scoreboardManager;

    private void Awake()
    {
        _gameManager = GameManager.Instance;
        _resetSignal = FindFirstObjectByType<ResetSignal>();
    }

    private void Start()
    {
        _gameManager = GameManager.Instance;
        _resetSignal = FindFirstObjectByType<ResetSignal>();
        _scoreboardManager = FindFirstObjectByType<ScoreboardManager>();
    }


    private void OnEnable()
    {
        var interactable = GetComponent<XRGrabInteractable>();
        if (interactable != null)
            interactable.selectEntered.AddListener(OnSelectEnter);
    }

    private void OnDisable()
    {
        var interactable = GetComponent<XRGrabInteractable>();
        if (interactable != null)
            interactable.selectEntered.RemoveListener(OnSelectEnter);
    }

    private void Update()
    {
        if (_gameManager == null)
            _gameManager = GameManager.Instance;
    }




    private void OnSelectEnter(SelectEnterEventArgs args)
    {
        Debug.Log("[GameModeHoverButton] SelectEnter fired.");

        if (_gameManager == null)
            Debug.Log("[GameModeHoverButton] Skipped: GameManager is null.");
        else
            Debug.Log($"[GameModeHoverButton] clientID = {_gameManager.realtime.clientID}");

        if (_gameManager == null || _gameManager.realtime.clientID != 0)
        {
            Debug.Log("[GameModeHoverButton] Activation skipped due to conditions.");
            return;
        }

        Debug.Log($"[GameModeHoverButton] GRAB detected for {modeToSet}. Resetting game.");
        _gameManager.SetGameOver(false);
        _gameManager.SetWinner("");
        _scoreboardManager?.CancelCountdown();

        // Reset synced timer explicitly
        if (_gameManager.realtime.clientID == 0)
            _gameManager.ResetTimeAttackStartTime();


        _gameManager.SetGameMode(modeToSet);


        StartCoroutine(ResetBasketballsAndSignal());
    }



    private IEnumerator ResetBasketballsAndSignal()
    {
        foreach (var ball in FindObjectsOfType<Ball>())
        {
            var rt = ball.GetComponent<RealtimeTransform>();
            if (rt != null)
            {
                if (!rt.isOwnedLocallySelf)
                    rt.RequestOwnership();

                // Wait one frame to ensure ownership is transferred
                yield return null;

                if (rt.isOwnedLocallySelf)
                    Realtime.Destroy(ball.gameObject);
            }
        }

        // Wait one more frame to ensure all objects are fully cleaned up
        yield return null;

        if (ballSpawnPoint != null)
        {
            Realtime.Instantiate(
                basketballPrefabName,
                ballSpawnPoint.position,
                ballSpawnPoint.rotation
            );
        }

        if (_resetSignal != null && _resetSignal.SignalModel != null)
        {
            _resetSignal.SignalModel.resetCounter++;
            Debug.Log("[GameModeHoverButton] Reset signal sent.");
        }
    }

}
