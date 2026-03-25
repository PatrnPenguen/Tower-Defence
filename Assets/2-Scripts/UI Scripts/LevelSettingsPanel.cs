using UnityEngine;
using UnityEngine.UI;

public class LevelSettingsPanel : MonoBehaviour
{
    [Header("Panels & Buttons")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Button pausePlayButton;

    [Header("Pause Settings")]
    [SerializeField] private bool pauseGameWhenOpen = true;

    private bool isOpen = false;

    private void Start()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        Time.timeScale = 1f;
        isOpen = false;
    }

    public void OpenSettingsPanel()
    {
        if (settingsPanel == null)
        {
            return;
        }

        settingsPanel.SetActive(true);
        isOpen = true;
        pausePlayButton.interactable = false;

        if (pauseGameWhenOpen)
        {
            Time.timeScale = 0f;
        }
    }

    public void CloseSettingsPanel()
    {
        if (settingsPanel == null)
        {
            return;
        }

        settingsPanel.SetActive(false);
        isOpen = false;
        pausePlayButton.interactable = true;

        if (pauseGameWhenOpen)
        {
            Time.timeScale = 1f;
        }
    }

    public void ToggleSettingsPanel()
    {
        if (isOpen)
        {
            CloseSettingsPanel();
        }
        else
        {
            OpenSettingsPanel();
        }
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;

        if (LevelManager.main != null)
        {
            LevelManager.main.LoadMainMenu();
        }
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;

        if (LevelManager.main != null)
        {
            LevelManager.main.TryAgain();
        }
    }
}