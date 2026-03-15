using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    [SerializeField] private SpawnerLane lane;
    [SerializeField] private Transform[] pathPoints;

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
            Debug.LogWarning(gameObject.name + " has no path points assigned.");
            return;
        }

        Vector3 spawnPosition = pathPoints[0].position;

        GameObject enemyObject = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

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