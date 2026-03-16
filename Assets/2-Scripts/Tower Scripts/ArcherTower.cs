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
            Debug.Log("Tower is already at max level");
            return;
        }

        if (LevelManager.main.currency < upgradeCost)
        {
            Debug.Log("Not enough money to upgrade");
            return;
        }

        LevelManager.main.SpendCurrency(upgradeCost);

        damage += 1f;
        range += 0.5f;
        attackSpeed += 0.3f;

        upgradeCost += 30;
        sellCost += 20;

        currentLevel++;

        ApplyLevelVisuals();
        UpdateButtonTexts();

        Debug.Log("Archer tower upgraded");
    }

    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.cyan;
        Handles.DrawWireDisc(transform.position, transform.forward, range);
    }
}