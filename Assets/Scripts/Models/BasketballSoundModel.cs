using Normal.Realtime.Serialization;
[RealtimeModel(createMetaModel: true)]
public partial class BasketballSoundModel
{
    [RealtimeProperty(4, true, true)]
    private int _bounceAudioTrigger;
}
