using UnityEngine;
using UnityEngine.InputSystem;
using Normal.Realtime;

public class TimeAttackBallSpawner : MonoBehaviour
{
    public InputActionProperty spawnAction; // Assign Right Controller input
    public string basketballPrefabName = "Basketball"; // Must match prefab in Resources

    private GameManager _gameManager;
    private Realtime _realtime;
    private Transform _handTransform;

    void Awake()
    {
        _handTransform = transform;
        _gameManager = GameManager.Instance;
        _realtime = FindFirstObjectByType<Realtime>();
    }

    void Start()
    {
        // Ensure fallback if not available in Awake
        if (_gameManager == null)
            _gameManager = GameManager.Instance;

        if (_realtime == null)
            _realtime = FindFirstObjectByType<Realtime>();
    }

    void Update()
    {
        // Live fallback in case GameManager or Realtime initialized late
        if (_gameManager == null)
            _gameManager = GameManager.Instance;

        if (_realtime == null)
            _realtime = FindFirstObjectByType<Realtime>();

        if (_gameManager == null || _realtime == null)
        {
            Debug.LogWarning("[TimeAttackBallSpawner] Missing GameManager or Realtime instance.");
            return;
        }

        if (_gameManager.CurrentGameMode != GameMode.TimeAttack)
            return;

        if (spawnAction.action == null)
        {
            Debug.LogWarning("[TimeAttackBallSpawner] Spawn action not assigned!");
            return;
        }

        if (spawnAction.action.WasPerformedThisFrame())
        {
            Debug.Log("[TimeAttackBallSpawner] Button pressed. Attempting to spawn basketball.");

            var options = new Realtime.InstantiateOptions
            {
                ownedByClient = true,
                preventOwnershipTakeover = false,
                destroyWhenOwnerLeaves = true,
                useInstance = _realtime
            };

            var obj = Realtime.Instantiate(
                basketballPrefabName,
                _handTransform.position,
                _handTransform.rotation,
                options
            );

            if (obj != null)
                Debug.Log("[TimeAttackBallSpawner] Basketball successfully instantiated.");
            else
                Debug.LogError("[TimeAttackBallSpawner] Failed to instantiate basketball.");
        }
    }

    void OnEnable()
    {
        spawnAction.action.Enable();
        Debug.Log("[TimeAttackBallSpawner] Input action enabled.");
    }

    void OnDisable()
    {
        spawnAction.action.Disable();
        Debug.Log("[TimeAttackBallSpawner] Input action disabled.");
    }
}
