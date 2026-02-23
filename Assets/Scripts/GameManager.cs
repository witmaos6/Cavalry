using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI Panels")]
    public Button gameStartButton;
    public Button gameRestartButton;

    [Header("Score Settings")]
    public TextMeshProUGUI scoreText;
    private int score;

    [Header("Timer Settings")] // To do: 임시기능이므로 추후에 삭제
    public TextMeshProUGUI timerText;
    public float timeRemaining = 120.0f;
    private bool timerIsRunning = false;

    public bool isGameActive = false;

    private void Awake()
    {
        Instance = this;
    }

    public void StartGame()
    {
        isGameActive = true;
        timerIsRunning = true;
        Cursor.visible = false;
        gameStartButton.gameObject.SetActive(false);

        SpawnManager spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        if(spawnManager != null )
        {
            spawnManager.StartSpawning();
        }

        PlayerController playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        if(playerController != null)
        {
            playerController.hpSlider.gameObject.SetActive(true);
        }

        timerText.gameObject.SetActive(true);
        scoreText.gameObject.SetActive(true);
        UpdateScoreUI();
        DisplayTime(timeRemaining);
    }

    public void GameOver()
    {
        isGameActive = false;
        timerIsRunning = false;
        Cursor.visible = true;
        gameRestartButton.gameObject.SetActive(true);

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(GameObject enemy in enemies)
        {
            Destroy(enemy);
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameRestartButton.gameObject.SetActive(false);
        score = 0;
        scoreText.gameObject.SetActive(false);
        if(timerText != null)
        {
            timerText.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                DisplayTime(timeRemaining);
            }
            else
            {
                timeRemaining = 0;
                timerIsRunning = false;
                GameOver();
            }
        }
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if(scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        if (timerText == null) return;

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        minutes = Mathf.Max(minutes, 0);

        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        seconds = Mathf.Max(seconds, 0);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
