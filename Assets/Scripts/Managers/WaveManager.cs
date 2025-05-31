using DesignPatterns.Generics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : Singleton<WaveManager>
{
    [SerializeField] EnemyController prefabEnemy;
    [SerializeField] int waveCount;
    [SerializeField] float waveRate;
    [Header("Path")]
    [SerializeField] List<Transform> pathPoints;

    public ObjectPooler<EnemyController> Pooler;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Pooler = new(prefabEnemy);
        InvokeRepeating(nameof(SpawnEnemy), 0, waveRate);
    }

    private void SpawnEnemy()
    { 
        EnemyController enemy = Pooler.Get();
        enemy.Initialize(pathPoints);
    }
}
