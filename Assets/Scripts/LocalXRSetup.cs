using UnityEngine;
using UnityEngine.SpatialTracking;
using UnityEngine.XR.Interaction.Toolkit;
using Normal.Realtime;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class LocalXRSetup : MonoBehaviour
{
    [Header("Camera Components")]
    public Camera playerCamera;
    public TrackedPoseDriver trackedPoseDriver;

    [Header("Movement Script")]
    public SimpleXRMovement simpleXRMovement;

    [Header("Interaction Components")]
    public XRDirectInteractor leftHandInteractor;
    public XRDirectInteractor rightHandInteractor;

    void Start()
    {
        var rt = GetComponent<RealtimeTransform>();
        bool isLocal = rt != null && rt.isOwnedLocallySelf;

        if (playerCamera != null) playerCamera.enabled = isLocal;
        if (trackedPoseDriver != null) trackedPoseDriver.enabled = isLocal;
        if (simpleXRMovement != null) simpleXRMovement.enabled = isLocal;

        // Setup XR Interaction if local
        if (isLocal)
        {
            XRInteractionManager manager = FindFirstObjectByType<XRInteractionManager>();

            if (leftHandInteractor != null)
            {
                leftHandInteractor.interactionManager = manager;
                leftHandInteractor.enabled = true;
            }

            if (rightHandInteractor != null)
            {
                rightHandInteractor.interactionManager = manager;
                rightHandInteractor.enabled = true;
            }
        }
    }
}
