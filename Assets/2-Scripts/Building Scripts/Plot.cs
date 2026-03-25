using System;
using UnityEngine;

public class Plot : MonoBehaviour
{
    [Header("Plot Save")]
    [SerializeField] private int plotId = 0;

    [Header("References")]
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Color hoverColor;
    [SerializeField] private Color towerBuildColor;
    [SerializeField] private GameObject notEnoughMoneyText;

    [NonSerialized] public GameObject towerObj;
    [NonSerialized] public TowerBasics tower;

    private Color startColor;
    private bool isTowerBuilded = false;
    private int builtTowerIndex = -1;

    private void Start()
    {
        startColor = sr.color;
    }

    private void OnMouseEnter()
    {
        if (UIManager.main.IsHoveringUI())
        {
            return;
        }

        sr.color = hoverColor;
    }

    private void OnMouseOver()
    {
        if (UIManager.main.IsHoveringUI())
        {
            return;
        }

        if (tower != null && Input.GetMouseButtonDown(1))
        {
            tower.ToggleRange();
        }
    }

    private void OnMouseExit()
    {
        sr.color = isTowerBuilded ? towerBuildColor : startColor;

        if (tower != null)
        {
            tower.HideRange();
        }
    }

    private void OnMouseDown()
    {
        if (UIManager.main.IsHoveringUI())
        {
            return;
        }

        if (LevelManager.main != null && LevelManager.main.IsGameEnded())
        {
            return;
        }

        if (Input.GetMouseButtonDown(1))
        {
            return;
        }

        if (towerObj != null)
        {
            if (tower != null)
            {
                tower.OpenUpgradeUI();
            }
            return;
        }

        TowerBasics.CloseSelectedTowerUI();

        if (!BuildManager.main.HasTowerSelected())
        {
            return;
        }

        int selectedTowerIndex = BuildManager.main.GetSelectedTowerIndex();
        TowerData towerToBuild = BuildManager.main.GetSelectedTower();

        if (towerToBuild == null || towerToBuild.prefab == null)
        {
            return;
        }

        TowerBasics towerBasics = towerToBuild.prefab.GetComponent<TowerBasics>();

        if (towerBasics.BuildCost > LevelManager.main.currency)
        {
            if (notEnoughMoneyText != null)
            {
                Instantiate(notEnoughMoneyText, transform.position, Quaternion.identity);
            }
            return;
        }

        LevelManager.main.SpendCurrency(towerBasics.BuildCost);

        towerObj = Instantiate(towerToBuild.prefab, transform.position, Quaternion.identity);
        tower = towerObj.GetComponent<TowerBasics>();

        if (tower != null)
        {
            tower.SetOwnerPlot(this);
        }

        builtTowerIndex = selectedTowerIndex;
        sr.color = towerBuildColor;
        isTowerBuilded = true;

        BuildManager.main.ClearSelectedTower();
    }

    public void BuildTowerFromSave(int towerIndex, int towerLevel)
    {
        if (towerObj != null)
        {
            return;
        }

        TowerData towerToBuild = BuildManager.main.GetTowerDataByIndex(towerIndex);

        if (towerToBuild == null || towerToBuild.prefab == null)
        {
            return;
        }

        towerObj = Instantiate(towerToBuild.prefab, transform.position, Quaternion.identity);
        tower = towerObj.GetComponent<TowerBasics>();

        if (tower != null)
        {
            tower.SetOwnerPlot(this);
            tower.ApplySavedLevel(towerLevel);
        }

        builtTowerIndex = towerIndex;
        isTowerBuilded = true;
        sr.color = towerBuildColor;
    }

    public void ClearPlot()
    {
        towerObj = null;
        tower = null;
        builtTowerIndex = -1;
        isTowerBuilded = false;
        sr.color = startColor;
    }

    public void ClearPlotForLoad()
    {
        if (towerObj != null)
        {
            Destroy(towerObj);
        }

        towerObj = null;
        tower = null;
        builtTowerIndex = -1;
        isTowerBuilded = false;
        sr.color = startColor;
    }

    public int GetPlotId()
    {
        return plotId;
    }

    public bool HasTower()
    {
        return towerObj != null && tower != null;
    }

    public TowerBasics GetTower()
    {
        return tower;
    }

    public int GetBuiltTowerIndex()
    {
        return builtTowerIndex;
    }
}