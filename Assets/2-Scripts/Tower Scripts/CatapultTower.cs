using UnityEngine;
using UnityEditor;

public class CatapultTower : TowerBasics
{
    [Header("Catapult Tower References")]
    [SerializeField] private Transform turretRotationPoint;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private Transform firingPoint;
    [SerializeField] private float rotationSpeed = 200f;

    [Header("Catapult Special Stats")]
    [SerializeField] private float splashRadius = 1.5f;

    private Transform target;
    private float timeUntilFire;

    protected override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        if (target == null)
        {
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

        if (hits.Length > 0)
        {
            target = hits[0].transform;
        }
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
        GameObject projectilePrefab = GetCurrentProjectilePrefab();

        if (projectilePrefab == null)
        {
            Debug.LogWarning(gameObject.name + " current level projectile prefab is missing.");
            return;
        }

        if (firingPoint == null)
        {
            Debug.LogWarning(gameObject.name + " firing point is missing.");
            return;
        }

        if (target == null)
        {
            return;
        }

        PlayWeaponShootAnimation();

        Vector2 targetPosition = target.position;

        GameObject projectileObject = Instantiate(
            projectilePrefab,
            firingPoint.position,
            Quaternion.identity
        );

        CatapultProjectile projectile = projectileObject.GetComponent<CatapultProjectile>();

        if (projectile != null)
        {
            projectile.SetTargetPosition(targetPosition, damage, splashRadius, enemyMask);
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

        damage += 2f;
        range += 0.4f;
        attackSpeed += 0.15f;
        splashRadius += 0.2f;

        upgradeCost += 40;
        sellCost += 25;

        currentLevel++;

        ApplyLevelVisuals();
        UpdateButtonTexts();

        Debug.Log("Catapult tower upgraded");
    }

    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.cyan;
        Handles.DrawWireDisc(transform.position, transform.forward, range);
    }
}