using System;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    [Header("Player Damage SFX")]
    [SerializeField] private AudioSource damageAudioSource;
    [SerializeField] private AudioClip damageSfx;
    [SerializeField] [Range(0f, 1f)] private float damageSfxVolume = 1f;

    [Header("Scene Settings")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private string nextLevelSceneName = "";

    [Header("Level Progress")]
    [SerializeField] private int currentLevelNumber = 1;

    [Header("Win Stars")]
    [SerializeField] private GameObject star1;
    [SerializeField] private GameObject star2;
    [SerializeField] private GameObject star3;

    private int currentHealth;
    private bool gameEnded = false;
    private bool saveStateLoaded = false;

    private void Awake()
    {
        main = this;

        // Default values are set once here.
        // If there is a save, GameSaveManager will overwrite these later.
        currency = startCurrency;
        currentHealth = startHealth;
    }

    private void Start()
    {
        RefreshHealthUI();

        if (damageAudioSource == null)
        {
            damageAudioSource = GetComponent<AudioSource>();
        }

        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }

        if (losePanel != null)
        {
            losePanel.SetActive(false);
        }

        HideAllStars();
        InitializeLevelProgress();
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

        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        RefreshHealthUI();
        PlayDamageSfx();

        if (currentHealth <= 0)
        {
            LoseLevel();
        }
    }

    private void PlayDamageSfx()
    {
        if (damageAudioSource == null || damageSfx == null)
        {
            return;
        }

        damageAudioSource.PlayOneShot(damageSfx, damageSfxVolume);
    }

    public void WinLevel()
    {
        if (gameEnded)
        {
            return;
        }

        gameEnded = true;
        Debug.Log("YOU WIN");

        int earnedStars = CalculateStars();
        ShowStars(earnedStars);
        SaveLevelProgress(earnedStars);

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

    private int CalculateStars()
    {
        if (currentHealth >= 40)
        {
            return 3;
        }

        if (currentHealth >= 25)
        {
            return 2;
        }

        if (currentHealth >= 1)
        {
            return 1;
        }

        return 0;
    }

    private void ShowStars(int starCount)
    {
        HideAllStars();

        if (star1 != null)
        {
            star1.SetActive(starCount >= 1);
        }

        if (star2 != null)
        {
            star2.SetActive(starCount >= 2);
        }

        if (star3 != null)
        {
            star3.SetActive(starCount >= 3);
        }
    }

    private void HideAllStars()
    {
        if (star1 != null)
        {
            star1.SetActive(false);
        }

        if (star2 != null)
        {
            star2.SetActive(false);
        }

        if (star3 != null)
        {
            star3.SetActive(false);
        }
    }

    private void InitializeLevelProgress()
    {
        if (!PlayerPrefs.HasKey("UnlockedLevel"))
        {
            PlayerPrefs.SetInt("UnlockedLevel", 1);
            PlayerPrefs.Save();
        }
    }

    private void SaveLevelProgress(int earnedStars)
    {
        int currentUnlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        if (currentLevelNumber >= currentUnlockedLevel)
        {
            PlayerPrefs.SetInt("UnlockedLevel", currentLevelNumber + 1);
        }

        string starsKey = "LevelStars_" + currentLevelNumber;
        int oldStars = PlayerPrefs.GetInt(starsKey, 0);

        if (earnedStars > oldStars)
        {
            PlayerPrefs.SetInt(starsKey, earnedStars);
        }

        PlayerPrefs.Save();
    }

    public void LoadMainMenu()
    {
        if (string.IsNullOrEmpty(mainMenuSceneName))
        {
            Debug.LogWarning("Main menu scene name is empty.");
            return;
        }

        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void TryAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadNextLevel()
    {
        if (string.IsNullOrEmpty(nextLevelSceneName))
        {
            Debug.LogWarning("Next level scene name is empty.");
            return;
        }

        SceneManager.LoadScene(nextLevelSceneName);
    }

    public int GetUnlockedLevel()
    {
        return PlayerPrefs.GetInt("UnlockedLevel", 1);
    }

    public int GetSavedStarsForLevel(int levelNumber)
    {
        return PlayerPrefs.GetInt("LevelStars_" + levelNumber, 0);
    }

    public int GetCurrency()
    {
        return currency;
    }

    public void LoadSaveState(int savedCurrency, int savedHealth)
    {
        currency = savedCurrency;
        currentHealth = savedHealth;
        saveStateLoaded = true;
        RefreshHealthUI();
    }

    public bool HasLoadedSaveState()
    {
        return saveStateLoaded;
    }
}