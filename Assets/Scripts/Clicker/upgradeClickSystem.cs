using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClickerUpgradeSystem : MonoBehaviour
{
    [System.Serializable]
    public class Upgrade
    {
        public string upgradeName;
        public int baseCost = 10;
        public int clickGain = 1; // Количество добавляемых кликов за уровень
        public Button button;
        public TextMeshProUGUI costText;
        public TextMeshProUGUI levelText;

        [HideInInspector] public int currentLevel = 0;
        [HideInInspector] public int currentCost;
    }

    [Header("Настройки")]
    [SerializeField] private Upgrade[] upgrades;
    [SerializeField] private TextMeshProUGUI clickGainText;

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

    private void InitializeUpgrades()
    {
        if (upgrades == null) return;

        for (int i = 0; i < upgrades.Length; i++)
        {
            if (upgrades[i] == null) continue;

            upgrades[i].currentLevel = PlayerPrefs.GetInt($"Upgrade_{i}_Level", 0);
            upgrades[i].currentCost = CalculateUpgradeCost(upgrades[i]);

            int index = i;
            upgrades[i].button.onClick.RemoveAllListeners();
            upgrades[i].button.onClick.AddListener(() => BuyUpgrade(index));
        }

        // Пересчитываем общий ClickGain после загрузки всех апгрейдов
        CalculateTotalClickGain();
    }

    private int CalculateUpgradeCost(Upgrade upgrade)
    {
        return upgrade.baseCost * (upgrade.currentLevel + 1);
    }

    private void BuyUpgrade(int upgradeIndex)
    {
        if (Clicker.Instance == null) return;
        if (upgradeIndex < 0 || upgradeIndex >= upgrades.Length || upgrades[upgradeIndex] == null) return;

        var upgrade = upgrades[upgradeIndex];
        if (Clicker.Instance.Money >= upgrade.currentCost)
        {
            Clicker.Instance.Money -= upgrade.currentCost;
            upgrade.currentLevel++;
            upgrade.currentCost = CalculateUpgradeCost(upgrade);

            PlayerPrefs.SetInt($"Upgrade_{upgradeIndex}_Level", upgrade.currentLevel);

            // Вместо прямого добавления к ClickGain, пересчитываем общее значение
            CalculateTotalClickGain();

            UpdateSingleUpgradeUI(upgradeIndex);
            Clicker.Instance.UpdateUI();
        }
    }

    // Новый метод для расчета общего ClickGain
    private void CalculateTotalClickGain()
    {
        if (Clicker.Instance == null || upgrades == null) return;

        int totalClickGain = 1; // Базовое значение

        foreach (var upgrade in upgrades)
        {
            if (upgrade != null)
            {
                totalClickGain += upgrade.currentLevel * upgrade.clickGain;
            }
        }

        Clicker.Instance.ClickGain = totalClickGain;
        UpdateClickGainText();
    }

    private void UpdateUI()
    {
        UpdateClickGainText();

        if (upgrades == null) return;
        for (int i = 0; i < upgrades.Length; i++)
        {
            UpdateSingleUpgradeUI(i);
        }
    }

    private void UpdateClickGainText()
    {
        if (clickGainText != null && Clicker.Instance != null)
            clickGainText.text = $"+{Clicker.Instance.ClickGain}/клик";
    }

    private void UpdateSingleUpgradeUI(int index)
    {
        if (index < 0 || index >= upgrades.Length || upgrades[index] == null) return;
        if (Clicker.Instance == null) return;

        var upgrade = upgrades[index];
        if (upgrade.costText != null)
            upgrade.costText.text = $"Цена: {upgrade.currentCost.ToString()} руб.";

        if (upgrade.levelText != null)
            upgrade.levelText.text = $"Куплено {upgrade.currentLevel}";

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
                PlayerPrefs.DeleteKey($"Upgrade_{i}_Level");
            }
        }

        if (Clicker.Instance != null)
        {
            Clicker.Instance.ClickGain = 1; // Сбрасываем к базовому значению
            Clicker.Instance.UpdateUI();
        }
        UpdateUI();
    }
}