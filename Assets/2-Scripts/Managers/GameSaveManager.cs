using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class TowerSaveData
{
    public int plotId;
    public int towerIndex;
    public int towerLevel;
}

[System.Serializable]
public class GameSaveData
{
    public string sceneName;
    public int currency;
    public int health;
    public int completedWaveIndex;
    public List<TowerSaveData> towers = new List<TowerSaveData>();
}

public class GameSaveManager : MonoBehaviour
{
    public static GameSaveManager main;

    [Header("Scene Plots")]
    [SerializeField] private Plot[] scenePlots;

    private const string SaveKey = "GameSaveData";

    private void Awake()
    {
        if (main == null)
        {
            main = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator Start()
    {
        yield return null;
        LoadGameIfExists();
    }

    public void SaveGame()
    {
        if (LevelManager.main == null || WaveManager.main == null)
        {
            return;
        }

        GameSaveData saveData = new GameSaveData();

        saveData.sceneName = SceneManager.GetActiveScene().name;
        saveData.currency = LevelManager.main.GetCurrency();
        saveData.health = LevelManager.main.GetCurrentHealth();
        saveData.completedWaveIndex = WaveManager.main.GetCurrentWaveIndex();

        if (scenePlots != null)
        {
            for (int i = 0; i < scenePlots.Length; i++)
            {
                Plot plot = scenePlots[i];

                if (plot == null || !plot.HasTower())
                {
                    continue;
                }

                TowerBasics tower = plot.GetTower();

                if (tower == null)
                {
                    continue;
                }

                TowerSaveData towerData = new TowerSaveData();
                towerData.plotId = plot.GetPlotId();
                towerData.towerIndex = plot.GetBuiltTowerIndex();
                towerData.towerLevel = tower.CurrentLevel;

                saveData.towers.Add(towerData);
            }
        }

        string json = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();

        Debug.Log("Game saved.");
    }

    public void LoadGameIfExists()
    {
        if (!PlayerPrefs.HasKey(SaveKey))
        {
            return;
        }

        string json = PlayerPrefs.GetString(SaveKey);

        if (string.IsNullOrEmpty(json))
        {
            return;
        }

        GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);

        if (saveData == null)
        {
            return;
        }

        if (saveData.sceneName != SceneManager.GetActiveScene().name)
        {
            return;
        }

        if (LevelManager.main != null)
        {
            LevelManager.main.LoadSaveState(saveData.currency, saveData.health);
        }

        if (WaveManager.main != null)
        {
            WaveManager.main.LoadSaveWaveState(saveData.completedWaveIndex);
        }

        ClearAllPlots();

        if (saveData.towers != null)
        {
            for (int i = 0; i < saveData.towers.Count; i++)
            {
                TowerSaveData towerData = saveData.towers[i];
                Plot plot = GetPlotById(towerData.plotId);

                if (plot == null)
                {
                    continue;
                }

                plot.BuildTowerFromSave(towerData.towerIndex, towerData.towerLevel);
            }
        }

        Debug.Log("Game loaded.");
    }

    private Plot GetPlotById(int plotId)
    {
        if (scenePlots == null)
        {
            return null;
        }

        for (int i = 0; i < scenePlots.Length; i++)
        {
            if (scenePlots[i] != null && scenePlots[i].GetPlotId() == plotId)
            {
                return scenePlots[i];
            }
        }

        return null;
    }

    private void ClearAllPlots()
    {
        if (scenePlots == null)
        {
            return;
        }

        for (int i = 0; i < scenePlots.Length; i++)
        {
            if (scenePlots[i] != null)
            {
                scenePlots[i].ClearPlotForLoad();
            }
        }
    }

    public bool HasSave()
    {
        return PlayerPrefs.HasKey(SaveKey);
    }

    public void DeleteSave()
    {
        PlayerPrefs.DeleteKey(SaveKey);
        PlayerPrefs.Save();
    }
}