using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private float hitpoints = 2f;
    [SerializeField] private int currencyWorth = 50;

    private bool isDestroyed = false;

    public void TakeDamage(float damage)
    {
        hitpoints -= damage;

        if (hitpoints <= 0f && !isDestroyed)
        {
            isDestroyed = true;

            WaveManager.OnEnemyRemovedFromWave();
            LevelManager.main.IncreaseCurrency(currencyWorth);

            Destroy(gameObject);
        }
    }
}