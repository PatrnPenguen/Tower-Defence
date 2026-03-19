using System.Collections;
using UnityEngine;

public class LightningBullet : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private GameObject impactEffectPrefab;

    [Header("Attributes")]
    [SerializeField] private float bulletSpeed = 6f;
    [SerializeField] private float destroyDelayIfNoTarget = 0.5f;
    [SerializeField] private float rotationOffset = -90f;

    private Transform target;
    private float bulletDamage;
    private float splashRadius;
    private float stunDuration;
    private LayerMask enemyMask;
    private bool isDestroying = false;

    public void SetTarget(
        Transform newTarget,
        float newDamage,
        float newSplashRadius,
        float newStunDuration,
        LayerMask newEnemyMask
    )
    {
        target = newTarget;
        bulletDamage = newDamage;
        splashRadius = newSplashRadius;
        stunDuration = newStunDuration;
        enemyMask = newEnemyMask;

        UpdateBulletRotation();
    }

    private void FixedUpdate()
    {
        if (target == null)
        {
            rb.linearVelocity = Vector2.zero;

            if (!isDestroying)
            {
                StartCoroutine(DestroyBulletOverTime());
            }

            return;
        }

        Vector2 direction = (target.position - transform.position).normalized;
        rb.linearVelocity = direction * bulletSpeed;

        UpdateBulletRotation();
    }

    private void UpdateBulletRotation()
    {
        if (target == null)
        {
            return;
        }

        Vector2 direction = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + rotationOffset;

        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("hit " + other.gameObject.name);
        Explode();
    }

    private IEnumerator DestroyBulletOverTime()
    {
        isDestroying = true;
        yield return new WaitForSeconds(destroyDelayIfNoTarget);

        Destroy(gameObject);
    }

    private void Explode()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(
            transform.position,
            splashRadius,
            enemyMask
        );

        for (int i = 0; i < hitColliders.Length; i++)
        {
            Health health = hitColliders[i].GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(bulletDamage);
            }

            EnemyMovment enemyMovement = hitColliders[i].GetComponent<EnemyMovment>();
            if (enemyMovement != null)
            {
                enemyMovement.ApplyStun(stunDuration);
            }
        }

        Debug.Log("exploded");
        SpawnImpactEffect();
        Destroy(gameObject);
    }

    private void SpawnImpactEffect()
    {
        if (impactEffectPrefab != null)
        {
            Instantiate(impactEffectPrefab, transform.position, Quaternion.identity);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, splashRadius);
    }
}