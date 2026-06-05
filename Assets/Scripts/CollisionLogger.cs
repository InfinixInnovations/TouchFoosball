using UnityEngine;

public class CollisionLogger : MonoBehaviour
{
    [Tooltip("Log physical collisions (non-trigger colliders).")]
    public bool logCollisions = true;

    [Tooltip("Log trigger overlaps (colliders marked Is Trigger).")]
    public bool logTriggers = true;

    // --- Physical collisions (requires a Rigidbody on this or the other object) ---

    void OnCollisionEnter(Collision collision)
    {
        if (logCollisions)
            Debug.Log(name + " collided with: " + collision.gameObject.name, this);
    }

    // --- Trigger overlaps (one collider must have Is Trigger checked) ---

    void OnTriggerEnter(Collider other)
    {
        if (logTriggers)
            Debug.Log(name + " triggered by: " + other.gameObject.name, this);
    }
}