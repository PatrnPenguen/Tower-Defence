using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TowerBasics : MonoBehaviour
{
    [Header("Common Stats")]
    [SerializeField] protected float damage;
    [SerializeField] protected float range;
    [SerializeField] protected float attackSpeed;
    [SerializeField] protected int buildCost;
    [SerializeField] protected int upgradeCost;
    [SerializeField] protected int sellCost;
    
    [Header("Level Settings")]
    [SerializeField] protected int currentLevel = 1;
    [SerializeField] protected int maxLevel = 3;

    [Header("UI")]
    [SerializeField] protected GameObject upgradeUI;
    [SerializeField] protected Button upgradeButton;
    [SerializeField] protected Button sellButton;
    [SerializeField] protected TextMeshProUGUI upgradeButtonText;
    [SerializeField] protected TextMeshProUGUI sellButtonText;

    public float Damage => damage;
    public float Range => range;
    public float AttackSpeed => attackSpeed;
    public int BuildCost => buildCost;
    public int UpgradeCost => upgradeCost;
    public int SellCost => sellCost;

    protected virtual void Start()
    {
        if (upgradeButton != null)
        {
            upgradeButton.onClick.AddListener(UpgradeTower);
        }

        if (sellButton != null)
        {
            sellButton.onClick.AddListener(SellTower);
        }

        UpdateButtonTexts();
    }

    public virtual void OpenUpgradeUI()
    {
        if (upgradeUI != null)
        {
            upgradeUI.SetActive(true);
        }
    }

    public virtual void CloseUpgradeUI()
    {
        if (upgradeUI != null)
        {
            upgradeUI.SetActive(false);
        }

        UIManager.main.SetHoveringState(false);
    }
    
    protected bool CanUpgrade()
    {
        return currentLevel < maxLevel;
    }
    
    protected void UpdateButtonTexts()
    {
        if (upgradeButton != null)
        {
            upgradeButton.interactable = CanUpgrade();
        }
        
        if (CanUpgrade())
        {
            upgradeButtonText.text = "Upgrade" + "\n" + upgradeCost;
        }
        else
        {
            upgradeButtonText.text = "Max Level";
        }

        if (sellButtonText != null)
        {
            sellButtonText.text = "Sell" + "\n" + sellCost;
        }
    }
    
    public virtual void SellTower()
    {
        LevelManager.main.IncreaseCurrency(sellCost);

        if (Plot.main != null)
        {
            Plot.main.towerObj = null;
            Plot.main.tower = null;
        }

        CloseUpgradeUI();
        Destroy(gameObject);
    }

    public virtual void UpgradeTower()
    {
        if (LevelManager.main.currency < upgradeCost)
        {
            Debug.Log("Not enough money to upgrade");
            return;
        }

        LevelManager.main.SpendCurrency(upgradeCost);

        damage += 1f;
        range += 0.3f;
        attackSpeed += 0.2f;

        upgradeCost += 25;
        sellCost += 25;

        Debug.Log(gameObject.name + " upgraded");
        UpdateButtonTexts();
    }
}