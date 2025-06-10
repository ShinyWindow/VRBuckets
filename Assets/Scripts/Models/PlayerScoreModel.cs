using Normal.Realtime.Serialization;

[RealtimeModel(createMetaModel: true)]
public partial class PlayerScoreModel
{
    [RealtimeProperty(1, true, true)]
    private int _score;

    [RealtimeProperty(2, true, true)]
    private int _combo;

    [RealtimeProperty(3, true, true)]
    private bool _lastShotScored;
}
