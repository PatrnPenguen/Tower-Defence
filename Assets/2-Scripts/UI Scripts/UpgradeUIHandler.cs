using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeUIHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool mouse_over = false;

    private TowerBasics ownerTower;

    public void SetOwnerTower(TowerBasics tower)
    {
        ownerTower = tower;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouse_over = true;
        UIManager.main.SetHoveringState(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouse_over = false;
        UIManager.main.SetHoveringState(false);

        if (ownerTower != null)
        {
            ownerTower.CloseUpgradeUI();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}