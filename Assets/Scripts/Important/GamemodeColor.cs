using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class GamemodeColor : MonoBehaviour
{
    public GameMode targetMode;                // The mode that triggers the "active" color
    public Material activeMaterial;            // Material to use when mode matches
    public Material inactiveMaterial;          // Material to use when mode does not match

    private GameManager _gameManager;
    private MeshRenderer _meshRenderer;
    private GameMode _lastMode = (GameMode)(-1); // Track last mode to avoid redundant updates

    void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _gameManager = GameManager.Instance;
    }

    void Update()
    {
        if (_gameManager == null || _gameManager.CurrentGameMode == _lastMode)
            return;

        _lastMode = _gameManager.CurrentGameMode;

        if (_lastMode == targetMode)
            _meshRenderer.material = activeMaterial;
        else
            _meshRenderer.material = inactiveMaterial;
    }
}
