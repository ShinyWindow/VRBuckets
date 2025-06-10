using UnityEngine;

public class HoopEntry : MonoBehaviour
{
    public HoopTriggerScorer scorer;

    private void OnTriggerEnter(Collider other)
    {
        scorer.OnEntry(other);
    }
}
