using NUnit.Framework;
using System.Collections;
using UnityEngine;



public class SpawnManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    public float spawnRate = 2.0f;
    public float spawnRange = 24.0f;

    private int totalWeight;
    private StageManager stageManager;
    private StageInfo currentStage;

    public int stageWave = 0;
    private int currentSpawnEnemy = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stageManager = GetComponent<StageManager>();
        if(stageManager != null)
        {
            // To do: 스테이지 선택하면 그 스테이지 정보를 가져오는 것으로 변경
            currentStage = stageManager.stageList[stageWave];
        }

        if(currentStage != null)
        {
            foreach (var data in currentStage.enemySpawnDatas)
            {
                totalWeight += data.weight;
            }
        }
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while(GameManager.Instance.isGameActive)
        {
            yield return new WaitForSeconds(spawnRate);

            if(currentSpawnEnemy < currentStage.waveDatas[stageWave].nrToSpawn)
            {
                int currentEnemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
                if (currentEnemyCount < currentStage.waveDatas[stageWave].spawnLimit)
                {
                    SpawnEnemy();
                }
            }
            else
            {
                break;
            }
        }
    }

    void SpawnEnemy()
    {
        if (currentStage.enemySpawnDatas.Length == 0)
            return;

        if (!GameManager.Instance.isGameActive)
            return;

        // 1. 가중치 기반으로 랜덤 인덱스 선택
        int randomNumber = Random.Range(0, totalWeight);
        GameObject selectedEnemy = null;

        int currentWeightSum = 0;
        foreach (var data in currentStage.enemySpawnDatas)
        {
            currentWeightSum += data.weight;
            if (randomNumber < currentWeightSum)
            {
                selectedEnemy = data.enemyPrefab;
                break;
            }
        }

        float spawnX = Random.Range(-spawnRange, spawnRange);
        float spawnY = Random.Range(-spawnRange, spawnRange);
        Vector3 spawnPos = new Vector3(spawnX, spawnY, 0f);

        Vector3 playerPos = GameObject.Find("Player").transform.position;
        while (Vector3.Distance(spawnPos, playerPos) < 5.0f)
        {
            spawnX = Random.Range(-spawnRange, spawnRange);
            spawnY = Random.Range(-spawnRange, spawnRange);
            spawnPos = new Vector3(spawnX, spawnY, 0f);
        }

        if (selectedEnemy != null)
        {
            Instantiate(selectedEnemy, spawnPos, Quaternion.identity);
            currentSpawnEnemy++;
        }
    }
}
