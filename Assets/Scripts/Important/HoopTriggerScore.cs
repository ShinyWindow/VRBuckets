using UnityEngine;
using System.Collections.Generic;
using Normal.Realtime;

public class HoopTriggerScorer : MonoBehaviour
{
    public Collider entryTrigger;
    public Collider exitTrigger;

    private readonly HashSet<Collider> validBalls = new();

    private void OnTriggerEnter(Collider other)
    {
        // This only triggers if you enter this object's collider — ignored in current setup
    }

    public void OnEntry(Collider ball)
    {
        if (IsValidBall(ball))
        {
            validBalls.Add(ball);
            Debug.Log($"[HoopTriggerScorer] Ball ENTERED entry trigger: {ball.name}");
        }
    }

    public void OnExit(Collider ballCollider)
    {
        if (!IsValidBall(ballCollider))
        {
            Debug.Log("[HoopTriggerScorer] Invalid ball exited — ignoring.");
            return;
        }

        if (!validBalls.Contains(ballCollider))
        {
            Debug.Log("[HoopTriggerScorer] Ball was not in entry list — not scoring.");
            return;
        }

        validBalls.Remove(ballCollider);

        var ball = ballCollider.GetComponent<Ball>();
        if (ball == null)
        {
            Debug.LogWarning("[HoopTriggerScorer] Ball component missing.");
            return;
        }

        Debug.Log("[HoopTriggerScorer] Valid ball exited — calling RegisterScore()");
        ball.RegisterScore();
    }

    private bool IsValidBall(Collider col)
    {
        bool isValid = col.CompareTag("Basketball") && col.GetComponent<RealtimeView>() != null;
        if (!isValid)
        {
            Debug.Log($"[HoopTriggerScorer] Rejected collider: {col.name}");
        }
        return isValid;
    }
}
