using UnityEngine;

public static class LevelProgressManager
{
    private const string UnlockedLevelKey = "UnlockedLevel";
    private const string StarsKeyPrefix = "LevelStars_";

    public static void InitializeProgress()
    {
        if (!PlayerPrefs.HasKey(UnlockedLevelKey))
        {
            PlayerPrefs.SetInt(UnlockedLevelKey, 1);
            PlayerPrefs.Save();
        }
    }

    public static int GetUnlockedLevel()
    {
        InitializeProgress();
        return PlayerPrefs.GetInt(UnlockedLevelKey, 1);
    }

    public static bool IsLevelUnlocked(int levelNumber)
    {
        return levelNumber <= GetUnlockedLevel();
    }

    public static void UnlockLevel(int levelNumber)
    {
        InitializeProgress();

        int currentUnlocked = GetUnlockedLevel();

        if (levelNumber > currentUnlocked)
        {
            PlayerPrefs.SetInt(UnlockedLevelKey, levelNumber);
            PlayerPrefs.Save();
        }
    }

    public static void SaveStars(int levelNumber, int stars)
    {
        int oldStars = GetStars(levelNumber);

        if (stars > oldStars)
        {
            PlayerPrefs.SetInt(StarsKeyPrefix + levelNumber, stars);
            PlayerPrefs.Save();
        }
    }

    public static int GetStars(int levelNumber)
    {
        return PlayerPrefs.GetInt(StarsKeyPrefix + levelNumber, 0);
    }

    public static void ResetAllProgress()
    {
        PlayerPrefs.DeleteKey(UnlockedLevelKey);

        for (int i = 1; i <= 50; i++)
        {
            PlayerPrefs.DeleteKey(StarsKeyPrefix + i);
        }

        PlayerPrefs.Save();
        InitializeProgress();
    }
}