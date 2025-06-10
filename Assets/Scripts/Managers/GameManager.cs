using UnityEngine;
using Normal.Realtime;

// Manages the overall game state including current mode, timer, and game over status.
// Ensures synchronization across the network using Normcore's realtime model.
public class GameManager : RealtimeComponent<GameStateModel>
{
    public static GameManager Instance { get; private set; }

    public bool IsGameOver => model != null && model.isGameOver;
    public double TimeAttackStartRoomTime => model != null ? model.timeAttackElapsedSeconds : 0;
    public GameMode CurrentGameMode => model != null ? (GameMode)model.gameMode : GameMode.FreePlay;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    protected override void OnRealtimeModelReplaced(GameStateModel previousModel, GameStateModel currentModel)
    {
        if (realtime.clientID == 0 && currentModel != null)
            currentModel.gameMode = (int)GameMode.FreePlay;
    }

    public void ResetGameState()
    {
        if (realtime.clientID != 0 || model == null) return;

        model.isGameOver = false;
        model.timeAttackElapsedSeconds = 0;
    }

    public void SetGameMode(GameMode mode)
    {
        if (realtime.clientID != 0 || model == null) return;

        model.gameMode = (int)mode;
    }

    public void SetGameOver(bool value)
    {
        if (realtime.clientID != 0 || model == null) return;

        model.isGameOver = value;
    }

    public void BeginTimeAttackIfNotStarted()
    {
        if (realtime.clientID == 0 && model != null && model.timeAttackElapsedSeconds == 0)
            model.timeAttackElapsedSeconds = realtime.room.time;
    }

    public void ResetTimeAttackStartTime()
    {
        if (realtime.clientID == 0 && model != null)
            model.timeAttackElapsedSeconds = 0;
    }
}
