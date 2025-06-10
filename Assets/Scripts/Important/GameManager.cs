using UnityEngine;
using Normal.Realtime;
using System;

public class GameManager : RealtimeComponent<GameStateModel>
{
    public static GameManager Instance { get; private set; }

    public bool IsGameOver => model != null && model.isGameOver;
    public double TimeAttackStartRoomTime => model != null ? model.timeAttackElapsedSeconds : 0;



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
        if (previousModel != null)
        {
            previousModel.gameModeDidChange -= OnGameModeChanged;
        }

        if (currentModel != null)
        {
            currentModel.gameModeDidChange += OnGameModeChanged;

            // Host initializes default mode
            if (realtime.clientID == 0)
            {
                currentModel.gameMode = (int)GameMode.FreePlay;
            }
        }
    }

    private void OnGameModeChanged(GameStateModel model, int newMode)
    {
        Debug.Log($"[GameManager] Game mode changed to: {(GameMode)newMode}");
        // You can update UI or logic here
    }

    private void OnWinnerChanged(GameStateModel model, string newWinner)
    {
        Debug.Log($"[GameManager] New winner set: {newWinner}");
        // You can update UI here too
    }

    public GameMode CurrentGameMode => model != null ? (GameMode)model.gameMode : GameMode.FreePlay;

    // Call this from a reset button
    public void ResetGameState()
    {
        if (realtime.clientID != 0 || model == null) return;

        model.isGameOver = false;
        model.timeAttackStartTime = 0; // 💥 Important: Reset the synced timer
        Debug.Log("[GameManager] Game state reset.");
    }



    public void SetGameMode(GameMode mode)
    {
        if (realtime.clientID != 0 || model == null) return;

        model.gameMode = (int)mode;
        Debug.Log($"[GameManager] Game mode set to {mode}");
    }

    public void SetWinner(string name)
    {
        if (realtime.clientID != 0 || model == null) return;

        Debug.Log($"[GameManager] Winner set to {name}");
    }

    public void SetGameOver(bool value)
    {
        if (realtime.clientID != 0 || model == null) return;

        model.isGameOver = value;
        Debug.Log($"[GameManager] Game over set to: {value}");
    }


    public void BeginTimeAttackIfNotStarted()
    {
        if (realtime.clientID == 0 && model != null && model.timeAttackElapsedSeconds == 0)
        {
            model.timeAttackElapsedSeconds = realtime.room.time;
            Debug.Log($"[GameManager] Time Attack started at room time: {model.timeAttackElapsedSeconds}");
        }
    }

    public void ResetTimeAttackStartTime()
    {
        if (realtime.clientID == 0 && model != null)
        {
            model.timeAttackElapsedSeconds = 0; // ✅ this is now used as startRoomTime
            Debug.Log("[GameManager] TimeAttackStartRoomTime reset.");
        }
    }



}
