using System.Collections;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("Score UI")]
    public TMP_Text blueScoreText;
    public TMP_Text redScoreText;

    [Header("Goal UI")]
    public TMP_Text blueGoalText;
    public TMP_Text redGoalText;

    [Header("Ball")]
    public Rigidbody ballRb;
    public Transform ballSpawnPoint;

    private int blueScore;
    private int redScore;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateUI();

        if (blueGoalText != null)
            blueGoalText.gameObject.SetActive(false);

        if (redGoalText != null)
            redGoalText.gameObject.SetActive(false);
    }

    public void GoalScored(bool blueTeamScored)
    {
        StartCoroutine(GoalRoutine(blueTeamScored));
    }

    IEnumerator GoalRoutine(bool blueTeamScored)
    {
        // Stop ball
        ballRb.linearVelocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;

        if (blueTeamScored)
        {
            blueScore++;

            if (blueGoalText != null)
                blueGoalText.gameObject.SetActive(true);
        }
        else
        {
            redScore++;

            if (redGoalText != null)
                redGoalText.gameObject.SetActive(true);
        }

        UpdateUI();

        yield return new WaitForSeconds(2f);

        // Reset ball
        ballRb.transform.position = ballSpawnPoint.position;
        ballRb.linearVelocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;

        if (blueGoalText != null)
            blueGoalText.gameObject.SetActive(false);

        if (redGoalText != null)
            redGoalText.gameObject.SetActive(false);
    }

    void UpdateUI()
    {
        blueScoreText.text = blueScore.ToString();
        redScoreText.text = redScore.ToString();
    }
}