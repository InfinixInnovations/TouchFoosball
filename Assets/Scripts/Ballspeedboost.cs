using UnityEngine;

public class BallTravelBoost : MonoBehaviour
{
    public float travelMultiplier = 1.02f;
    public float maxSpeed = 25f;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (rb.linearVelocity.magnitude > 0.1f)
        {
            rb.linearVelocity *= travelMultiplier;
        }

        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity =
                rb.linearVelocity.normalized * maxSpeed;
        }
    }
}