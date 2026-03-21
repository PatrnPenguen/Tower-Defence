using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopTowerButton : MonoBehaviour
{
    [Header("Button Settings")]
    [SerializeField] private int towerIndex;

    [Header("References")]
    [SerializeField] private Image borderImage;
    [SerializeField] private TextMeshProUGUI buttonText;

    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = Color.yellow;

    private void Start()
    {
        UpdateButtonText();
        RefreshVisual();
    }

    public void SelectThisTower()
    {
        BuildManager.main.SetSelectedTower(towerIndex);
    }

    public void RefreshVisual()
    {
        if (borderImage == null) return;

        if (BuildManager.main.IsTowerSelected(towerIndex))
        {
            borderImage.color = selectedColor;
        }
        else
        {
            borderImage.color = normalColor;
        }
    }

    private void UpdateButtonText()
    {
        if (buttonText == null || BuildManager.main == null)
        {
            return;
        }

        TowerData towerData = BuildManager.main.GetTowerDataByIndex(towerIndex);

        if (towerData == null || towerData.prefab == null)
        {
            buttonText.text = "Empty";
            return;
        }

        TowerBasics towerBasics = towerData.prefab.GetComponent<TowerBasics>();

        if (towerBasics == null)
        {
            buttonText.text = towerData.towerName;
            return;
        }

        buttonText.text = towerData.towerName + "\n" + towerBasics.BuildCost;
    }
}