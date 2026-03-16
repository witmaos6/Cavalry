using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public enum SkillID
    {
        Guard, Reflection, Dummy, Dash, // Active
        MultipleShot, OnemoreTimeShot // Passive
    } 

    public int clearStage = 0;
    public int totalSkillPoint = 3; // To do: 임시 포인트
    public int allocatedPoint = 0;
    public int remainPoint = 3;
    public List<SkillID> skillSet = new List<SkillID>();
}

public class SaveData : MonoBehaviour
{
    public static SaveData instance;
    string filePath;
    public int stageNum = 0;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        
        DontDestroyOnLoad(gameObject);
        filePath = Application.persistentDataPath + "/savefile.json";
    }

    public void SaveGame(GameData data)
    {
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(filePath, json);
        Debug.Log("게임 저장 완료: " + json);
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

    public void CreateNewGame()
    {
        GameData newData = new GameData();
        SaveGame(newData);
    }
}
