using UnityEngine;

public class Plot : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Color hoverColor;

    public GameObject towerObj;
    public TowerBasics tower;
    private Color startColor;

    private void Start()
    {
        startColor = sr.color;
    }

    private void OnMouseEnter()
    {
        sr.color = hoverColor;
    }

    private void OnMouseExit()
    {
        sr.color = startColor;
    }

    private void OnMouseDown()
    {
        if (UIManager.main.IsHoveringUI()) return;

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

        if (towerBasics == null)
        {
            Debug.LogError("Selected tower prefab does not have TowerBasics on it.");
            return;
        }

        if (towerBasics.Cost > LevelManager.main.currency)
        {
            Debug.Log("Not enough money");
            return;
        }

        LevelManager.main.SpendCurrency(towerBasics.Cost);

        towerObj = Instantiate(towerToBuild.prefab, transform.position, Quaternion.identity);
        tower = towerObj.GetComponent<TowerBasics>();

        BuildManager.main.ClearSelectedTower();
    }
}