using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    [SerializeField] private SpawnerLane lane;
    [SerializeField] private Transform[] pathPoints;

    [Header("Health Scaling")]
    [SerializeField] private float healthIncreasePercentPer5Waves = 20f;

    [Header("Warning")]
    [SerializeField] private GameObject normalWarningObject;
    [SerializeField] private GameObject bossWarningObject;

    public SpawnerLane Lane => lane;

    public void SpawnEnemy(GameObject enemyPrefab)
    {
        if (enemyPrefab == null)
        {
            return;
        }

        if (pathPoints == null || pathPoints.Length == 0)
        {
            return;
        }

        Vector3 spawnPosition = pathPoints[0].position;

        GameObject enemyObject = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

        Health health = enemyObject.GetComponent<Health>();

        if (health != null && WaveManager.main != null)
        {
            int currentWave = WaveManager.main.GetCurrentWaveNumber();
            int bonusStep = (currentWave - 1) / 5;
            float totalPercentBonus = bonusStep * healthIncreasePercentPer5Waves;

            health.IncreaseHealthByPercent(totalPercentBonus);
        }

        EnemyMovment enemyMovment = enemyObject.GetComponent<EnemyMovment>();

        if (enemyMovment != null)
        {
            enemyMovment.SetPath(pathPoints);
        }
    }

    public void ShowNormalWarning()
    {
        HideWarnings();

        if (normalWarningObject != null)
        {
            normalWarningObject.SetActive(true);
        }
    }

    public void ShowBossWarning()
    {
        HideWarnings();

        if (bossWarningObject != null)
        {
            bossWarningObject.SetActive(true);
        }
    }

    public void HideWarnings()
    {
        if (normalWarningObject != null)
        {
            normalWarningObject.SetActive(false);
        }

        if (bossWarningObject != null)
        {
            bossWarningObject.SetActive(false);
        }
    }
}