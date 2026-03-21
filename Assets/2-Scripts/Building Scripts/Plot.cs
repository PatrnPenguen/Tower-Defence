using System;
using System.Collections;
using UnityEngine;

public class Plot : MonoBehaviour
{
    public static Plot main;
    [Header("References")]
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Color hoverColor;
    [SerializeField] private Color towerBuildColor;
    [SerializeField] private GameObject notEnoughMoneyText;

    [NonSerialized] public GameObject towerObj;
    [NonSerialized] public TowerBasics tower;
    private Color startColor;
    private bool isTowerBuilded = false;

    private void Awake()
    {
        main = this;
    }

    private void Start()
    {
        startColor = sr.color;
    }

    private void OnMouseEnter()
    {
        if (UIManager.main.IsHoveringUI()) return;

        sr.color = hoverColor;
    }

    private void OnMouseExit()
    {
        sr.color = isTowerBuilded ? towerBuildColor : startColor;
    }

    private void OnMouseDown()
{
    if (UIManager.main.IsHoveringUI()) return;

    if (LevelManager.main != null && LevelManager.main.IsGameEnded())
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
    sr.color = towerBuildColor;
    isTowerBuilded = true;
    BuildManager.main.ClearSelectedTower();
}
}