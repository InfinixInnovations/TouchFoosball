using UnityEngine;

public class PlayerKick : MonoBehaviour
{
    public float kickStrength = 10f;

    private void OnCollisionEnter(Collision collision)
    {
        FoosballBall ball =
            collision.gameObject.GetComponent<FoosballBall>();

        if (ball == null)
            return;

        Vector3 dir =
            (collision.transform.position -
             transform.position).normalized;

        ball.Kick(
            dir,
            kickStrength
        );
    }
}