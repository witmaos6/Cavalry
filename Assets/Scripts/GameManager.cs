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

    public bool isGameActive = false;

    private void Awake()
    {
        Instance = this;
    }

    public void StartGame()
    {
        isGameActive = true;
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

        scoreText.gameObject.SetActive(true);
        UpdateScoreUI();
    }

    public void GameOver()
    {
        isGameActive = false;
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
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
