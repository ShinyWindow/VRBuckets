using Normal.Realtime;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class RequestOwnership : MonoBehaviour
{
    [SerializeField] private RealtimeView realtimeView;
    [SerializeField] private RealtimeTransform realtimeTransform;
    [SerializeField] private XRGrabInteractable XRGrabInteractable;

    private void OnEnable()
    {
        XRGrabInteractable.selectEntered.AddListener(RequestObjectOwnership);
    }

    private void RequestObjectOwnership(SelectEnterEventArgs args)
    {
        realtimeView.RequestOwnership();
        realtimeTransform.RequestOwnership();
    }

    private void OnDisable()
    {
        XRGrabInteractable.selectEntered.RemoveListener(RequestObjectOwnership);
    }

}
