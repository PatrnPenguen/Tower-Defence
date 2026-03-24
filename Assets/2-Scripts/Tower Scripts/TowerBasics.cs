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
    
    [Header("Common Stats Upgrades")]
    [SerializeField] protected float damageUp = 1f;
    [SerializeField] protected float rangeUp = 0.3f;
    [SerializeField] protected float attackSpeedUp = 0.2f;
    [SerializeField] protected int upgradeCostUp = 25;
    [SerializeField] protected int sellCostUp = 25;

    [Header("Level Settings")]
    [SerializeField] protected int currentLevel = 1;
    [SerializeField] protected int maxLevel = 3;

    [Header("Tower Visual References")]
    [SerializeField] protected SpriteRenderer baseSpriteRenderer;
    [SerializeField] protected Animator weaponAnimator;
    [SerializeField] protected GameObject rangeIndicator;

    [Header("Animation Speed Settings")]
    [SerializeField] protected float baseWeaponAnimationSpeed = 1f;

    [Header("Shoot SFX")]
    [SerializeField] protected AudioSource shootAudioSource;
    [SerializeField] protected AudioClip shootSfx;
    [SerializeField] [Range(0f, 1f)] protected float shootSfxVolume = 1f;

    protected float baseAttackSpeed;
    protected Plot ownerPlot;

    [Header("Level Visual Data")]
    [SerializeField] protected TowerLevelVisualData[] levelVisuals;

    [Header("UI")]
    [SerializeField] protected GameObject upgradeUI;
    [SerializeField] protected Button upgradeButton;
    [SerializeField] protected Button sellButton;
    [SerializeField] protected TextMeshProUGUI upgradeButtonText;
    [SerializeField] protected TextMeshProUGUI sellButtonText;

    private static TowerBasics selectedTower;

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

        if (shootAudioSource == null)
        {
            shootAudioSource = GetComponent<AudioSource>();
        }

        if (upgradeButton != null)
        {
            upgradeButton.onClick.AddListener(UpgradeTower);
        }

        if (sellButton != null)
        {
            sellButton.onClick.AddListener(SellTower);
        }

        ApplyLevelVisuals();
        UpdateWeaponAnimationSpeed();
        UpdateButtonTexts();
        HideRange();

        if (upgradeUI != null)
        {
            upgradeUI.SetActive(false);
        }
    }

    public void SetOwnerPlot(Plot plot)
    {
        ownerPlot = plot;
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

        PlayShootSfx();
    }

    protected void PlayShootSfx()
    {
        if (shootAudioSource == null || shootSfx == null)
        {
            return;
        }

        shootAudioSource.PlayOneShot(shootSfx, shootSfxVolume);
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
        if (selectedTower != null && selectedTower != this)
        {
            selectedTower.CloseUpgradeUI();
        }

        selectedTower = this;

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

        if (selectedTower == this)
        {
            selectedTower = null;
        }

        if (UIManager.main != null)
        {
            UIManager.main.SetHoveringState(false);
        }
    }

    public static void CloseSelectedTowerUI()
    {
        if (selectedTower != null)
        {
            selectedTower.CloseUpgradeUI();
        }
    }

    protected bool CanUpgrade()
    {
        return currentLevel < maxLevel && LevelManager.main.currency >= upgradeCost;
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
                if (currentLevel == maxLevel)
                {
                    upgradeButtonText.text = "Max Level";
                }
                else
                {
                    upgradeButtonText.text = "Upgrade" + "\n" + upgradeCost;
                }
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

        if (ownerPlot != null)
        {
            ownerPlot.ClearPlot();
        }

        CloseUpgradeUI();
        HideRange();
        Destroy(gameObject);
    }

    public virtual void UpgradeTower()
    {
        if (!CanUpgrade())
        {
            return;
        }

        if (LevelManager.main.currency < upgradeCost)
        {
            return;
        }

        LevelManager.main.SpendCurrency(upgradeCost);

        damage += damageUp;
        range += rangeUp;
        attackSpeed += attackSpeedUp;

        upgradeCost += upgradeCostUp;
        sellCost += sellCostUp;

        currentLevel++;

        ApplyLevelVisuals();
        UpdateWeaponAnimationSpeed();
        UpdateButtonTexts();

        if (rangeIndicator != null && rangeIndicator.activeSelf)
        {
            ShowRange();
        }
    }
    
    public void ShowRange()
    {
        if (rangeIndicator == null)
        {
            return;
        }

        rangeIndicator.SetActive(true);

        float diameter = range;
        rangeIndicator.transform.localScale = new Vector3(diameter, diameter, 1f);
    }

    public void HideRange()
    {
        if (rangeIndicator == null)
        {
            return;
        }

        rangeIndicator.SetActive(false);
    }

    public bool IsRangeVisible()
    {
        if (rangeIndicator == null)
        {
            return false;
        }

        return rangeIndicator.activeSelf;
    }

    public void ToggleRange()
    {
        if (rangeIndicator == null)
        {
            return;
        }

        if (rangeIndicator.activeSelf)
        {
            HideRange();
        }
        else
        {
            ShowRange();
        }
    }
}