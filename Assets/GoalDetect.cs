using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    public bool blueTeamScores;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Ball"))
            return;

        ScoreManager.Instance.GoalScored(
            blueTeamScores
        );
    }
}