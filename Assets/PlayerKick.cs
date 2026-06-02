using UnityEngine;

public class PlayerKick : MonoBehaviour
{
    [Header("Kick Settings")]
    public float kickMultiplier = 1.5f;
    public float maxKickSpeed = 50f;
    public float minSpinSpeed = 25f;

    private RodController rod;

    void Start()
    {
        rod = GetComponentInParent<RodController>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Ball"))
            return;

        if (rod == null)
            return;

        Rigidbody ballRb = collision.rigidbody;

        if (ballRb == null)
            return;

        float spinSpeed = rod.CurrentSpinSpeed;

        if (spinSpeed < minSpinSpeed)
            return;

        Vector3 kickDirection =
            (collision.transform.position - transform.position).normalized;

        float kickSpeed =
            Mathf.Clamp(
                spinSpeed * kickMultiplier,
                0f,
                maxKickSpeed
            );

        // Add speed to existing velocity
        ballRb.linearVelocity += kickDirection * kickSpeed;

        Debug.Log(
            $"Kick Speed: {kickSpeed:F2} | Rod Spin: {spinSpeed:F2}"
        );
    }
}