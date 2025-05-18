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
    [SerializeField] private float uiUpdateInterval = 0.2f; // Интервал обновления UI

    private float timer = 0f;
    private float uiUpdateTimer = 0f;

    private void Start()
    {
        if (Clicker.Instance == null)
        {
            Debug.LogError("Clicker instance not found in scene!");
            enabled = false;
            return;
        }

        InitializeUpgrades();
        UpdateUI();
    }

    private void Update()
    {
        // Обновление пассивного дохода
        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            ApplyPassiveIncome();
            timer = 0f;
        }

        // Регулярное обновление UI
        uiUpdateTimer += Time.deltaTime;
        if (uiUpdateTimer >= uiUpdateInterval)
        {
            UpdateButtonsInteractivity();
            uiUpdateTimer = 0f;
        }
    }

    private void InitializeUpgrades()
    {
        if (upgrades == null || upgrades.Length == 0)
        {
            Debug.LogError("Upgrades array is not set or empty!");
            return;
        }

        for (int i = 0; i < upgrades.Length; i++)
        {
            if (upgrades[i] == null) continue;

            upgrades[i].currentLevel = PlayerPrefs.GetInt($"AutoClicker_{i}_Level", 0);
            upgrades[i].currentCost = CalculateUpgradeCost(upgrades[i]);

            // Обновляем UI сразу при инициализации
            UpdateSingleUpgradeUI(i);

            if (upgrades[i].button != null)
            {
                int index = i;
                upgrades[i].button.onClick.RemoveAllListeners();
                upgrades[i].button.onClick.AddListener(() => BuyAutoClicker(index));
            }
        }

        UpdateTotalPassiveProfit();
    }

    private int CalculateUpgradeCost(AutoClickerUpgrade upgrade)
    {
        return upgrade.baseCost * (upgrade.currentLevel + 1);
    }

    private void BuyAutoClicker(int upgradeIndex)
    {
        if (Clicker.Instance == null) return;
        if (upgradeIndex < 0 || upgradeIndex >= upgrades.Length || upgrades[upgradeIndex] == null) return;

        var upgrade = upgrades[upgradeIndex];
        if (Clicker.Instance.Money >= upgrade.currentCost)
        {
            Clicker.Instance.Money -= upgrade.currentCost;
            upgrade.currentLevel++;
            upgrade.currentCost = CalculateUpgradeCost(upgrade);

            PlayerPrefs.SetInt($"AutoClicker_{upgradeIndex}_Level", upgrade.currentLevel);
            PlayerPrefs.Save();

            UpdateTotalPassiveProfit();
            UpdateSingleUpgradeUI(upgradeIndex);
            Clicker.Instance.UpdateUI();
        }
    }

    private void ApplyPassiveIncome()
    {
        if (Clicker.Instance == null || upgrades == null) return;

        float totalProfit = Clicker.Instance.PassiveProfit;
        if (totalProfit > 0)
        {
            Clicker.Instance.Money += totalProfit * updateInterval;
            Debug.Log($"Applied passive income: {totalProfit * updateInterval}");
        }
    }

    private void UpdateTotalPassiveProfit()
    {
        if (Clicker.Instance == null || upgrades == null) return;

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
        UpdateTotalPassiveProfit();
        UpdateAllUpgradeTexts();
        UpdateButtonsInteractivity();
    }

    private void UpdateAllUpgradeTexts()
    {
        if (upgrades == null) return;

        for (int i = 0; i < upgrades.Length; i++)
        {
            UpdateSingleUpgradeUI(i);
        }
    }

    private void UpdateButtonsInteractivity()
    {
        if (upgrades == null || Clicker.Instance == null) return;

        foreach (var upgrade in upgrades)
        {
            if (upgrade != null && upgrade.button != null)
            {
                upgrade.button.interactable = Clicker.Instance.Money >= upgrade.currentCost;
            }
        }
    }

    private void UpdateSingleUpgradeUI(int index)
    {
        if (index < 0 || index >= upgrades.Length || upgrades[index] == null) return;

        var upgrade = upgrades[index];

        // Всегда обновляем текст цены
        if (upgrade.costText != null)
            upgrade.costText.text = $"Цена: {upgrade.currentCost} руб.";

        // Всегда обновляем текст уровня
        if (upgrade.levelText != null)
            upgrade.levelText.text = $"Куплено {upgrade.currentLevel}";

        // Обновляем состояние кнопки
        if (upgrade.button != null)
            upgrade.button.interactable = Clicker.Instance.Money >= upgrade.currentCost;
    }

    [ContextMenu("Сбросить все улучшения")]
    public void ResetAllUpgrades()
    {
        if (upgrades == null) return;

        for (int i = 0; i < upgrades.Length; i++)
        {
            if (upgrades[i] != null)
            {
                upgrades[i].currentLevel = 0;
                upgrades[i].currentCost = upgrades[i].baseCost;
                PlayerPrefs.DeleteKey($"AutoClicker_{i}_Level");

                // Обновляем UI каждого апгрейда после сброса
                UpdateSingleUpgradeUI(i);
            }
        }
        PlayerPrefs.Save();

        UpdateTotalPassiveProfit();

        if (Clicker.Instance != null)
        {
            Clicker.Instance.PassiveProfit = 0;
            Clicker.Instance.UpdateUI();
        }
    }
}