using UnityEngine;

public class CatapultProjectile : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private GameObject impactEffectPrefab;

    [Header("Attributes")]
    [SerializeField] private float projectileSpeed = 4f;
    [SerializeField] private float hitDistance = 0.1f;

    private Vector2 targetPosition;
    private float projectileDamage;
    private float splashRadius;
    private LayerMask enemyMask;
    private bool hasTargetPosition = false;
    private bool hasExploded = false;

    public void SetTargetPosition(
        Vector2 newTargetPosition,
        float newDamage,
        float newSplashRadius,
        LayerMask newEnemyMask
    )
    {
        targetPosition = newTargetPosition;
        projectileDamage = newDamage;
        splashRadius = newSplashRadius;
        enemyMask = newEnemyMask;
        hasTargetPosition = true;
    }

    private void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
    }

    private void FixedUpdate()
    {
        if (!hasTargetPosition || hasExploded)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 currentPosition = rb.position;
        Vector2 direction = (targetPosition - currentPosition).normalized;

        rb.linearVelocity = direction * projectileSpeed;

        float distanceToTarget = Vector2.Distance(currentPosition, targetPosition);

        if (distanceToTarget <= hitDistance)
        {
            Explode();
        }
    }

    private void Explode()
    {
        if (hasExploded)
        {
            return;
        }

        hasExploded = true;
        rb.linearVelocity = Vector2.zero;

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
                health.TakeDamage(projectileDamage);
            }
        }

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
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, splashRadius);
    }
}