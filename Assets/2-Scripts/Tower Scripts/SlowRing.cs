using System.Collections.Generic;
using UnityEngine;

public class SlowRing : MonoBehaviour
{
    [SerializeField] private float slowPercent = 0.4f;
    [SerializeField] private float slowDuration = 1f;
    [SerializeField] private float lifeTime = 0.2f;
    [SerializeField] private LayerMask enemyMask;

    private HashSet<EnemyMovment> affectedEnemies = new HashSet<EnemyMovment>();

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Ring touched: " + other.name);

        if (((1 << other.gameObject.layer) & enemyMask) == 0)
        {
            return;
        }

        EnemyMovment enemy = other.GetComponentInParent<EnemyMovment>();

        if (enemy == null)
        {
            Debug.Log("EnemyMovment not found on touched object.");
            return;
        }

        if (affectedEnemies.Contains(enemy))
        {
            return;
        }

        affectedEnemies.Add(enemy);
        enemy.ApplySlowPercent(slowPercent, slowDuration);
    }
}