using System.Collections;
using UnityEngine;

public class EnemyMovment : MonoBehaviour
{
    public enum LookDirection
    {
        Up = 0,
        Down = 1,
        Left = 2,
        Right = 3
    }

    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private Collider2D enemyCollider;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Attributes")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float deathDestroyDelay = 0.8f;

    private Transform[] currentPath;
    private Transform currentTarget;
    private int pathIndex = 0;
    private float baseSpeed;
    private Coroutine stunCoroutine;
    private Coroutine slowCoroutine;
    private bool isStunned = false;
    private bool isDead = false;
    public LookDirection currentLookDirection = LookDirection.Down;

    private void Start()
    {
        baseSpeed = moveSpeed;
        UpdateAnimator(false);
    }

    private void Update()
    {
        if (isDead)
        {
            return;
        }

        if (currentTarget == null)
        {
            UpdateAnimator(false);
            return;
        }

        if (Vector2.Distance(transform.position, currentTarget.position) <= 0.1f)
        {
            pathIndex++;

            if (currentPath == null || pathIndex >= currentPath.Length)
            {
                WaveManager.OnEnemyRemovedFromWave();
                Destroy(gameObject);
                return;
            }

            currentTarget = currentPath[pathIndex];
        }
    }

    private void FixedUpdate()
    {
        if (isDead)
        {
            rb.linearVelocity = Vector2.zero;
            UpdateAnimator(false);
            return;
        }

        if (currentTarget == null || isStunned)
        {
            rb.linearVelocity = Vector2.zero;
            UpdateAnimator(false);
            return;
        }

        Vector2 direction = (currentTarget.position - transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed;

        UpdateLookDirection();
        UpdateAnimator(true);
    }

    public void SetPath(Transform[] newPath)
    {
        if (newPath == null || newPath.Length == 0)
        {
            return;
        }

        currentPath = newPath;
        pathIndex = 0;
        currentTarget = currentPath[pathIndex];

        transform.position = currentTarget.position;
    }

    public void UpdateSpeed(float newSpeed)
    {
        moveSpeed = newSpeed;
    }

    public void ResetSpeed()
    {
        moveSpeed = baseSpeed;
    }

    public void ApplyStun(float duration)
    {
        if (isDead)
        {
            return;
        }

        if (stunCoroutine != null)
        {
            StopCoroutine(stunCoroutine);
        }

        stunCoroutine = StartCoroutine(StunCoroutine(duration));
    }

    private IEnumerator StunCoroutine(float duration)
    {
        isStunned = true;
        rb.linearVelocity = Vector2.zero;
        UpdateAnimator(false);

        yield return new WaitForSeconds(duration);

        isStunned = false;
        stunCoroutine = null;
    }

    public void ApplySlowPercent(float slowPercent, float duration)
    {
        if (isDead)
        {
            return;
        }

        if (slowCoroutine != null)
        {
            StopCoroutine(slowCoroutine);
        }

        slowCoroutine = StartCoroutine(SlowCoroutine(slowPercent, duration));
    }

    private IEnumerator SlowCoroutine(float slowPercent, float duration)
    {
        moveSpeed = baseSpeed * (1f - slowPercent);

        yield return new WaitForSeconds(duration);

        ResetSpeed();
        slowCoroutine = null;
    }

    public void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;
        rb.linearVelocity = Vector2.zero;

        if (enemyCollider != null)
        {
            enemyCollider.enabled = false;
        }

        UpdateAnimator(false);

        if (animator != null)
        {
            animator.SetInteger("Direction", (int)currentLookDirection);
            animator.SetTrigger("Die");
        }

        StartCoroutine(DestroyAfterDeath());
    }

    private IEnumerator DestroyAfterDeath()
    {
        yield return new WaitForSeconds(deathDestroyDelay);
        Destroy(gameObject);
    }

    private void UpdateLookDirection()
    {
        Vector2 velocity = rb.linearVelocity;

        if (velocity.sqrMagnitude <= 0.001f)
        {
            return;
        }

        if (Mathf.Abs(velocity.x) > Mathf.Abs(velocity.y))
        {
            if (velocity.x > 0f)
            {
                spriteRenderer.flipX = false;
                currentLookDirection = LookDirection.Right;
            }
            else
            {
                spriteRenderer.flipX = true;
                currentLookDirection = LookDirection.Left;
            }
        }
        else
        {
            if (velocity.y > 0f)
            {
                currentLookDirection = LookDirection.Up;
            }
            else
            {
                currentLookDirection = LookDirection.Down;
            }
        }
    }

    private void UpdateAnimator(bool isMoving)
    {
        if (animator != null)
        {
            animator.SetBool("IsMoving", isMoving);
            animator.SetInteger("Direction", (int)currentLookDirection);
        }
    }
}