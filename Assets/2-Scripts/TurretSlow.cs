using System.Collections;
using UnityEditor;
using UnityEngine;

public class TurretSlow : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private LayerMask enemyMask;
    
    [Header("Attribute")] 
    [SerializeField] private float targetingRange = 3f;
    [SerializeField] private float attackPerSecond = 0.25f;
    [SerializeField] private float slowEffectSpeed = 0.5f;
    [SerializeField] private float slowEffectDuration = 1f;
    
    private float timeUntilFire;
    
    private void Update()
    {
        timeUntilFire += Time.deltaTime;

        if (timeUntilFire >= 1f / attackPerSecond)
        {
            FreezeEnemies();
            timeUntilFire = 0;
        }
    }

    private void FreezeEnemies()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, targetingRange, 
            (Vector2)transform.position, 0f, enemyMask);

        if (hits.Length > 0)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit2D hit = hits[i];

                EnemyMovment enemyMovment = hit.transform.GetComponent<EnemyMovment>();
                enemyMovment.UpdateSpeed(slowEffectSpeed);

                StartCoroutine(ResetEnemySpeed(enemyMovment));
            }
        }
    }

    private IEnumerator ResetEnemySpeed(EnemyMovment enemyMovment)
    {
        yield return new WaitForSeconds(slowEffectDuration);
        enemyMovment.ResetSpeed();
    }
    
    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.cyan;
        Handles.DrawWireDisc(transform.position, transform.forward, targetingRange);
    }
}
