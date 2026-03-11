using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;

    [Header("Attributes")]
    [SerializeField] private float bulletSpeed = 5f;

    private Transform target;
    private float bulletDamage;

    public void SetTarget(Transform newTarget, float newDamage)
    {
        target = newTarget;
        bulletDamage = newDamage;
    }

    private void FixedUpdate()
    {
        if (target == null)
        {
            StartCoroutine(DestroyBulletOverTime());
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

        Destroy(gameObject);
    }

    private IEnumerator DestroyBulletOverTime()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}