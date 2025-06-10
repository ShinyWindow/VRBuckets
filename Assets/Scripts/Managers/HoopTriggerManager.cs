using UnityEngine;
using System.Collections.Generic;
using Normal.Realtime;

public class HoopTriggerManager: MonoBehaviour
{
    private readonly HashSet<Collider> validBalls = new();

    public void OnEntry(Collider ball)
    {
        if (IsValidBall(ball))
            validBalls.Add(ball);
    }

    public void OnExit(Collider ballCollider)
    {
        if (!IsValidBall(ballCollider) || !validBalls.Contains(ballCollider))
            return;

        validBalls.Remove(ballCollider);

        Ball ball = ballCollider.GetComponent<Ball>();
        if (ball != null)
            ball.RegisterScore();
    }

    private bool IsValidBall(Collider col)
    {
        return col.CompareTag("Basketball") && col.GetComponent<RealtimeView>() != null;
    }
}
