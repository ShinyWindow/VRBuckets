using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ThrowTuner : MonoBehaviour
{
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void TuneThrow(Vector3 rawVelocity, Transform hand)
    {
        rb.linearVelocity = rawVelocity; // Pure raw hand velocity, no modification
    }
}
