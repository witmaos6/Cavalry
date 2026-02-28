using UnityEngine;
using UnityEngine.UI;

public class HomeUIManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject stageSelectPanel;

    [Header("Buttons")]
    public Button newGameButton;
    public Button continueGameButton;
    public Button backToMenu;
    public Button quitGame;

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
        ShowStageSelect();

        // To do: Savefile 생성
    }

    void OnContinueGameClicked()
    {
        ShowStageSelect();

        // To do: Savefile 불러오기
    }

    void ShowStageSelect()
    {
        mainMenuPanel.SetActive(false);
        stageSelectPanel.SetActive(true);
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
