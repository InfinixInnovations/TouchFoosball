using UnityEngine;

public class BallSpeedBoost : MonoBehaviour
{
    [Header("Speed Settings")]
    public float speedMultiplier = 1.02f;
    public float maxSpeed = 20f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float speed = rb.linearVelocity.magnitude;

        if (speed > 0.1f)
        {
            rb.linearVelocity *= speedMultiplier;
        }

        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity =
                rb.linearVelocity.normalized * maxSpeed;
        }
    }
}