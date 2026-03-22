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
    [SerializeField] private EnemyHealthBar healthBar;

    private bool isDestroyed = false;
    private float maxHitpoints;

    private void Start()
    {
        maxHitpoints = hitpoints;
        UpdateHealthBar();
    }

    public int GetEndpointDamage()
    {
        return endpointDamage;
    }

    public void IncreaseHealthByPercent(float percent)
    {
        hitpoints += hitpoints * (percent / 100f);
        maxHitpoints = hitpoints;
        UpdateHealthBar();
    }

    public void TakeDamage(float damage)
    {
        if (isDestroyed)
        {
            return;
        }

        hitpoints -= damage;
        UpdateHealthBar();

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

    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.UpdateHealthBar(hitpoints, maxHitpoints);
        }
    }
}