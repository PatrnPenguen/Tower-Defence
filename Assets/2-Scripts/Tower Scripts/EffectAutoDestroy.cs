using UnityEngine;

public class EffectAutoDestroy : MonoBehaviour
{
    [SerializeField] private float lifeTime = 0.4f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}