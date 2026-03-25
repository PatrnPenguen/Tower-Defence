using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuSceneManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject levelSelectPanel;
    [SerializeField] private GameObject settingsPanel;

    [Header("Level Buttons")]
    [SerializeField] private LevelSelectButtonUI[] levelButtons;

    [Header("Play Button Levels")]
    [SerializeField] private string level1SceneName = "Level1";
    [SerializeField] private string level2SceneName = "Level2";
    [SerializeField] private string level3SceneName = "Level3";

    private void Start()
    {
        if (!PlayerPrefs.HasKey("UnlockedLevel"))
        {
            PlayerPrefs.SetInt("UnlockedLevel", 1);
            PlayerPrefs.Save();
        }

        if (levelSelectPanel != null)
        {
            levelSelectPanel.SetActive(false);
        }

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        RefreshLevelButtons();
    }

    public void PlayLastUnlockedLevel()
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        if (unlockedLevel <= 1)
        {
            SceneManager.LoadScene(level1SceneName);
            return;
        }

        if (unlockedLevel == 2)
        {
            SceneManager.LoadScene(level2SceneName);
            return;
        }

        SceneManager.LoadScene(level3SceneName);
    }

    public void OpenLevelSelectPanel()
    {
        if (levelSelectPanel != null)
        {
            levelSelectPanel.SetActive(true);
        }

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        RefreshLevelButtons();
    }

    public void CloseLevelSelectPanel()
    {
        if (levelSelectPanel != null)
        {
            levelSelectPanel.SetActive(false);
        }
    }

    public void OpenSettingsPanel()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }

        if (levelSelectPanel != null)
        {
            levelSelectPanel.SetActive(false);
        }
    }

    public void CloseSettingsPanel()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    public void CloseAllPanels()
    {
        if (levelSelectPanel != null)
        {
            levelSelectPanel.SetActive(false);
        }

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    public void LoadLevel(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("Scene name is empty.");
            return;
        }

        SceneManager.LoadScene(sceneName);
    }

    public void RefreshLevelButtons()
    {
        if (levelButtons == null)
        {
            return;
        }

        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (levelButtons[i] != null)
            {
                levelButtons[i].Setup(this);
            }
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ResetProgress()
    {
        PlayerPrefs.DeleteKey("UnlockedLevel");

        for (int i = 1; i <= 50; i++)
        {
            PlayerPrefs.DeleteKey("LevelStars_" + i);
        }

        PlayerPrefs.SetInt("UnlockedLevel", 1);
        PlayerPrefs.Save();

        RefreshLevelButtons();
    }
}