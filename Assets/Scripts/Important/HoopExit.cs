using UnityEngine;

public class HoopExit : MonoBehaviour
{
    public HoopTriggerScorer scorer;

    private void OnTriggerEnter(Collider other)
    {
        scorer.OnExit(other);
    }
}
