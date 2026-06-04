using UnityEngine;

public class FoosballBall : MonoBehaviour
{
    [Header("Ball Settings")]
    public float friction = 0.98f;
    public float maxSpeed = 15f;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Vector3 velocity = rb.linearVelocity;

        // keep ball on table
        velocity.y = 0;

        // gradual slowdown
        velocity *= friction;

        // clamp speed
        if (velocity.magnitude > maxSpeed)
        {
            velocity =
                velocity.normalized * maxSpeed;
        }

        rb.linearVelocity = velocity;
    }

    public void Kick(
        Vector3 direction,
        float strength
    )
    {
        rb.linearVelocity =
            direction.normalized * strength;
    }
}