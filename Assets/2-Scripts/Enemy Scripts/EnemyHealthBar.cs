using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image fillImage;

    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if (fillImage == null || maxHealth <= 0f)
        {
            return;
        }

        fillImage.fillAmount = currentHealth / maxHealth;
    }
}