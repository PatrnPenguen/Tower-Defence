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
    [SerializeField] private GameObject[] starObjects;

    [Header("Colors")]
    [SerializeField] private Color unlockedColor = Color.white;
    [SerializeField] private Color lockedColor = Color.gray;

    private MainMenuSceneManager mainMenuSceneManager;

    public void Setup(MainMenuSceneManager menuManager)
    {
        mainMenuSceneManager = menuManager;
        RefreshButton();
    }

    public void RefreshButton()
    {
        bool isUnlocked = LevelProgressManager.IsLevelUnlocked(levelNumber);

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
            levelText.color = isUnlocked ? Color.white : new Color(0.7f, 0.7f, 0.7f, 1f);
        }

        int savedStars = LevelProgressManager.GetStars(levelNumber);

        if (starObjects != null)
        {
            for (int i = 0; i < starObjects.Length; i++)
            {
                if (starObjects[i] != null)
                {
                    starObjects[i].SetActive(i < savedStars);
                }
            }
        }
    }

    public void OnClickLevelButton()
    {
        if (!LevelProgressManager.IsLevelUnlocked(levelNumber))
        {
            return;
        }

        if (mainMenuSceneManager != null)
        {
            mainMenuSceneManager.LoadLevel(sceneName);
        }
    }
}