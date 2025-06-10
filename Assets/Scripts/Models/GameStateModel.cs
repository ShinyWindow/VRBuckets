using Normal.Realtime.Serialization;

public enum GameMode
{
    FreePlay = 0,
    TimeAttack = 1
}

[RealtimeModel(createMetaModel: true)]
public partial class GameStateModel
{
    [RealtimeProperty(1, true, true)]
    private int _gameMode;

    [RealtimeProperty(2, true, true)]
    private bool _isGameOver; // ✅ Only need this now

    [RealtimeProperty(3, true, true)]
    private double _timeAttackStartTime; // ⏱️ Add this line
}
