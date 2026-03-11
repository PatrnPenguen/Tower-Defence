using UnityEngine;
using UnityEngine.UI;

public class TowerBasics : MonoBehaviour
{
    [Header("Common Stats")]
    [SerializeField] protected float damage;
    [SerializeField] protected float range;
    [SerializeField] protected float attackSpeed;
    [SerializeField] protected int cost;
    [SerializeField] protected int upgradeCost;
    [SerializeField] protected int sellCost;

    [Header("UI")]
    [SerializeField] protected GameObject upgradeUI;
    [SerializeField] protected Button upgradeButton;

    public float Damage => damage;
    public float Range => range;
    public float AttackSpeed => attackSpeed;
    public int Cost => cost;
    public int UpgradeCost => upgradeCost;
    public int SellCost => sellCost;

    protected virtual void Start()
    {
        if (upgradeButton != null)
        {
            upgradeButton.onClick.AddListener(UpgradeTower);
        }
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
    }

    public virtual void SellTower()
    {
        LevelManager.main.IncreaseCurrency(sellCost);
        Destroy(gameObject);
    }
}