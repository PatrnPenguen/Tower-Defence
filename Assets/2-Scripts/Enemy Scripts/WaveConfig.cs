using System;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveConfig", menuName = "Tower Defence/Wave Config")]
public class WaveConfig : ScriptableObject
{
    public float startDelay = 0f;
    public SpawnerWaveEntry[] spawnerEntries;
}

[Serializable]
public class SpawnerWaveEntry
{
    public SpawnerLane lane;
    public EnemyGroupData[] groups;
}

[Serializable]
public class EnemyGroupData
{
    public GameObject enemyPrefab;
    public int enemyCount = 5;
    public float timeBetweenSpawns = 0.5f;
    public float delayBeforeGroup = 0f;
    public bool isBoss = false;
}