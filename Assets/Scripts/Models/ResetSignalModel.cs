using Normal.Realtime.Serialization;

[RealtimeModel(createMetaModel: true)]
public partial class ResetSignalModel {
    [RealtimeProperty(3, true, true)]
    private int _resetCounter;
}
