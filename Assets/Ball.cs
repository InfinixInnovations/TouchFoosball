using UnityEngine;

public class BallController : MonoBehaviour
{
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Prevent flying
        Vector3 vel = rb.linearVelocity;

        vel.y = 0;

        rb.linearVelocity = vel;

        // Keep on table
        Vector3 pos = transform.position;

        pos.y = 0.25f;

        transform.position = pos;
    }
}