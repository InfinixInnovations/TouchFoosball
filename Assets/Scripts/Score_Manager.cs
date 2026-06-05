using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Score_Manager : MonoBehaviour
{
    public static Score_Manager Instance { get; private set; }

    [Header("Score UI")]
    public TMP_Text blueScoreText;
    public TMP_Text redScoreText;

    [Header("Announcement (optional)")]
    public TMP_Text announcementText;   // flashes "GOAL!" then clears
    public float announcementDuration = 1.5f;

    [Header("Team Colors")]
    public Color blueColor = new Color(0.2f, 0.5f, 1f);
    public Color redColor = new Color(1f, 0.25f, 0.25f);
    public Color neutralColor = Color.white;

    [Header("Match Settings")]
    public int scoreToWin = 4;
    public bool stopOnWin = true;

    [Header("Ball Reset (optional)")]
    public Transform ball;              // the ball Rigidbody/Transform
    public Transform ballSpawnPoint;    // where to drop it after a goal
    public float resetDelay = 1f;

    private int blueScore;
    private int redScore;
    private bool matchOver;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        UpdateUI();
        if (announcementText != null)
            announcementText.text = "";
    }

    // Called by GoalTrigger when the ball enters a goal.
    // Pass the team that OWNS the goal that was scored in;
    // the OTHER team gets the point.
    public void GoalScoredIn(Team scoredGoalOwner)
    {
        if (matchOver)
            return;

        Team scoringTeam;
        if (scoredGoalOwner == Team.Blue)
        {
            redScore++;     // ball went into Blue's goal -> Red scores
            scoringTeam = Team.Red;
        }
        else
        {
            blueScore++;    // ball went into Red's goal -> Blue scores
            scoringTeam = Team.Blue;
        }

        UpdateUI();
        ShowAnnouncement("GOAL!", ColorFor(scoringTeam));

        if (stopOnWin && (blueScore >= scoreToWin || redScore >= scoreToWin))
        {
            matchOver = true;
            Team winner = blueScore >= scoreToWin ? Team.Blue : Team.Red;
            ShowAnnouncement(winner + " WINS!", ColorFor(winner), true);
        }
        else
        {
            StartCoroutine(ResetBallAfterDelay());
        }
    }

    private void UpdateUI()
    {
        if (blueScoreText != null) blueScoreText.text = blueScore.ToString();
        if (redScoreText != null) redScoreText.text = redScore.ToString();
    }

    private Color ColorFor(Team team)
    {
        return team == Team.Blue ? blueColor : redColor;
    }

    private void ShowAnnouncement(string message, Color color, bool persistent = false)
    {
        if (announcementText == null)
            return;

        StopCoroutine(nameof(ClearAnnouncement));
        announcementText.text = message;
        announcementText.color = color;

        if (!persistent)
            StartCoroutine(ClearAnnouncement());
    }

    private IEnumerator ClearAnnouncement()
    {
        yield return new WaitForSeconds(announcementDuration);
        if (announcementText != null)
            announcementText.text = "";
    }

    private IEnumerator ResetBallAfterDelay()
    {
        yield return new WaitForSeconds(resetDelay);

        if (ball == null || ballSpawnPoint == null)
            yield break;

        Rigidbody rb = ball.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        ball.position = ballSpawnPoint.position;
        ball.rotation = ballSpawnPoint.rotation;
    }

    public void ResetMatch()
    {
        blueScore = 0;
        redScore = 0;
        matchOver = false;
        UpdateUI();
        if (announcementText != null)
            announcementText.text = "";
        StartCoroutine(ResetBallAfterDelay());
    }
}

public enum Team
{
    Blue,
    Red
}