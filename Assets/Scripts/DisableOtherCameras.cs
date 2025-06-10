using UnityEngine;
using Normal.Realtime;

public class DisableOtherCameras : MonoBehaviour
{
    void Start()
    {
        // Optional: Only continue if this player owns this rig
        var rt = GetComponentInParent<RealtimeTransform>();
        if (rt != null && !rt.isOwnedLocallySelf) return;

        Camera thisCamera = GetComponent<Camera>();

        // Get all cameras in the scene (even inactive ones)
        var allCameras = FindObjectsByType<Camera>(FindObjectsSortMode.None);

        foreach (Camera cam in allCameras)
        {
            if (cam != thisCamera)
            {
                cam.enabled = false;
            }
        }
    }
}
