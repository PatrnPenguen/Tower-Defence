using UnityEngine;
using UnityEditor;

public class ArcherTower : TowerBasics
{
    [Header("Archer Tower References")]
    [SerializeField] private Transform turretRotationPoint;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private Transform firingPoint;
    [SerializeField] private float rotationSpeed = 200f;

    private Transform target;
    private float timeUntilFire;

    protected override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        UpdateButtonTexts();
        if (!IsTargetValid())
        {
            target = null;
            FindTarget();
            return;
        }

        RotateTowardsTarget();

        if (!CheckTargetIsInRange())
        {
            target = null;
            return;
        }

        timeUntilFire += Time.deltaTime;

        if (timeUntilFire >= 1f / attackSpeed)
        {
            Shoot();
            timeUntilFire = 0f;
        }
    }

    private void FindTarget()
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
            Transform possibleTarget = hits[i].transform;

            if (possibleTarget == null)
            {
                continue;
            }

            EnemyMovment enemyMovment = possibleTarget.GetComponent<EnemyMovment>();

            if (enemyMovment == null)
            {
                enemyMovment = possibleTarget.GetComponentInParent<EnemyMovment>();
            }

            if (enemyMovment != null && !enemyMovment.IsDead)
            {
                target = possibleTarget;
                return;
            }
        }

        target = null;
    }

    private bool IsTargetValid()
    {
        if (target == null)
        {
            return false;
        }

        EnemyMovment enemyMovment = target.GetComponent<EnemyMovment>();

        if (enemyMovment == null)
        {
            enemyMovment = target.GetComponentInParent<EnemyMovment>();
        }

        if (enemyMovment == null)
        {
            return false;
        }

        return !enemyMovment.IsDead;
    }

    private void RotateTowardsTarget()
    {
        if (target == null || turretRotationPoint == null)
        {
            return;
        }

        float angle = Mathf.Atan2(
            target.position.y - transform.position.y,
            target.position.x - transform.position.x
        ) * Mathf.Rad2Deg - 90f;

        Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);

        turretRotationPoint.rotation = Quaternion.RotateTowards(
            turretRotationPoint.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    private bool CheckTargetIsInRange()
    {
        if (target == null)
        {
            return false;
        }

        return Vector2.Distance(target.position, transform.position) <= range;
    }

    private void Shoot()
    {
        if (!IsTargetValid())
        {
            target = null;
            return;
        }

        GameObject projectilePrefab = GetCurrentProjectilePrefab();

        if (projectilePrefab == null)
        {
            return;
        }

        if (firingPoint == null)
        {
            return;
        }

        PlayWeaponShootAnimation();

        GameObject projectileObject = Instantiate(
            projectilePrefab,
            firingPoint.position,
            Quaternion.identity
        );

        Bullet bullet = projectileObject.GetComponent<Bullet>();

        if (bullet != null)
        {
            bullet.SetTarget(target, damage);
        }
    }

    public override void UpgradeTower()
    {
        if (!CanUpgrade())
        {
            return;
        }

        if (LevelManager.main.currency < upgradeCost)
        {
            return;
        }

        LevelManager.main.SpendCurrency(upgradeCost);

        damage += damageUp;
        range += rangeUp;
        attackSpeed += attackSpeedUp;

        upgradeCost += upgradeCostUp;
        sellCost += sellCostUp;

        currentLevel++;

        ApplyLevelVisuals();
        UpdateWeaponAnimationSpeed();
        UpdateButtonTexts();
    }

    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.cyan;
        Handles.DrawWireDisc(transform.position, transform.forward, range);
    }
}