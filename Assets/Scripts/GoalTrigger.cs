using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
public class GoalTrigger : MonoBehaviour
{
    [Header("Which team's goal is this?")]
    [Tooltip("The team that DEFENDS this goal. The opposing team scores when the ball enters.")]
    public Team goalOwner;

    [Header("Ball Detection")]
    public string ballTag = "Ball";

    private bool scoredThisEntry;

    void Reset()
    {
        // Make sure the collider is a trigger when the component is added.
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (scoredThisEntry)
            return;

        if (!other.CompareTag(ballTag))
            return;

        scoredThisEntry = true;

        if (Score_Manager.Instance != null)
            Score_Manager.Instance.GoalScoredIn(goalOwner);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(ballTag))
            scoredThisEntry = false;
    }
}