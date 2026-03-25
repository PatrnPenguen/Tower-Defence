using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettingsUI : MonoBehaviour
{
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private string musicVolumeParameter = "MusicVolume";
    [SerializeField] private string sfxVolumeParameter = "SFXVolume";

    [Header("UI")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private TextMeshProUGUI musicPercentText;
    [SerializeField] private TextMeshProUGUI sfxPercentText;

    [Header("Default Values")]
    [SerializeField] [Range(0.0001f, 1f)] private float defaultMusicVolume = 0.8f;
    [SerializeField] [Range(0.0001f, 1f)] private float defaultSfxVolume = 0.8f;

    private const string MusicPrefKey = "MusicVolume";
    private const string SfxPrefKey = "SFXVolume";

    private void Start()
    {
        float savedMusic = PlayerPrefs.GetFloat(MusicPrefKey, defaultMusicVolume);
        float savedSfx = PlayerPrefs.GetFloat(SfxPrefKey, defaultSfxVolume);

        if (musicSlider != null)
        {
            musicSlider.minValue = 0.0001f;
            musicSlider.maxValue = 1f;
            musicSlider.value = savedMusic;
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        if (sfxSlider != null)
        {
            sfxSlider.minValue = 0.0001f;
            sfxSlider.maxValue = 1f;
            sfxSlider.value = savedSfx;
            sfxSlider.onValueChanged.AddListener(SetSfxVolume);
        }

        SetMusicVolume(savedMusic);
        SetSfxVolume(savedSfx);
    }

    public void SetMusicVolume(float value)
    {
        value = Mathf.Clamp(value, 0.0001f, 1f);

        if (audioMixer != null)
        {
            audioMixer.SetFloat(musicVolumeParameter, Mathf.Log10(value) * 20f);
        }

        if (musicPercentText != null)
        {
            musicPercentText.text = Mathf.RoundToInt(value * 100f) + "%";
        }

        PlayerPrefs.SetFloat(MusicPrefKey, value);
        PlayerPrefs.Save();
    }

    public void SetSfxVolume(float value)
    {
        value = Mathf.Clamp(value, 0.0001f, 1f);

        if (audioMixer != null)
        {
            audioMixer.SetFloat(sfxVolumeParameter, Mathf.Log10(value) * 20f);
        }

        if (sfxPercentText != null)
        {
            sfxPercentText.text = Mathf.RoundToInt(value * 100f) + "%";
        }

        PlayerPrefs.SetFloat(SfxPrefKey, value);
        PlayerPrefs.Save();
    }

    public void ResetToDefaults()
    {
        if (musicSlider != null)
        {
            musicSlider.value = defaultMusicVolume;
        }

        if (sfxSlider != null)
        {
            sfxSlider.value = defaultSfxVolume;
        }

        SetMusicVolume(defaultMusicVolume);
        SetSfxVolume(defaultSfxVolume);
    }
}