using UnityEngine;
using Normal.Realtime;

[RequireComponent(typeof(RealtimeTransform), typeof(Rigidbody), typeof(AudioSource))]
public class NetworkedBasketball : MonoBehaviour
{
    [Header("Bounce Sound Settings")]
    public AudioClip[] bounceSounds;
    public float minVelocity = 1.5f;
    public float cooldownTime = 0.1f;

    private Rigidbody _rb;
    private RealtimeTransform _rt;
    private AudioSource _audioSource;
    private float _lastSoundTime;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rt = GetComponent<RealtimeTransform>();
        _audioSource = GetComponent<AudioSource>();
    }

    void FixedUpdate()
    {
        // Only simulate physics if we own it
        _rb.isKinematic = !_rt.isOwnedLocallySelf;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!_rt.isOwnedLocallySelf) return;

        // Don't play sounds too frequently
        if (Time.time - _lastSoundTime < cooldownTime)
            return;

        float impact = collision.relativeVelocity.magnitude;

        if (impact > minVelocity && bounceSounds.Length > 0)
        {
            _lastSoundTime = Time.time;

            // Pick a random clip
            AudioClip clip = bounceSounds[Random.Range(0, bounceSounds.Length)];

            // Add slight variation
            _audioSource.pitch = Random.Range(0.95f, 1.05f);
            _audioSource.volume = Mathf.Clamp01(impact / 6f);

            _audioSource.PlayOneShot(clip);
        }
    }
}
