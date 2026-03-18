using System.Collections;
using UnityEngine;

public class EnemyMovment : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;

    [Header("Attributes")]
    [SerializeField] private float moveSpeed = 2f;

    private Transform[] currentPath;
    private Transform currentTarget;
    private int pathIndex = 0;
    private float baseSpeed;
    private Coroutine stunCoroutine;
    private bool isStunned = false;

    private void Start()
    {
        baseSpeed = moveSpeed;
    }

    private void Update()
    {
        if (currentTarget == null)
        {
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
        if (currentTarget == null || isStunned)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 direction = (currentTarget.position - transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed;
    }

    public void SetPath(Transform[] newPath)
    {
        if (newPath == null || newPath.Length == 0)
        {
            Debug.LogWarning("Path is empty.");
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

        yield return new WaitForSeconds(duration);

        isStunned = false;
        stunCoroutine = null;
    }
}