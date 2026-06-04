using UnityEngine;

public class BallMinimumSpeed : MonoBehaviour
{
    public float minimumSpeed = 2f;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Vector3 vel = rb.linearVelocity;

        if (vel.magnitude < minimumSpeed)
        {
            if (vel.magnitude < 0.01f)
            {
                vel = Random.insideUnitSphere;
                vel.y = 0;
                vel.Normalize();
            }

            rb.linearVelocity =
                vel.normalized * minimumSpeed;
        }
    }
}