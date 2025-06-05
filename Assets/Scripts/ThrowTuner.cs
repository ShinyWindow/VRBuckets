using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ThrowTuner : MonoBehaviour
{
    [Header("Throw Settings")]
    public float velocityMultiplier = 0.7f;
    public float maxSpeed = 10f;
    public float upwardBoost = 0.1f;
    public float forwardBoost = 1.2f;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void TuneThrow(Vector3 rawVelocity, Transform hand)
    {
        // Separate vertical and horizontal parts
        Vector3 horizontal = new Vector3(rawVelocity.x, 0f, rawVelocity.z);
        float horizontalSpeed = horizontal.magnitude;

        Vector3 direction = rawVelocity.normalized;

        // Scale only horizontal part of the velocity
        float boostedMagnitude = horizontalSpeed * forwardBoost + Mathf.Abs(rawVelocity.y);

        Vector3 adjusted = direction * boostedMagnitude;

        // Optional: add upward lift
        adjusted.y += upwardBoost;

        // Scale total speed
        adjusted *= velocityMultiplier;

        // Clamp final speed
        adjusted = Vector3.ClampMagnitude(adjusted, maxSpeed);

        rb.linearVelocity = adjusted;
    }


}
