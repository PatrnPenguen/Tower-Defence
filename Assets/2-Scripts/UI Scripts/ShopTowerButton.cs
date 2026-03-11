using UnityEngine;
using UnityEngine.UI;

public class ShopTowerButton : MonoBehaviour
{
    [Header("Button Settings")]
    [SerializeField] private int towerIndex;

    [Header("Visual References")]
    [SerializeField] private Image borderImage;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = Color.yellow;

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
}