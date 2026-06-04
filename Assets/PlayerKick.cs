using UnityEngine;

public class GoalKick : MonoBehaviour
{
    [Header("Goal Target")]
    public Transform targetGoal;

    [Header("Kick Settings")]
    public float kickSpeed = 12f;

    [Range(0f, 1f)]
    public float goalInfluence = 0.8f;

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Ball"))
            return;

        Rigidbody ballRb = collision.rigidbody;

        if (ballRb == null)
            return;

        // Actual hit direction
        Vector3 hitDirection =
            (collision.transform.position -
             transform.position).normalized;

        // Goal direction
        Vector3 goalDirection =
            (targetGoal.position -
             collision.transform.position).normalized;

        // Blend toward goal
        Vector3 finalDirection =
            Vector3.Lerp(
                hitDirection,
                goalDirection,
                goalInfluence
            ).normalized;

        ballRb.linearVelocity =
            finalDirection * kickSpeed;
    }
}