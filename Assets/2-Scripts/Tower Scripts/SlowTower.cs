using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SlowTower : TowerBasics
{
    [Header("Slow Tower References")]
    [SerializeField] private LayerMask enemyMask;

    [Header("Slow Tower Special Stats")]
    [SerializeField] private float slowEffectSpeed = 0.5f;
    [SerializeField] private float slowEffectDuration = 1f;

    private float timeUntilAttack;
    private Dictionary<EnemyMovment, Coroutine> activeSlowCoroutines = new Dictionary<EnemyMovment, Coroutine>();

    protected override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        timeUntilAttack += Time.deltaTime;

        if (timeUntilAttack >= 1f / attackSpeed)
        {
            SlowEnemiesInRange();
            timeUntilAttack = 0f;
        }
    }

    private void SlowEnemiesInRange()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(
            transform.position,
            range,
            Vector2.zero,
            0f,
            enemyMask
        );

        if (hits.Length == 0)
        {
            return;
        }

        PlayWeaponShootAnimation();

        for (int i = 0; i < hits.Length; i++)
        {
            EnemyMovment enemyMovement = hits[i].transform.GetComponent<EnemyMovment>();

            if (enemyMovement == null)
            {
                continue;
            }

            enemyMovement.UpdateSpeed(slowEffectSpeed);

            if (activeSlowCoroutines.ContainsKey(enemyMovement))
            {
                if (activeSlowCoroutines[enemyMovement] != null)
                {
                    StopCoroutine(activeSlowCoroutines[enemyMovement]);
                }
            }

            Coroutine slowCoroutine = StartCoroutine(ResetEnemySpeedAfterDelay(enemyMovement));
            activeSlowCoroutines[enemyMovement] = slowCoroutine;
        }
    }

    private IEnumerator ResetEnemySpeedAfterDelay(EnemyMovment enemyMovement)
    {
        yield return new WaitForSeconds(slowEffectDuration);

        if (enemyMovement != null)
        {
            enemyMovement.ResetSpeed();
        }

        if (enemyMovement != null && activeSlowCoroutines.ContainsKey(enemyMovement))
        {
            activeSlowCoroutines.Remove(enemyMovement);
        }
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

        range += 0.4f;
        attackSpeed += 0.15f;
        slowEffectDuration += 0.25f;

        if (slowEffectSpeed > 0.2f)
        {
            slowEffectSpeed -= 0.05f;
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