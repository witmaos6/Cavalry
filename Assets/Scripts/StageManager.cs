using NUnit.Framework;
using UnityEngine;

[System.Serializable]
public struct EnemySpawnData
{
    public EnemySpawnData(GameObject enemy, int inWeight)
    {
        enemyPrefab = enemy;
        weight = inWeight;
    }
    public GameObject enemyPrefab;
    public int weight;
}

[System.Serializable]
public class WaveData
{
    public int nrToSpawn = 10;
    public int spawnLimit = 5;
}

[System.Serializable]
public class StageInfo
{
    public EnemySpawnData[] enemySpawnDatas;

    public WaveData[] waveDatas;
}

public class StageManager : MonoBehaviour
{
    public static StageManager instance;
    public StageInfo[] stageList;
    private int currentStageNum = 0;
    public int testStage = 0;

    private void Awake()
    {
        if(instance == null)
        {
            instance = GetComponent<StageManager>();
        }

        SaveData saveData = SaveData.instance;
        if(saveData != null )
        {
            currentStageNum = saveData.stageNum;
        }
    }

    public StageInfo GetCurrentStage()
    {
        Debug.Log($"{currentStageNum} Stage Start");
        if(currentStageNum == 0)
        {
            return stageList[testStage - 1];
        }
        return stageList[currentStageNum - 1];
    }

    public int GetCurrentStageNum()
    {
        if(currentStageNum == 0)
        {
            return testStage;
        }
        return currentStageNum;
    }
}
