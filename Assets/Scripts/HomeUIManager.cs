using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeUIManager : MonoBehaviour
{
    [Header("SaveFile")]
    public SaveData saveData;

    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject customKeyPanel;
    public GameObject guidePanel;

    [Header("Buttons")]
    public Button guideOpenButton;
    public Button newGameButton;
    public Button continueGameButton;
    public Button keySettingButton;
    public Button goToHome;
    public Button goToHomeFromGuidePanel;
    public Button quitGame;

    private void Awake()
    {
        saveData = SaveData.instance;
    }

    void Start()
    {
        mainMenuPanel.SetActive(true);
        customKeyPanel.SetActive(false);
        guidePanel.SetActive(false);

        guideOpenButton.onClick.AddListener(OnGuideOpenClicked);
        newGameButton.onClick.AddListener(OnNewGameClicked);
        continueGameButton.onClick.AddListener(OnContinueGameClicked);
        keySettingButton.onClick.AddListener(ToggleCustomKeyPanel);
        goToHome.onClick.AddListener(ToggleMainMenuPanel);
        goToHomeFromGuidePanel.onClick.AddListener(ToggleMainMenuPanel);

        quitGame.onClick.AddListener(QuitGame);
    }

    void OnGuideOpenClicked()
    {
        mainMenuPanel.SetActive(false);
        guidePanel.SetActive(true);
    }

    void OnNewGameClicked()
    {
        if(saveData != null)
        {
            saveData.CreateNewGame();
        }

        ToMainLevel();
    }

    void OnContinueGameClicked()
    {
        GameData data = saveData.LoadGame();
        Debug.Log($"Load Scuccess 도달 스테이지: {data.clearStage} ");

        ToMainLevel();
    }

    void ToggleCustomKeyPanel()
    {
        mainMenuPanel.SetActive(false);
        customKeyPanel.SetActive(true);
    }

    void ToggleMainMenuPanel()
    {
        mainMenuPanel.SetActive(true);
        customKeyPanel.SetActive(false);
        guidePanel.SetActive(false);
    }

    void ToMainLevel()
    {
        SceneManager.LoadScene("Main");
    }

    void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
