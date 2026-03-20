using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("UI Panels")]
    public GameObject stageSelectPanel;
    public GameObject gameReadyPanel;
    public GameObject gamelosePanel;
    public GameObject gameClearPanel;
    public GameObject nextStageButton;
    public GameObject playerSkillPanel;

    public bool isGameActive = false;
    private bool isGameOver = false;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stageSelectPanel.SetActive(true);
        gameReadyPanel.SetActive(false);
        gamelosePanel.SetActive(false);
        gameClearPanel.SetActive(false);
        playerSkillPanel.SetActive(false);
    }

    public void StartGame()
    {
        isGameActive = true;
        isGameOver = false;
        Cursor.visible = false;
        gameReadyPanel.SetActive(false);
        playerSkillPanel.SetActive(false);

        SpawnManager spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        if(spawnManager != null )
        {
            spawnManager.InitStageInfo();
            spawnManager.StartSpawning();
        }

        PlayerController playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        if(playerController != null)
        {
            playerController.hpSlider.gameObject.SetActive(true);
        }
    }

    public void GameOver()
    {
        isGameActive = false;
        isGameOver = true;
        Cursor.visible = true;
        gamelosePanel.SetActive(true);

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(GameObject enemy in enemies)
        {
            Destroy(enemy);
        }
    }

    public void GameClear()
    {
        isGameActive = false;
        isGameOver = true;
        Cursor.visible = true;
        gameClearPanel.SetActive(true);

        StageManager stageManager = StageManager.instance;
        if(stageManager != null)
        {
            if(stageManager.GetCurrentStageNum() >= 20)
            {
                nextStageButton.gameObject.SetActive(false);
            }
        }
    }

    public void RestartGame()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(AutoStageSelect());
    }

    IEnumerator AutoStageSelect()
    {
        yield return new WaitForEndOfFrame();

        StageSelectManager stageSelectManager = GetComponent<StageSelectManager>();
        if (stageSelectManager != null)
        {
            SaveData saveData = SaveData.instance;
            if (saveData != null)
            {
                stageSelectManager.OnStageButtonClicked(saveData.stageNum);
            }
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void NextStage()
    {
        SaveData saveData = SaveData.instance;
        if(saveData != null )
        {
            saveData.stageNum++;

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void ToggleSkillPanel()
    {
        if(gameReadyPanel.activeSelf)
        {
            gameReadyPanel.SetActive(false);
            playerSkillPanel.SetActive(true);
        }
        else
        {
            gameReadyPanel.SetActive(true);
            playerSkillPanel.SetActive(false);
        }
    }

    public void GoToHome()
    {
        SceneManager.LoadScene("Home");
    }

    public void OpenStageSelectPanel()
    {
        if(isGameOver)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            gameReadyPanel.SetActive(false);
            gamelosePanel.SetActive(false);
            gameClearPanel.SetActive(false);
            stageSelectPanel.SetActive(true);

            StageSelectManager stageSelectManager = GetComponent<StageSelectManager>();
            if (stageSelectManager != null)
            {
                stageSelectManager.GenerateStageButtons();
            }
        }
    }

    public void OpenGameReadyPanel()
    {
        gameReadyPanel.SetActive(true);
        stageSelectPanel.SetActive(false);
    }
}
