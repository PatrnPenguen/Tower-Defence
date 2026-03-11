using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager main;

    [Header("References")]
    [SerializeField] private TowerData[] towerPrefabs;

    private int selectedTowerIndex = -1;
    private ShopTowerButton[] shopButtons;

    private void Awake()
    {
        main = this;
    }

    private void Start()
    {
        shopButtons = FindObjectsByType<ShopTowerButton>(FindObjectsSortMode.None);
        UpdateShopButtonVisuals();
    }

    public TowerData GetSelectedTower()
    {
        if (selectedTowerIndex < 0 || selectedTowerIndex >= towerPrefabs.Length)
        {
            return null;
        }

        return towerPrefabs[selectedTowerIndex];
    }

    public void SetSelectedTower(int towerIndex)
    {
        if (towerIndex < 0 || towerIndex >= towerPrefabs.Length)
        {
            return;
        }

        selectedTowerIndex = towerIndex;
        UpdateShopButtonVisuals();
    }

    public void ClearSelectedTower()
    {
        selectedTowerIndex = -1;
        UpdateShopButtonVisuals();
    }

    public bool HasTowerSelected()
    {
        return selectedTowerIndex != -1;
    }

    public bool IsTowerSelected(int towerIndex)
    {
        return selectedTowerIndex == towerIndex;
    }

    private void UpdateShopButtonVisuals()
    {
        if (shopButtons == null) return;

        for (int i = 0; i < shopButtons.Length; i++)
        {
            shopButtons[i].RefreshVisual();
        }
    }
}