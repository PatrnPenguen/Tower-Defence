using UnityEngine;

public class PPButon : MonoBehaviour
{
    [Header("Button Visuals")]
    [SerializeField] private GameObject PlayImage;
    [SerializeField] private GameObject PauseImage;
    
    private bool isPaused = false;
    
    private void Start()
    {
        Time.timeScale = 1f;
        PauseImage.SetActive(true);
        PlayImage.SetActive(false);
    }

    public void ButtonAction()
    {
        if (!isPaused)
        {
            Time.timeScale = 0f;
            isPaused = true;
            PlayImage.SetActive(true);
            PauseImage.SetActive(false);
        }
        else
        {
            Time.timeScale = 1f;
            isPaused = false;
            PauseImage.SetActive(true);
            PlayImage.SetActive(false);
        }
    }
    
}
