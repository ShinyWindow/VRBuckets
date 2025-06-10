using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class HapticOnGrab : MonoBehaviour
{
    public float amplitude = 0.5f;
    public float duration = 0.1f;

    XRGrabInteractable grabInteractable;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
    }

    void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(HandleGrab);
    }

    void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(HandleGrab);
    }

    void HandleGrab(SelectEnterEventArgs args)
    {
        // Try to pull the controller component
        if (args.interactorObject is UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInputInteractor baseInteractor)
        {
            baseInteractor.xrController?.SendHapticImpulse(amplitude, duration);
        }
        else
        {
            Debug.LogWarning("Interactor does not support haptics");
        }
    }
}
