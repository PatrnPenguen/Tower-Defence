using System.Collections;
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

    protected override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        timeUntilAttack += Time.deltaTime;

        if (timeUntilAttack >= 1f / attackSpeed)
        {
            FreezeEnemies();
            timeUntilAttack = 0f;
        }
    }

    private void FreezeEnemies()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(
            transform.position,
            range,
            Vector2.zero,
            0f,
            enemyMask
        );

        for (int i = 0; i < hits.Length; i++)
        {
            EnemyMovment enemyMovement = hits[i].transform.GetComponent<EnemyMovment>();

            if (enemyMovement != null)
            {
                enemyMovement.UpdateSpeed(slowEffectSpeed);
                StartCoroutine(ResetEnemySpeed(enemyMovement));
            }
        }
    }

    private IEnumerator ResetEnemySpeed(EnemyMovment enemyMovement)
    {
        yield return new WaitForSeconds(slowEffectDuration);

        if (enemyMovement != null)
        {
            enemyMovement.ResetSpeed();
        }
    }

    public override void UpgradeTower()
    {
        if (LevelManager.main.currency < upgradeCost)
        {
            Debug.Log("Not enough money to upgrade");
            return;
        }

        LevelManager.main.SpendCurrency(upgradeCost);

        range += 0.4f;
        attackSpeed += 0.15f;
        slowEffectDuration += 0.2f;

        upgradeCost += 30;
        sellCost += 20;

        Debug.Log("Slow tower upgraded");
    }

    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.cyan;
        Handles.DrawWireDisc(transform.position, transform.forward, range);
    }
}