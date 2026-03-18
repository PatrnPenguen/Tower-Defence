using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class TowerLevelVisualData
{
    public Sprite baseSprite;
    public RuntimeAnimatorController weaponAnimatorController;
    public GameObject projectilePrefab;
}

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

    [Header("Tower Visual References")]
    [SerializeField] protected SpriteRenderer baseSpriteRenderer;
    [SerializeField] protected Animator weaponAnimator;
    
    [Header("Animation Speed Settings")]
    [SerializeField] protected float baseWeaponAnimationSpeed = 1f;

    protected float baseAttackSpeed;

    [Header("Level Visual Data")]
    [SerializeField] protected TowerLevelVisualData[] levelVisuals;

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
    public int CurrentLevel => currentLevel;

    protected virtual void Start()
    {
        baseAttackSpeed = attackSpeed;
        
        if (upgradeButton != null)
        {
            upgradeButton.onClick.AddListener(UpgradeTower);
        }

        if (sellButton != null)
        {
            sellButton.onClick.AddListener(SellTower);
        }

        ApplyLevelVisuals();
        UpdateButtonTexts();
    }

    protected int GetLevelIndex()
    {
        return Mathf.Clamp(currentLevel - 1, 0, maxLevel - 1);
    }

    protected void ApplyLevelVisuals()
    {
        if (levelVisuals == null || levelVisuals.Length == 0)
        {
            return;
        }

        int levelIndex = GetLevelIndex();

        if (levelIndex >= levelVisuals.Length)
        {
            levelIndex = levelVisuals.Length - 1;
        }

        TowerLevelVisualData currentVisualData = levelVisuals[levelIndex];

        if (currentVisualData == null)
        {
            return;
        }

        if (baseSpriteRenderer != null && currentVisualData.baseSprite != null)
        {
            baseSpriteRenderer.sprite = currentVisualData.baseSprite;
        }

        if (weaponAnimator != null && currentVisualData.weaponAnimatorController != null)
        {
            weaponAnimator.runtimeAnimatorController = currentVisualData.weaponAnimatorController;
        }
    }
    
    protected void PlayWeaponShootAnimation()
    {
        if (weaponAnimator != null)
        {
            weaponAnimator.SetTrigger("Shoot");
        }
    }
    
    protected void UpdateWeaponAnimationSpeed()
    {
        if (weaponAnimator == null)
        {
            return;
        }

        if (baseAttackSpeed <= 0f)
        {
            weaponAnimator.speed = baseWeaponAnimationSpeed;
            return;
        }

        float attackSpeedMultiplier = attackSpeed / baseAttackSpeed;
        weaponAnimator.speed = baseWeaponAnimationSpeed * attackSpeedMultiplier;
    }

    protected GameObject GetCurrentProjectilePrefab()
    {
        if (levelVisuals == null || levelVisuals.Length == 0)
        {
            return null;
        }

        int levelIndex = GetLevelIndex();

        if (levelIndex >= levelVisuals.Length)
        {
            levelIndex = levelVisuals.Length - 1;
        }

        TowerLevelVisualData currentVisualData = levelVisuals[levelIndex];

        if (currentVisualData == null)
        {
            return null;
        }

        return currentVisualData.projectilePrefab;
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

        if (UIManager.main != null)
        {
            UIManager.main.SetHoveringState(false);
        }
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

        if (upgradeButtonText != null)
        {
            if (CanUpgrade())
            {
                upgradeButtonText.text = "Upgrade" + "\n" + upgradeCost;
            }
            else
            {
                upgradeButtonText.text = "Max Level";
            }
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
        if (!CanUpgrade())
        {
            Debug.Log("Tower is already at max level");
            return;
        }

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

        currentLevel++;

        ApplyLevelVisuals();
        UpdateWeaponAnimationSpeed();
        UpdateButtonTexts();

        Debug.Log(gameObject.name + " upgraded");
    }
}