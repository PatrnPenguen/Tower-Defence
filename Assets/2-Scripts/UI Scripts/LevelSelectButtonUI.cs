using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectButtonUI : MonoBehaviour
{
    [Header("Level Info")]
    [SerializeField] private int levelNumber = 1;
    [SerializeField] private string sceneName;

    [Header("References")]
    [SerializeField] private Button button;
    [SerializeField] private Image buttonImage;
    [SerializeField] private TextMeshProUGUI levelText;

    [Header("Star References")]
    //[SerializeField] private SpriteRenderer star1Outline;
    [SerializeField] private GameObject star1Inside;
    //[SerializeField] private SpriteRenderer star2Outline;
    [SerializeField] private GameObject star2Inside;
    //[SerializeField] private SpriteRenderer star3Outline;
    [SerializeField] private GameObject star3Inside;
    
    [Header("Colors")]
    [SerializeField] private Color unlockedColor = Color.white;
    [SerializeField] private Color lockedColor = Color.gray;
    [SerializeField] private Color unlockedTextColor = Color.white;
    [SerializeField] private Color lockedTextColor = Color.gray;

    private MainMenuSceneManager mainMenuSceneManager;

    public void Setup(MainMenuSceneManager menuManager)
    {
        mainMenuSceneManager = menuManager;
        RefreshButton();
    }

    public void RefreshButton()
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
        bool isUnlocked = levelNumber <= unlockedLevel;

        if (button != null)
        {
            button.interactable = isUnlocked;
        }

        if (buttonImage != null)
        {
            buttonImage.color = isUnlocked ? unlockedColor : lockedColor;
        }

        if (levelText != null)
        {
            levelText.color = isUnlocked ? unlockedTextColor : lockedTextColor;
        }

        int savedStars = PlayerPrefs.GetInt("LevelStars_" + levelNumber, 0);

        if (star1Inside != null)
        {
            star1Inside.SetActive(savedStars >= 1);
        }

        if (star2Inside != null)
        {
            star2Inside.SetActive(savedStars >= 2);
        }

        if (star3Inside != null)
        {
            star3Inside.SetActive(savedStars >= 3);
        }
    }

    public void OnClickLevelButton()
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        if (levelNumber > unlockedLevel)
        {
            return;
        }

        if (mainMenuSceneManager != null)
        {
            mainMenuSceneManager.LoadLevel(sceneName);
        }
    }
}