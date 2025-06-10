using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using Normal.Realtime;

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
    private RealtimeTransform heldRt;
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
            if (heldRt == null) heldRt = heldRb.GetComponent<RealtimeTransform>();

            if (heldRt == null)
            {
                Debug.LogWarning("[Grabber] Held object has no RealtimeTransform.");
                return;
            }

            // NEW: Set kinematic state based on ownership
            heldRb.isKinematic = !heldRt.isOwnedLocallySelf;

            if (!heldRt.isOwnedLocallySelf)
            {
                Debug.Log("[Grabber] Skipping physics – not owned locally.");
                return;
            }

            Vector3 followVelocity = (attachPoint.position - heldRb.position) / Time.fixedDeltaTime;
            heldRb.linearVelocity = followVelocity;
            heldRb.angularVelocity = Vector3.zero;

            heldRb.transform.hasChanged = true;
            Debug.Log($"[Grabber] Applying follow velocity: {followVelocity}");
        }

    }

    void TryGrab()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, grabRadius, grabbableLayer);
        if (hits.Length > 0)
        {
            Rigidbody rbCandidate = hits[0].attachedRigidbody;
            if (rbCandidate != null)
            {
                RealtimeTransform rtCandidate = rbCandidate.GetComponent<RealtimeTransform>();

                if (rtCandidate == null)
                {
                    Debug.LogWarning("[Grabber] Object has no RealtimeTransform.");
                    return;
                }

                // Only grab if not already owned by another client
                if (!rtCandidate.isOwnedLocallySelf)
                    rtCandidate.RequestOwnership();

                // Wait to confirm ownership before grabbing
                if (!rtCandidate.isOwnedLocallySelf)
                {
                    Debug.Log("[Grabber] Waiting for ownership...");
                    return; // Do not proceed until ownership confirmed
                }

                heldRb = rbCandidate;
                heldRt = rtCandidate;

                // Now safe to modify Rigidbody
                heldRb.isKinematic = false;
                heldRb.position = attachPoint.position;
                heldRb.linearVelocity = Vector3.zero;
                heldRb.angularVelocity = Vector3.zero;

                Debug.Log("[Grabber] Object successfully grabbed with ownership.");
            }
        }
    }



    void Release()
    {
        if (heldRt == null || !heldRt.isOwnedLocallySelf)
        {
            Debug.Log("[Grabber] Releasing object but we don't own it.");
            heldRb = null;
            heldRt = null;
            positionHistory.Clear();
            return;
        }

        if (positionHistory.Count < 2)
        {
            heldRb.linearVelocity = Vector3.zero;
            Debug.Log("[Grabber] Too few samples, dropping ball with zero velocity.");
        }
        else
        {
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

            heldRb.linearVelocity = velocity;
            heldRb.transform.hasChanged = true;


            Debug.Log($"[Grabber] Throw velocity applied: {velocity} | Magnitude: {velocity.magnitude}");
        }

        heldRb = null;
        heldRt = null;
        positionHistory.Clear();
    }
}
