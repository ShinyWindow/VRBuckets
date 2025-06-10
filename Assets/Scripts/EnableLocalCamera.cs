using UnityEngine;
using Normal.Realtime;

public class EnableLocalCamera : MonoBehaviour
{
    void Start()
    {
        var rt = GetComponentInParent<RealtimeTransform>();
        if (rt != null && rt.isOwnedLocallySelf)
        {
            GetComponent<Camera>().enabled = true;
        }
    }
}
