using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopTowerButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Button Settings")]
    [SerializeField] private int towerIndex;

    [Header("References")]
    [SerializeField] private GameObject selectedObject;
    [SerializeField] private TextMeshProUGUI buttonText;

    [Header("Hover Info")]
    [SerializeField] private HoverPanelTracker hoverPanelTracker;

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
        if (selectedObject == null)
        {
            return;
        }

        selectedObject.SetActive(BuildManager.main.IsTowerSelected(towerIndex));
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverPanelTracker != null)
        {
            hoverPanelTracker.ForceShow();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (hoverPanelTracker != null)
        {
            hoverPanelTracker.ForceHide();
        }
    }
}