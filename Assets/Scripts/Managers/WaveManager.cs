using DesignPatterns.Generics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class WaveManager : Singleton<WaveManager>
{
    [Header("UI")]
    [SerializeField] TextMeshProUGUI waveText;

    [Header("Waves")]
    [SerializeField] List<WaveData> wavesData;
    [SerializeField] float pauseBetweenWaves = 2f;

    private int currentWave = 0;
    private bool isSpawing = false;

    [Header("Path")]
    [SerializeField] List<Transform> pathPoints;
    [SerializeField] Transform spawnPoint;

    List<int> wavesLeftEnemies;

    private Coroutine spawnCoroutine;

    public Dictionary<EnemyData, ObjectPooler<EnemyController>> Pools;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Pools = new();
        wavesLeftEnemies = new();
        wavesLeftEnemies.AddRange(wavesData.Select(x => x.Enemies.Count));
        RefreshUI();
        StartWaves();
    }

    private void RefreshUI()
    {
        waveText.text = $"Wave {currentWave + 1}/{wavesData.Count}";
    }

    private IEnumerator SpawnWave(float delay)
    {
        yield return WaitForSeconds(delay);

        WaveData wave = wavesData[currentWave];

        isSpawing = true;

        for (int i = 0; i < wave.Enemies.Count; i++)
        {
            SpawnEnemy(wave.Enemies[i]);
            yield return new WaitForSeconds(wave.SpawnRate);
        }

        isSpawing = false;
    }

    private IEnumerator WaitForSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

    private void SpawnEnemy(EnemyData data)
    {
        if (!Pools.ContainsKey(data))
        {
            Pools[data] = new ObjectPooler<EnemyController>(data.Prefab);
        }
        EnemyController enemy = Pools[data].Get();
        enemy.Initialize(pathPoints, spawnPoint.position, currentWave);
    }

    public void StartWaves()
    {
        spawnCoroutine = StartCoroutine(SpawnWave(0f));
    }

    private void SetEnemy(EnemyController enemy)
    {
        enemy.gameObject.SetActive(false);
        Pools[enemy.EnemyData].Set(enemy);
    }

    public void DefeatEnemy(EnemyController enemy)
    {
        wavesLeftEnemies[enemy.WaveId]--;
        SetEnemy(enemy);

        if (!IsWaveEnded()) return;

        if (currentWave < wavesData.Count - 1)
        {
            currentWave++;
            RefreshUI();
            StopSpawningCoroutine();
            spawnCoroutine = StartCoroutine(SpawnWave(wavesData[currentWave].SpawnRate));
            GameManager.Instance.AddCoins(wavesData[enemy.WaveId].WavePrize);
            return;
        }

        GameManager.Instance.Victory();
    }

    public bool IsWaveEnded()
    {
        return !FindAnyObjectByType<EnemyController>(FindObjectsInactive.Exclude) && !isSpawing;
    }

    public void ResetWaves()
    {
        StopSpawningCoroutine();
        SetAll();
        currentWave = 0;
        wavesLeftEnemies.Clear();
        wavesLeftEnemies.AddRange(wavesData.Select(x => x.Enemies.Count));
    }

    private void StopSpawningCoroutine()
    {
        StopCoroutine(spawnCoroutine);
        spawnCoroutine = null;
        isSpawing = false;
    }

    private void SetAll()
    {
        EnemyController[] activeEnemies = FindObjectsByType<EnemyController>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        for (int i = 0; i < activeEnemies.Length; i++)
        {
            EnemyController enemy = activeEnemies[i];

            SetEnemy(enemy);
        }
    }
}
