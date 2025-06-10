using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using Normal.Realtime;
using System.Collections;

// This component handles both game mode switching via grab interaction and changing material color to indicate the active game mode.
[RequireComponent(typeof(XRGrabInteractable), typeof(MeshRenderer))]
public class GameModeButton : MonoBehaviour
{
    [Header("Game Mode Settings")]
    public GameMode modeToSet;
    public string basketballPrefabName = "Basketball";
    public Transform ballSpawnPoint;

    [Header("Visual Feedback")]
    public Material activeMaterial;
    public Material inactiveMaterial;

    private GameManager _gameManager;
    private ResetSignal _resetSignal;
    private ScoreboardManager _scoreboardManager;
    private MeshRenderer _meshRenderer;
    private GameMode _lastMode = (GameMode)(-1);

    private void Awake()
    {
        _gameManager = GameManager.Instance;
        _resetSignal = FindFirstObjectByType<ResetSignal>();
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        _gameManager = GameManager.Instance;
        _resetSignal = FindFirstObjectByType<ResetSignal>();
        _scoreboardManager = FindFirstObjectByType<ScoreboardManager>();
    }

    private void OnEnable()
    {
        GetComponent<XRGrabInteractable>().selectEntered.AddListener(OnSelectEnter);
    }

    private void OnDisable()
    {
        GetComponent<XRGrabInteractable>().selectEntered.RemoveListener(OnSelectEnter);
    }

    private void Update()
    {
        if (_gameManager == null) return;

        GameMode current = _gameManager.CurrentGameMode;
        if (current == _lastMode) return;

        _lastMode = current;
        _meshRenderer.material = (current == modeToSet) ? activeMaterial : inactiveMaterial;
    }

    private void OnSelectEnter(SelectEnterEventArgs args)
    {
        if (_gameManager == null || _gameManager.realtime.clientID != 0)
            return;

        _gameManager.SetGameOver(false);
        _scoreboardManager?.CancelCountdown();
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

                yield return null;

                if (rt.isOwnedLocallySelf)
                    Realtime.Destroy(ball.gameObject);
            }
        }

        yield return null;

        if (ballSpawnPoint != null)
        {
            Realtime.Instantiate(
                basketballPrefabName,
                ballSpawnPoint.position,
                ballSpawnPoint.rotation
            );
        }

        if (_resetSignal?.SignalModel != null)
            _resetSignal.SignalModel.resetCounter++;
    }
}
