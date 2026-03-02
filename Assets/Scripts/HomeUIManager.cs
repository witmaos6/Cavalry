using UnityEngine;
using UnityEngine.UI;

public class HomeUIManager : MonoBehaviour
{
    [Header("SaveFile")]
    public SaveData saveData;

    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject stageSelectPanel;

    [Header("Buttons")]
    public Button newGameButton;
    public Button continueGameButton;
    public Button backToMenu;
    public Button quitGame;

    private void Awake()
    {
        saveData = SaveData.instance;
    }

    void Start()
    {
        ShowMainMenu();

        newGameButton.onClick.AddListener(OnNewGameClicked);
        continueGameButton.onClick.AddListener(OnContinueGameClicked);

        backToMenu.onClick.AddListener(ShowMainMenu);
        quitGame.onClick.AddListener(QuitGame);
    }

    void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        stageSelectPanel.SetActive(false);
    }

    void OnNewGameClicked()
    {
        if(saveData != null)
        {
            saveData.CreateNewGame();
        }

        ShowStageSelect();
    }

    void OnContinueGameClicked()
    {
        GameData data = saveData.LoadGame();
        Debug.Log($"Load Scuccess 도달 스테이지: {data.clearStage} ");

        ShowStageSelect();
    }

    void ShowStageSelect()
    {
        mainMenuPanel.SetActive(false);
        stageSelectPanel.SetActive(true);

        StageSelectManager stageSelectManager = GetComponent<StageSelectManager>();
        if(stageSelectManager != null )
        {
            // stageSelectManager.GenerateStageButtons();
            // To do: 필요하면 추가
        }
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
