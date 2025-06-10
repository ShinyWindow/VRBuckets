using Normal.Realtime;
using UnityEngine;

// Exposes the ResetSignalModel so other scripts can read or modify synced reset data.
public class ResetSignal : RealtimeComponent<ResetSignalModel>
{
    public ResetSignalModel SignalModel => model;
}
