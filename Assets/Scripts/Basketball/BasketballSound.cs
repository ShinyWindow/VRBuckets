using UnityEngine;

// Plays randomized bounce sounds when the basketball hits something with enough force,
// adding audio variation and feedback based on impact velocity.
[RequireComponent(typeof(AudioSource), typeof(Rigidbody))]
public class BasketballSound : MonoBehaviour
{
    public AudioClip[] bounceSounds;
    public float minVelocity = 1.5f;
    public float cooldownTime = 0.1f;

    private AudioSource audioSource;
    private Rigidbody rb;
    private float lastSoundTime;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        // Don't play sounds too frequently
        if (Time.time - lastSoundTime < cooldownTime)
            return;

        float impact = collision.relativeVelocity.magnitude;

        if (impact > minVelocity)
        {
            lastSoundTime = Time.time;

            // Choose a random bounce sound
            AudioClip clip = bounceSounds[Random.Range(0, bounceSounds.Length)];

            // Slight pitch and volume variation
            audioSource.pitch = Random.Range(0.95f, 1.05f);
            audioSource.volume = Mathf.Clamp01(impact / 6f); // Normalize impact

            audioSource.PlayOneShot(clip);
        }
    }
}
