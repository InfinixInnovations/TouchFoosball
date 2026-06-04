using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject startPanel;

    [Header("Ball")]
    public Rigidbody ballRb;

    private bool gameStarted = false;

    void Start()
    {
        gameStarted = false;

        startPanel.SetActive(true);

        // Freeze ball before game starts
        ballRb.isKinematic = true;
    }

    public void StartGame()
    {
        gameStarted = true;

        startPanel.SetActive(false);

        ballRb.isKinematic = false;
    }
}