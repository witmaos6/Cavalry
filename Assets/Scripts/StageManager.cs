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
}
