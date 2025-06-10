using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

// Overrides the default XR grab throw behavior to compute a custom throw velocity
// based on recent hand movement, producing more realistic and tunable throws.
public class CustomThrowTuner : MonoBehaviour
{
    private Transform handTransform;
    public float throwForceMultiplier = 1.3f;

    private XRGrabInteractable grabInteractable;
    private readonly Queue<(Vector3 pos, float time)> positionHistory = new();
    public int positionSampleCount = 10;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.throwOnDetach = false;

        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    private void OnDestroy()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrab);
        grabInteractable.selectExited.RemoveListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        if (args.interactorObject is XRBaseInteractor interactor)
            handTransform = interactor.attachTransform;

        positionHistory.Clear();
    }


    private void OnRelease(SelectExitEventArgs args)
    {
        var rb = GetComponent<Rigidbody>();
        if (rb == null || handTransform == null)
            return;

        if (positionHistory.Count < 2)
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }

        var samples = new List<(Vector3 pos, float time)>(positionHistory);
        var first = samples[0];
        var last = samples[^1];

        float deltaTime = last.time - first.time;
        if (deltaTime <= 0f)
            deltaTime = Time.fixedDeltaTime;

        Vector3 velocity = (last.pos - first.pos) / deltaTime;
        velocity.y *= 0.75f;

        Vector3 handForward = handTransform.forward;
        float forwardFactor = Vector3.Dot(velocity.normalized, handForward);
        velocity += handForward * forwardFactor * 0.5f;
        velocity = Vector3.ClampMagnitude(velocity, 8f);
        velocity.y += 0.5f;
        velocity *= throwForceMultiplier;

        rb.linearVelocity = velocity;
        Debug.Log($"[CustomThrowTuner] Final throw velocity: {velocity}");
    }

    void LateUpdate()
    {
        if (handTransform != null)
        {
            positionHistory.Enqueue((handTransform.position, Time.time));
            if (positionHistory.Count > positionSampleCount)
                positionHistory.Dequeue();
        }
    }
}
