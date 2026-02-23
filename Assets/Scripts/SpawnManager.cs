using NUnit.Framework;
using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
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

    [Header("Spawn Settings")]
    public EnemySpawnData[] enemySpawnList;
    public float spawnRate = 2.0f;
    public float spawnRange = 24.0f;
    public int maxEnemyCount = 10;

    private int totalWeight;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach(var data in enemySpawnList)
        {
            totalWeight += data.weight;
        }
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator SpawnRoutine()
    {
        while(GameManager.Instance.isGameActive)
        {
            yield return new WaitForSeconds(spawnRate);

            int currentEnemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
            if(currentEnemyCount < maxEnemyCount)
            {
                SpawnEnemy();
            }
        }
    }

    void SpawnEnemy()
    {
        if (enemySpawnList.Length == 0)
            return;

        if (!GameManager.Instance.isGameActive)
            return;

        // 1. 가중치 기반으로 랜덤 인덱스 선택
        int randomNumber = Random.Range(0, totalWeight);
        GameObject selectedEnemy = null;

        int currentWeightSum = 0;
        foreach (var data in enemySpawnList)
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
        }
    }
}
