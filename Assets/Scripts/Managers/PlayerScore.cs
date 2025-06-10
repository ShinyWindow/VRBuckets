using Normal.Realtime;
using UnityEngine;
using System.Collections;

// Manages the player's score and combo state across the network.
// Listens for reset signals and syncs score data using Normcore.
public class PlayerScore : RealtimeComponent<PlayerScoreModel>
{
    private ResetSignalModel _resetSignalModel;
    private int _lastSeenResetCounter = -1;

    private void Start()
    {
        _resetSignalModel = FindObjectOfType<ResetSignal>()?.SignalModel;
        if (_resetSignalModel != null)
            _resetSignalModel.resetCounterDidChange += OnResetCounterChanged;
    }

    private void OnResetCounterChanged(ResetSignalModel model, int value)
    {
        if (_lastSeenResetCounter != value)
        {
            _lastSeenResetCounter = value;
            ResetScore();
        }
    }

    public void AddScore(int amount)
    {
        if (model == null)
            return;

        if (!isOwnedLocally)
            model.RequestOwnership();

        model.score += amount;
    }

    public void IncrementCombo()
    {
        if (model != null)
            model.combo = Mathf.Min(model.combo + 1, 2);
    }

    public void ResetCombo()
    {
        if (model != null)
            model.combo = 0;
    }

    public int GetCombo() => model != null ? model.combo : 0;
    public int GetScore() => model != null ? model.score : 0;

    public void ResetScore()
    {
        if (model == null)
            return;

        if (!isOwnedLocally)
        {
            model.RequestOwnership();
            StartCoroutine(WaitAndReset());
        }
        else
        {
            ApplyReset();
        }
    }

    private IEnumerator WaitAndReset()
    {
        float elapsed = 0f;
        const float timeout = 1f;

        while (!isOwnedLocally && elapsed < timeout)
        {
            yield return null;
            elapsed += Time.deltaTime;
        }

        if (isOwnedLocally)
            ApplyReset();
    }

    private void ApplyReset()
    {
        model.score = 0;
        model.combo = 0;
    }

    protected override void OnRealtimeModelReplaced(PlayerScoreModel previousModel, PlayerScoreModel currentModel)
    {
        if (previousModel != null)
            previousModel.scoreDidChange -= OnScoreChanged;

        if (currentModel != null)
            currentModel.scoreDidChange += OnScoreChanged;
    }

    private void OnScoreChanged(PlayerScoreModel model, int score)
    {
        // No-op: remove or use if needed later
    }
}
