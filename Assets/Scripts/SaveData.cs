using System.IO;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int ClearStage = 0;
    // To do: 스킬 포인트
    // To do: 스킬 포인트 분배 정보
}

public class SaveData : MonoBehaviour
{
    string filePath;

    void Start()
    {
        filePath = Application.persistentDataPath + "/savefile.json";
    }

    public void SaveGame(GameData data)
    {
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(filePath, json);

        // To do: 저장 완료 UI 생성
    }

    public GameData LoadGame()
    {
        if(File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            return JsonUtility.FromJson<GameData>(json);
        }
        return new GameData();
    }
}
