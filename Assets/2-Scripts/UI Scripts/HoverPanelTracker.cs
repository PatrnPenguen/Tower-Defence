using UnityEngine;
using UnityEngine.EventSystems;

public class HoverPanelTracker : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject targetPanel;

    private int hoverCount = 0;

    private void Start()
    {
        if (targetPanel != null)
        {
            targetPanel.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hoverCount++;

        if (targetPanel != null)
        {
            targetPanel.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hoverCount--;
        
        if (hoverCount <= 0)
        {
            hoverCount = 0;

            if (targetPanel != null)
            {
                targetPanel.SetActive(false);
            }
        }
    }

    public void ForceShow()
    {
        hoverCount++;

        if (targetPanel != null)
        {
            targetPanel.SetActive(true);
        }
    }

    public void ForceHide()
    {
        hoverCount--;

        if (hoverCount <= 0)
        {
            hoverCount = 0;

            if (targetPanel != null)
            {
                targetPanel.SetActive(false);
            }
        }
    }
}