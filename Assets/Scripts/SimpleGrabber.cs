using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class SimpleGrabber : MonoBehaviour
{
    [Header("Grab Settings")]
    public float grabRadius = 0.1f;
    public LayerMask grabbableLayer;
    public InputActionProperty grabAction;

    private Rigidbody heldRb;
    private Transform attachPoint;
    private Queue<Vector3> velocityHistory = new();
    private int velocitySampleCount = 5;

    void Start()
    {
        grabAction.action.Enable();

        attachPoint = new GameObject("AttachPoint").transform;
        attachPoint.SetParent(transform);
        attachPoint.localPosition = Vector3.zero;
    }

    void FixedUpdate()
    {
        bool isGrabbing = grabAction.action.ReadValue<float>() > 0.5f;

        if (isGrabbing && heldRb == null)
            TryGrab();
        else if (!isGrabbing && heldRb != null)
            Release();

        if (heldRb != null)
        {
            // Record velocity history for throw calculation
            velocityHistory.Enqueue((attachPoint.position - heldRb.position) / Time.fixedDeltaTime);
            if (velocityHistory.Count > velocitySampleCount)
                velocityHistory.Dequeue();

            // Simple physics follow
            Vector3 targetVel = (attachPoint.position - heldRb.position) * 50f;
            heldRb.linearVelocity = targetVel;
        }
    }

    void TryGrab()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, grabRadius, grabbableLayer);
        if (hits.Length > 0)
        {
            heldRb = hits[0].attachedRigidbody;
            if (heldRb != null)
                heldRb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }
    }

    void Release()
    {
        Vector3 avgVelocity = Vector3.zero;
        foreach (var vel in velocityHistory)
            avgVelocity += vel;
        avgVelocity /= Mathf.Max(velocityHistory.Count, 1);

        ThrowTuner tuner = heldRb.GetComponent<ThrowTuner>();
        if (tuner != null)
            tuner.TuneThrow(avgVelocity, transform);
        else
            heldRb.linearVelocity = avgVelocity;

        heldRb = null;
        velocityHistory.Clear();
    }
}
