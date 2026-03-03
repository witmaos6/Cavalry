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
    public StageInfo[] stageList;
    private int currentStageNum = 0;

    private void Awake()
    {
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
            // 임시 기능
            return stageList[0];
        }
        return stageList[currentStageNum - 1];
    }
}
