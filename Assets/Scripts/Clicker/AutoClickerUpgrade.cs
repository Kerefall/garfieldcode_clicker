using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AutoClickerUpgradeSystem : MonoBehaviour
{
    [System.Serializable]
    public class AutoClickerUpgrade
    {
        public string upgradeName;
        public int baseCost = 25;
        public float profitPerSecond = 0.1f;
        public Button button;
        public TextMeshProUGUI costText;
        public TextMeshProUGUI levelText;

        [HideInInspector] public int currentLevel = 0;
        [HideInInspector] public int currentCost;
    }

    [Header("Настройки")]
    [SerializeField] private AutoClickerUpgrade[] upgrades;
    [SerializeField] private float updateInterval = 1.0f;

    private float timer = 0f;

    private void Start()
    {
        InitializeUpgrades();
        UpdateUI();
        Update();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            ApplyPassiveIncome();
            timer = 0f;
        }
    }

    private void InitializeUpgrades()
    {
        if (upgrades == null) return;

        for (int i = 0; i < upgrades.Length; i++)
        {
            if (upgrades[i] == null) continue;

            upgrades[i].currentLevel = PlayerPrefs.GetInt($"AutoClicker_{i}_Level", 0);
            upgrades[i].currentCost = upgrades[i].baseCost * (upgrades[i].currentLevel + 1);

            if (upgrades[i].button != null)
            {
                int index = i;
                upgrades[i].button.onClick.RemoveAllListeners();
                upgrades[i].button.onClick.AddListener(() => BuyAutoClicker(index));
            }
        }
    }

    private void BuyAutoClicker(int upgradeIndex)
    {
        if (upgradeIndex < 0 || upgradeIndex >= upgrades.Length || upgrades[upgradeIndex] == null) return;

        AutoClickerUpgrade upgrade = upgrades[upgradeIndex];
        if (Clicker.Instance.Money >= upgrade.currentCost)
        {
            Clicker.Instance.Money -= upgrade.currentCost;
            upgrade.currentLevel++;
            upgrade.currentCost = upgrade.baseCost * (upgrade.currentLevel + 1);

            PlayerPrefs.SetInt($"AutoClicker_{upgradeIndex}_Level", upgrade.currentLevel);
            UpdateTotalPassiveProfit();
            UpdateSingleUpgradeUI(upgradeIndex);
            Clicker.Instance.Update();
        }
    }

    private void ApplyPassiveIncome()
    {
        float totalProfit = 0f;
        foreach (var upgrade in upgrades)
        {
            if (upgrade != null)
            {
                totalProfit += upgrade.profitPerSecond * upgrade.currentLevel;
            }
        }

        if (totalProfit > 0)
        {
            Clicker.Instance.Money += totalProfit * updateInterval;
            Clicker.Instance.Update();
        }
    }

    private void UpdateTotalPassiveProfit()
    {
        float totalProfit = 0f;
        foreach (var upgrade in upgrades)
        {
            if (upgrade != null)
            {
                totalProfit += upgrade.profitPerSecond * upgrade.currentLevel;
            }
        }
        Clicker.Instance.PassiveProfit = totalProfit;
    }

    private void UpdateUI()
    {
        for (int i = 0; i < upgrades.Length; i++)
        {
            UpdateSingleUpgradeUI(i);
        }
    }

    private void UpdateSingleUpgradeUI(int index)
    {
        if (index < 0 || index >= upgrades.Length || upgrades[index] == null) return;

        AutoClickerUpgrade upgrade = upgrades[index];
        if (upgrade.costText != null)
            upgrade.costText.text = upgrade.currentCost.ToString();

        if (upgrade.levelText != null)
            upgrade.levelText.text = $"Ур. {upgrade.currentLevel}";

        if (upgrade.button != null)
            upgrade.button.interactable = Clicker.Instance.Money >= upgrade.currentCost;
    }

    [ContextMenu("Сбросить все улучшения")]
    public void ResetAllUpgrades()
    {
        for (int i = 0; i < upgrades.Length; i++)
        {
            if (upgrades[i] != null)
            {
                upgrades[i].currentLevel = 0;
                upgrades[i].currentCost = upgrades[i].baseCost;
                PlayerPrefs.DeleteKey($"AutoClicker_{i}_Level");
            }
        }
        UpdateTotalPassiveProfit();
        UpdateUI();
    }
}