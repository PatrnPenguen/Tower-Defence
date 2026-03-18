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

    private Transform target;
    private float bulletDamage;
    private bool isDestroying = false;

    public void SetTarget(Transform newTarget, float newDamage)
    {
        target = newTarget;
        bulletDamage = newDamage;
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
        
        Debug.Log("destroying " + gameObject.name);
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