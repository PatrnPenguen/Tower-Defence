using System;
using UnityEngine;

public class Plot : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Color hoverColor;
    [SerializeField] private Color towerBuildColor;
    [SerializeField] private GameObject notEnoughMoneyText;

    [NonSerialized] public GameObject towerObj;
    [NonSerialized] public TowerBasics tower;

    private Color startColor;
    private bool isTowerBuilded = false;

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

        sr.color = towerBuildColor;
        isTowerBuilded = true;

        BuildManager.main.ClearSelectedTower();
    }

    public void ClearPlot()
    {
        towerObj = null;
        tower = null;
        isTowerBuilded = false;
        sr.color = startColor;
    }
}