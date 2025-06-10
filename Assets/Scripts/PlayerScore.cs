using Normal.Realtime;
using UnityEngine;
using System.Collections;

public class PlayerScore : RealtimeComponent<PlayerScoreModel>
{

    private ResetSignalModel _resetSignalModel;
    private int _lastSeenResetCounter = -1;

    void Start()
    {
        _resetSignalModel = FindObjectOfType<ResetSignal>()?.SignalModel;

        if (_resetSignalModel != null)
            _resetSignalModel.resetCounterDidChange += OnResetCounterChanged;
    }

    private void OnResetCounterChanged(ResetSignalModel model, int value)
    {
        if (_lastSeenResetCounter == value) return;
        _lastSeenResetCounter = value;

        ResetScore(); // We own our model so this is safe
    }


    public void AddScore(int amount)
    {
        if (model == null)
        {
            Debug.LogError($"[PlayerScore][{name}] model is null during AddScore!");
            return;
        }

        if (!isOwnedLocally)
        {
            Debug.Log($"[PlayerScore][{name}] Requesting ownership to add score.");
            model.RequestOwnership();
        }

        model.score += amount;
        Debug.Log($"[PlayerScore][{name}] Score incremented by {amount}. New score: {model.score}");
    }

    public void IncrementCombo()
    {
        if (model == null)
        {
            Debug.LogError($"[PlayerScore][{name}] model is null during IncrementCombo!");
            return;
        }

        model.combo = Mathf.Min(model.combo + 1, 2);
        Debug.Log($"[PlayerScore][{name}] Combo incremented. New combo: {model.combo}");
    }

    public void ResetCombo()
    {
        if (model == null)
        {
            Debug.LogError($"[PlayerScore][{name}] model is null during ResetCombo!");
            return;
        }

        model.combo = 0;
        Debug.Log($"[PlayerScore][{name}] Combo reset.");
    }

    public int GetCombo() => model != null ? model.combo : 0;
    public int GetScore() => model != null ? model.score : 0;

    protected override void OnRealtimeModelReplaced(PlayerScoreModel previousModel, PlayerScoreModel currentModel)
    {
        Debug.Log($"[PlayerScore][{name}] Model replaced.");

        if (previousModel != null)
            previousModel.scoreDidChange -= OnScoreChanged;

        if (currentModel != null)
            currentModel.scoreDidChange += OnScoreChanged;
    }

    private void OnScoreChanged(PlayerScoreModel model, int score)
    {
        Debug.Log($"[PlayerScore][{name}] Score updated remotely to: {score}");
    }

    public void ResetScore()
    {
        if (model == null)
        {
            Debug.LogError($"[PlayerScore][{name}] model is null during ResetScore!");
            return;
        }

        if (!isOwnedLocally)
        {
            Debug.Log($"[PlayerScore][{name}] Requesting ownership to reset score.");
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
        float timeout = 1f;
        float elapsed = 0f;

        while (!isOwnedLocally && elapsed < timeout)
        {
            yield return null;
            elapsed += Time.deltaTime;
        }

        if (isOwnedLocally)
        {
            ApplyReset();
        }
        else
        {
            Debug.LogWarning($"[PlayerScore][{name}] Ownership timeout. Could not reset.");
        }
    }

    private void ApplyReset()
    {
        model.score = 0;
        model.combo = 0;
        Debug.Log($"[PlayerScore][{name}] Score and combo reset.");
    }



}
