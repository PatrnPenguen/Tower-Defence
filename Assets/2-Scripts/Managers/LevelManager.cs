using System;
using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public static LevelManager main;

    [Header("Currency")]
    [SerializeField] private int startCurrency;
    [NonSerialized] public int currency;

    [Header("Player Health")]
    [SerializeField] private int startHealth = 50;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;

    private int currentHealth;
    private bool gameEnded = false;

    private void Awake()
    {
        main = this;
    }

    private void Start()
    {
        currency = startCurrency;
        currentHealth = startHealth;
        RefreshHealthUI();

        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }

        if (losePanel != null)
        {
            losePanel.SetActive(false);
        }
    }

    public void IncreaseCurrency(int amount)
    {
        if (gameEnded)
        {
            return;
        }

        currency += amount;
    }

    public bool SpendCurrency(int amount)
    {
        if (gameEnded)
        {
            return false;
        }

        if (amount <= currency)
        {
            currency -= amount;
            return true;
        }
        
        return false;
    }

    public void TakeDamage(int damage)
    {
        if (gameEnded)
        {
            return;
        }

        currentHealth -= damage;
        RefreshHealthUI();

        if (currentHealth <= 0)
        {
            LoseLevel();
        }
    }

    public void WinLevel()
    {
        if (gameEnded)
        {
            return;
        }

        gameEnded = true;
        Debug.Log("YOU WIN");

        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }
    }

    public void LoseLevel()
    {
        if (gameEnded)
        {
            return;
        }

        gameEnded = true;
        Debug.Log("YOU LOSE");

        if (losePanel != null)
        {
            losePanel.SetActive(true);
        }
    }

    public bool IsGameEnded()
    {
        return gameEnded;
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    private void RefreshHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = "HEALTH: " + currentHealth + "/" + startHealth;
        }
    }
}