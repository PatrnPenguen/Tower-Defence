using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private float hitpoints = 2f;
    [SerializeField] private int currencyWorth = 50;

    [Header("References")]
    [SerializeField] private EnemyMovment enemyMovment;
    [SerializeField] private GameObject deathBloodEffectPrefab;

    private bool isDestroyed = false;

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
                print("particales");
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