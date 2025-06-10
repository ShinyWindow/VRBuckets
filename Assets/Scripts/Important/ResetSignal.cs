using Normal.Realtime;
using UnityEngine;

public class ResetSignal : RealtimeComponent<ResetSignalModel>
{
    public ResetSignalModel SignalModel => model; // ✅ make model accessible publicly

    void Start()
    {
        if (model == null)
            Debug.LogError("[ResetSignal] Model is null! Make sure 'Is Scene View' is checked on the RealtimeView.");
        else
            Debug.Log("[ResetSignal] Model ready.");
    }


    protected override void OnRealtimeModelReplaced(ResetSignalModel previousModel, ResetSignalModel currentModel)
    {
        if (previousModel != null)
        {
            // Unsubscribe from events if needed
        }

        if (currentModel != null)
        {
            Debug.Log("[ResetSignal] Model replaced.");
        }
    }
}
