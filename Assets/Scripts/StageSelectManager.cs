using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageSelectManager : MonoBehaviour
{
    public SaveData saveDataProcessor;
    public GameObject stageButtonPrefab;
    public Transform contentTransform;

    private int maxTotalStage = 20;

    private void OnEnable()
    {
        GenerateStageButtons();
    }

    public void GenerateStageButtons()
    {
        foreach(Transform child in contentTransform)
        {
            Destroy(child.gameObject);
        }

        GameData data = saveDataProcessor.LoadGame();
        int availableStages = Mathf.Min(data.clearStage + 1, maxTotalStage);

        for(int i = availableStages; i >= 1; i--)
        {
            GameObject stageButton = Instantiate(stageButtonPrefab, contentTransform);

            TMP_Text buttonText = stageButton.GetComponentInChildren<TMP_Text>();
            if(buttonText != null )
            {
                buttonText.text = $"STAGE {i}";
            }

            int stageIndex = i;

            Button buttonComp = stageButton.GetComponent<Button>();
            if( buttonComp != null )
            {
                buttonComp.onClick.AddListener(() => OnStageButtonClicked(stageIndex));
            }
        }
    }

    void OnStageButtonClicked(int stageNum)
    {
        SaveData saveData = SaveData.instance;
        if (saveData != null)
        {
            saveData.stageNum = stageNum;
            SceneManager.LoadScene("Main");
        }
        else
        {
            Debug.Log("SaveData is null");
        }
    }
}
