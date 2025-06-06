using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class SimpleGrabber : MonoBehaviour
{
    [Header("Grab Settings")]
    public float grabRadius = 0.1f;
    public LayerMask grabbableLayer;
    public InputActionProperty grabAction;

    [Header("Velocity Tracking")]
    public Transform handTransform; // Assign your controller or hand transform
    private readonly Queue<(Vector3 pos, float time)> positionHistory = new();
    public int positionSampleCount = 10;

    private Rigidbody heldRb;
    private Transform attachPoint;

    [Range(0.1f, 3f)]
    public float throwForceMultiplier = 1.3f;


    void Start()
    {
        grabAction.action.Enable();

        attachPoint = new GameObject("AttachPoint").transform;
        attachPoint.SetParent(transform);
        attachPoint.localPosition = Vector3.zero;
        attachPoint.localRotation = Quaternion.identity;
    }

    void LateUpdate()
    {
        if (heldRb != null)
        {
            // Track hand position + time
            positionHistory.Enqueue((handTransform.position, Time.time));
            if (positionHistory.Count > positionSampleCount)
                positionHistory.Dequeue();
        }
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
            Vector3 followVelocity = (attachPoint.position - heldRb.position) / Time.fixedDeltaTime;
            heldRb.linearVelocity = followVelocity;
            heldRb.angularVelocity = Vector3.zero;
        }
    }

    void TryGrab()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, grabRadius, grabbableLayer);
        if (hits.Length > 0)
        {
            heldRb = hits[0].attachedRigidbody;
            if (heldRb != null)
            {
                heldRb.collisionDetectionMode = CollisionDetectionMode.Continuous;
                heldRb.interpolation = RigidbodyInterpolation.Interpolate;
                heldRb.useGravity = true;
                positionHistory.Clear();
            }
        }
    }

    void Release()
    {
        if (positionHistory.Count < 2)
        {
            heldRb.linearVelocity = Vector3.zero;
        }
        else
        {
            var samples = new List<(Vector3 pos, float time)>(positionHistory);
            var first = samples[0];
            var last = samples[^1];

            float deltaTime = last.time - first.time;
            if (deltaTime <= 0f)
                deltaTime = Time.fixedDeltaTime; // fallback

            Vector3 velocity = (last.pos - first.pos) / deltaTime;

            // Apply vertical dampening
            velocity.y *= 0.75f;

            // Apply forward boost based on hand direction
            Vector3 handForward = handTransform.forward;
            float forwardFactor = Vector3.Dot(velocity.normalized, handForward);
            velocity += handForward * forwardFactor * 0.5f; // Tune this multiplier

            // Clamp to max throw speed
            velocity = Vector3.ClampMagnitude(velocity, 8f);

            // Add subtle lift
            velocity.y += 0.5f;

            // Apply user-defined multiplier
            velocity *= throwForceMultiplier;

            Debug.Log($"Throw velocity: {velocity} | Magnitude: {velocity.magnitude}");

            heldRb.linearVelocity = velocity;

        }

        heldRb = null;
        positionHistory.Clear();
    }

}
