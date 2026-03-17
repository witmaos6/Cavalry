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

    public int waveNum = 0;
    private int currentSpawnEnemy = 0;

    public void InitStageInfo()
    {
        stageManager = GetComponent<StageManager>();
        if (stageManager != null)
        {
            currentStage = stageManager.GetCurrentStage();
        }

        if (currentStage != null)
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
        while(GameManager.instance.isGameActive)
        {
            yield return new WaitForSeconds(spawnRate);

            if(currentSpawnEnemy < currentStage.waveDatas[waveNum].nrToSpawn)
            {
                int currentEnemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
                if (currentEnemyCount < currentStage.waveDatas[waveNum].spawnLimit)
                {
                    SpawnEnemy();
                }
            }
            else
            {
                waveNum++;
                if (currentStage.waveDatas.Length > waveNum)
                {
                    Debug.Log($"{waveNum} wave clear");
                    // wave를 명확히 분리해야 한다면 추가 로직 필요
                }
                else
                {
                    StageSpawnEnd();
                    break;
                }
            }
        }
    }

    void SpawnEnemy()
    {
        if (currentStage.enemySpawnDatas.Length == 0)
            return;

        if (!GameManager.instance.isGameActive)
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

    void StageSpawnEnd()
    {
        InvokeRepeating("StageClearCheck", 1.0f, 1f);
    }

    void StageClearCheck()
    {
        PlayerController playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        if (playerController != null)
        {
            if(playerController.hp <= 0f)
            {
                CancelInvoke("StageClearCheck");
                return;
            }
        }

        int currentEnemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
        if(currentEnemyCount == 0)
        {
            SaveData saveData = SaveData.instance;
            if (saveData != null)
            {
                GameData gameData = saveData.LoadGame();
                if (gameData != null)
                {
                    if(gameData.clearStage == saveData.stageNum - 1)
                    {
                        if(gameData.clearStage < 20)
                        {
                            gameData.clearStage++;
                        }
                        saveData.SaveGame(gameData);
                    }
                }
            }
            GameManager gameManager = GameManager.instance;
            if (gameManager != null)
            {
                gameManager.GameClear();
            }

            CancelInvoke("StageClearCheck");
        }
    }
}
