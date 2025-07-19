using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class WaveData : ScriptableObject
{
    [SerializeField] List<EnemyData> enemies;
    [SerializeField] float spawnRate = 1f;
    [SerializeField] int wavePrize = 200;

    public List<EnemyData> Enemies => enemies;
    public float SpawnRate => spawnRate;
    public int WavePrize => wavePrize;
}
