using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI Panels")]
    public GameObject gameReadyPanel;
    public GameObject gameOverPanel;
    public GameObject gameClearPanel;

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
        gameReadyPanel.SetActive(false);

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
        gameOverPanel.gameObject.SetActive(true);

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(GameObject enemy in enemies)
        {
            Destroy(enemy);
        }
    }

    public void GameClear()
    {
        isGameActive = false;
        Cursor.visible = true;
        gameClearPanel.gameObject.SetActive(true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameReadyPanel.gameObject.SetActive(true);
        gameOverPanel.gameObject.SetActive(false);
        gameClearPanel.gameObject.SetActive(false);
        score = 0;
        scoreText.gameObject.SetActive(false);        
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

    public void NextStage()
    {
        Debug.Log("다음 스테이지로");
        // To do: 다음스테이지를 시작하는 기능 구현
    }

    public void StageSelect()
    {
        Debug.Log("스테이지 선택 화면으로 가는 기능 구현");
        // To do: 스테이지 선택 화면으로 가는 기능 구현
    }

}
