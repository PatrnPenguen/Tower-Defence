using UnityEngine;
using UnityEditor;

public class SlowTower : TowerBasics
{
    [Header("Slow Tower References")]
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private Transform castPoint;

    [Header("Slow Tower Special Stats")]
    [SerializeField] private float slowCooldown = 3f;
    [SerializeField] private float slowPercent = 0.3f;
    [SerializeField] private float slowDuration = 0.8f;

    private float timeUntilCast;

    protected override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        timeUntilCast += Time.deltaTime;

        if (timeUntilCast < slowCooldown)
        {
            return;
        }

        if (!HasEnemyInRange())
        {
            return;
        }

        CastSlowRing();
        timeUntilCast = 0f;
    }

    public float GetSlowPercent()
    {
        return slowPercent;
    }

    public float GetSlowDuration()
    {
        return slowDuration;
    }

    private bool HasEnemyInRange()
    {
        Collider2D hit = Physics2D.OverlapCircle(
            transform.position,
            range,
            enemyMask
        );

        return hit != null;
    }

    private void CastSlowRing()
    {
        GameObject ringPrefab = GetCurrentProjectilePrefab();

        if (ringPrefab == null)
        {
            Debug.LogWarning(gameObject.name + " current level ring prefab is missing.");
            return;
        }

        Vector3 spawnPosition = castPoint != null ? castPoint.position : transform.position;

        PlayWeaponShootAnimation();
        Instantiate(ringPrefab, spawnPosition, Quaternion.identity);
    }

    public override void UpgradeTower()
    {
        if (!CanUpgrade())
        {
            Debug.Log("Tower is already at max level");
            return;
        }

        if (LevelManager.main.currency < upgradeCost)
        {
            Debug.Log("Not enough money to upgrade");
            return;
        }

        LevelManager.main.SpendCurrency(upgradeCost);
        
        slowCooldown -= 0.3f;
        if (slowCooldown < 1.5f)
        {
            slowCooldown = 1.5f;
        }
        
        slowDuration += 0.1f;
        if (slowDuration < 0.6f)
        {
            slowDuration = 0.6f;
        }
        
        upgradeCost += 30;
        sellCost += 20;

        currentLevel++;

        ApplyLevelVisuals();
        UpdateButtonTexts();

        Debug.Log("Slow tower upgraded");
    }

    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.cyan;
        Handles.DrawWireDisc(transform.position, transform.forward, range);
    }
}