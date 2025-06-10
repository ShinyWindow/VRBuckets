using UnityEngine;

public class HoopTrigger : MonoBehaviour
{
    public enum TriggerType { Entry, Exit }
    public TriggerType triggerType;

    public HoopTriggerManager hoopManager;

    private void OnTriggerEnter(Collider other)
    {
        if (triggerType == TriggerType.Entry)
            hoopManager.OnEntry(other);
        else if (triggerType == TriggerType.Exit)
            hoopManager.OnExit(other);
    }
}
