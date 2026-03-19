using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private GameObject impactEffectPrefab;

    [Header("Attributes")]
    [SerializeField] private float bulletSpeed = 5f;
    [SerializeField] private float destroyDelayIfNoTarget = 0.5f;
    [SerializeField] private float rotationOffset = -90f;

    private Transform target;
    private float bulletDamage;
    private bool isDestroying = false;

    public void SetTarget(Transform newTarget, float newDamage)
    {
        target = newTarget;
        bulletDamage = newDamage;

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
        Health health = other.gameObject.GetComponent<Health>();

        if (health != null)
        {
            health.TakeDamage(bulletDamage);
        }

        SpawnImpactEffect();
        Destroy(gameObject);
    }

    private IEnumerator DestroyBulletOverTime()
    {
        isDestroying = true;
        yield return new WaitForSeconds(destroyDelayIfNoTarget);

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
}