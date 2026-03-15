using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaveManager : MonoBehaviour
{
    public static WaveManager main;

    [Header("Wave Settings")]
    [SerializeField] private WaveConfig[] waves;
    [SerializeField] private EnemySpawner[] sceneSpawners;

    [Header("UI")]
    [SerializeField] private GameObject startWaveButtonObject;
    [SerializeField] private Button startWaveButton;
    [SerializeField] private TextMeshProUGUI waveInfoText;
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private GameObject laneWarningPanel;
    [SerializeField] private TextMeshProUGUI laneWarningText;

    private int currentWaveIndex = 0;
    private int enemiesAlive = 0;
    private int enemiesLeftToSpawn = 0;
    private bool waveInProgress = false;
    private bool allWavesFinished = false;

    private void Awake()
    {
        main = this;
    }

    private void Start()
    {
        if (startWaveButton != null)
        {
            startWaveButton.onClick.AddListener(StartNextWave);
        }

        RefreshUI();
    }

    private IEnumerator RunWave(WaveConfig wave)
    {
        if (wave == null)
        {
            yield break;
        }

        waveInProgress = true;
        enemiesAlive = 0;
        enemiesLeftToSpawn = CountTotalEnemies(wave);

        RefreshUI();

        if (wave.startDelay > 0f)
        {
            yield return new WaitForSeconds(wave.startDelay);
        }

        if (wave.spawnerEntries != null)
        {
            for (int i = 0; i < wave.spawnerEntries.Length; i++)
            {
                StartCoroutine(RunSpawnerEntry(wave.spawnerEntries[i]));
            }
        }

        yield return new WaitUntil(() => enemiesLeftToSpawn <= 0 && enemiesAlive <= 0);

        waveInProgress = false;
        currentWaveIndex++;

        if (currentWaveIndex >= waves.Length)
        {
            allWavesFinished = true;
        }

        RefreshUI();
    }
    
    private EnemySpawner GetSpawnerByLane(SpawnerLane lane)
    {
        for (int i = 0; i < sceneSpawners.Length; i++)
        {
            if (sceneSpawners[i] != null && sceneSpawners[i].Lane == lane)
            {
                return sceneSpawners[i];
            }
        }

        return null;
    }

    private IEnumerator RunSpawnerEntry(SpawnerWaveEntry entry)
    {
        if (entry == null || entry.groups == null)
        {
            yield break;
        }

        EnemySpawner targetSpawner = GetSpawnerByLane(entry.lane);

        if (targetSpawner == null)
        {
            Debug.LogWarning("No spawner found for lane: " + entry.lane);
            yield break;
        }

        for (int i = 0; i < entry.groups.Length; i++)
        {
            EnemyGroupData group = entry.groups[i];

            if (group == null || group.enemyPrefab == null || group.enemyCount <= 0)
            {
                continue;
            }

            if (group.delayBeforeGroup > 0f)
            {
                yield return new WaitForSeconds(group.delayBeforeGroup);
            }

            for (int j = 0; j < group.enemyCount; j++)
            {
                targetSpawner.SpawnEnemy(group.enemyPrefab);

                enemiesLeftToSpawn--;
                enemiesAlive++;

                if (group.timeBetweenSpawns > 0f)
                {
                    yield return new WaitForSeconds(group.timeBetweenSpawns);
                }
            }
        }
    }

    private int CountTotalEnemies(WaveConfig wave)
    {
        int total = 0;

        if (wave == null || wave.spawnerEntries == null)
        {
            return total;
        }

        for (int i = 0; i < wave.spawnerEntries.Length; i++)
        {
            SpawnerWaveEntry entry = wave.spawnerEntries[i];

            if (entry == null || entry.groups == null)
            {
                continue;
            }

            for (int j = 0; j < entry.groups.Length; j++)
            {
                EnemyGroupData group = entry.groups[j];

                if (group != null)
                {
                    total += group.enemyCount;
                }
            }
        }

        return total;
    }

    public static void OnEnemyRemovedFromWave()
    {
        if (main == null)
        {
            return;
        }

        main.enemiesAlive--;

        if (main.enemiesAlive < 0)
        {
            main.enemiesAlive = 0;
        }
    }
    
    public void StartNextWave()
    {
        if (waveInProgress)
        {
            return;
        }

        if (allWavesFinished)
        {
            return;
        }

        if (currentWaveIndex >= waves.Length)
        {
            allWavesFinished = true;
            RefreshUI();
            return;
        }

        HideAllLaneWarnings();
        StartCoroutine(RunWave(waves[currentWaveIndex]));
    }
    
    private void ShowWarningsForCurrentWave()
    {
        HideAllLaneWarnings();

        if (currentWaveIndex >= waves.Length || waves[currentWaveIndex] == null)
        {
            return;
        }

        WaveConfig currentWave = waves[currentWaveIndex];

        if (currentWave.spawnerEntries == null)
        {
            return;
        }

        for (int i = 0; i < currentWave.spawnerEntries.Length; i++)
        {
            SpawnerWaveEntry entry = currentWave.spawnerEntries[i];
            EnemySpawner targetSpawner = GetSpawnerByLane(entry.lane);

            if (targetSpawner == null)
            {
                continue;
            }

            bool hasBoss = false;

            if (entry.groups != null)
            {
                for (int j = 0; j < entry.groups.Length; j++)
                {
                    if (entry.groups[j] != null && entry.groups[j].isBoss)
                    {
                        hasBoss = true;
                        break;
                    }
                }
            }

            if (hasBoss)
            {
                targetSpawner.ShowBossWarning();
            }
            else
            {
                targetSpawner.ShowNormalWarning();
            }
        }
    }
    
    private void HideAllLaneWarnings()
    {
        if (sceneSpawners == null)
        {
            return;
        }

        for (int i = 0; i < sceneSpawners.Length; i++)
        {
            if (sceneSpawners[i] != null)
            {
                sceneSpawners[i].HideWarnings();
            }
        }
    }

    private void RefreshUI()
    {
        bool showPreWaveUI = !waveInProgress && !allWavesFinished;

        if (startWaveButtonObject != null)
        {
            startWaveButtonObject.SetActive(showPreWaveUI);
        }

        if (waveInfoText != null)
        {
            if (allWavesFinished)
            {
                waveInfoText.text = "All waves completed";
            }
            else if (waveInProgress)
            {
                waveInfoText.text = "Wave " + (currentWaveIndex + 1) + " is running";
            }
            else
            {
                waveInfoText.text = "Ready for Wave " + (currentWaveIndex + 1);
            }
        }

        if (buttonText != null && !allWavesFinished)
        {
            buttonText.text = "Start Wave " + (currentWaveIndex + 1);
        }

        if (showPreWaveUI)
        {
            ShowWarningsForCurrentWave();
        }
        else
        {
            HideAllLaneWarnings();
        }
    }

    public int GetCurrentWaveNumber()
    {
        return currentWaveIndex + 1;
    }

    public int GetTotalWaveCount()
    {
        return waves.Length;
    }

    public bool IsWaveRunning()
    {
        return waveInProgress;
    }

    public bool AreAllWavesFinished()
    {
        return allWavesFinished;
    }
}