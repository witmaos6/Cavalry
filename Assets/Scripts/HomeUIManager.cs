using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeUIManager : MonoBehaviour
{
    [Header("SaveFile")]
    public SaveData saveDataInstance;

    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject customKeyPanel;
    public GameObject guidePanel;
    public GameObject newGameGuidePanel;

    [Header("Buttons")]
    public Button guideOpenButton;
    public Button newGameButton;
    public Button createNewSaveFile;
    public Button cancelNewSaveFile;
    public Button continueGameButton;
    public Button keySettingButton;
    public Button goToHome;
    public Button goToHomeFromGuidePanel;
    public Button quitGame;

    private void Awake()
    {
        saveDataInstance = SaveData.instance;
    }

    void Start()
    {
        mainMenuPanel.SetActive(true);
        customKeyPanel.SetActive(false);
        guidePanel.SetActive(false);
        newGameGuidePanel.SetActive(false);

        guideOpenButton.onClick.AddListener(OnGuideOpenClicked);
        newGameButton.onClick.AddListener(OnNewGameClicked);
        createNewSaveFile.onClick.AddListener(OnCreateNewSaveFileClicked);
        cancelNewSaveFile.onClick.AddListener(OnCancelNewSaveFile);

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
        if(saveDataInstance != null)
        {
            if(saveDataInstance.ExistSaveFile())
            {
                newGameGuidePanel.SetActive(true);
            }
            else
            {
                saveDataInstance.CreateNewGame();
                ToMainLevel();
            }
        }
    }

    void OnCreateNewSaveFileClicked()
    {
        saveDataInstance.CreateNewGame();
        ToMainLevel();
    }

    void OnCancelNewSaveFile()
    {
        newGameGuidePanel.SetActive(false);
    }

    void OnContinueGameClicked()
    {
        GameData data = saveDataInstance.LoadGame();
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
