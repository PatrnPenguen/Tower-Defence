using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private float hitpoints = 2f;
    [SerializeField] private int currencyWorth = 50;
    [SerializeField] private int endpointDamage = 1;

    [Header("References")]
    [SerializeField] private EnemyMovment enemyMovment;
    [SerializeField] private GameObject deathBloodEffectPrefab;

    private bool isDestroyed = false;

    public int GetEndpointDamage()
    {
        return endpointDamage;
    }

    public void TakeDamage(float damage)
    {
        if (isDestroyed)
        {
            return;
        }

        hitpoints -= damage;

        if (hitpoints <= 0f)
        {
            isDestroyed = true;

            if (deathBloodEffectPrefab != null)
            {
                Instantiate(
                    deathBloodEffectPrefab,
                    transform.position,
                    Quaternion.Euler(-90, 0, 0)
                );
            }

            WaveManager.OnEnemyRemovedFromWave();
            LevelManager.main.IncreaseCurrency(currencyWorth);

            if (enemyMovment != null)
            {
                enemyMovment.Die();
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}