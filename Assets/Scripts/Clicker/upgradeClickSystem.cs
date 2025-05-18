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
        public int clickGain = 1;
        public Button button;
        public TextMeshProUGUI costText;
        public TextMeshProUGUI levelText;

        [HideInInspector] public int currentLevel = 0;
        [HideInInspector] public int currentCost;
    }

    [Header("Настройки")]
    [SerializeField] private Upgrade[] upgrades;
    [SerializeField] private TextMeshProUGUI clickGainText;
    [SerializeField] private float uiUpdateInterval = 0.2f;

    private float uiUpdateTimer;

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
        uiUpdateTimer += Time.deltaTime;
        if (uiUpdateTimer >= uiUpdateInterval)
        {
            UpdateButtonsInteractivity();
            uiUpdateTimer = 0f;
        }
    }

    private void InitializeUpgrades()
    {
        if (upgrades == null) return;

        for (int i = 0; i < upgrades.Length; i++)
        {
            if (upgrades[i] == null) continue;

            // Загружаем сохраненный уровень
            upgrades[i].currentLevel = PlayerPrefs.GetInt($"Upgrade_{i}_Level", 0);
            // Рассчитываем текущую стоимость
            upgrades[i].currentCost = CalculateUpgradeCost(upgrades[i]);

            // Обновляем текст цены и уровня сразу при инициализации
            UpdateSingleUpgradeUI(i);

            int index = i;
            upgrades[i].button.onClick.RemoveAllListeners();
            upgrades[i].button.onClick.AddListener(() => BuyUpgrade(index));
        }

        CalculateTotalClickGain();
    }

    // Остальные методы остаются без изменений...
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

            CalculateTotalClickGain();
            UpdateSingleUpgradeUI(upgradeIndex);
            Clicker.Instance.UpdateUI();
        }
    }

    private void CalculateTotalClickGain()
    {
        if (Clicker.Instance == null || upgrades == null) return;

        int totalClickGain = 1;
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
        UpdateButtonsInteractivity();

        // Явно обновляем все тексты при полном обновлении UI
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
                PlayerPrefs.DeleteKey($"Upgrade_{i}_Level");

                // Обновляем UI каждого апгрейда после сброса
                UpdateSingleUpgradeUI(i);
            }
        }

        if (Clicker.Instance != null)
        {
            Clicker.Instance.ClickGain = 1;
            Clicker.Instance.UpdateUI();
        }

        CalculateTotalClickGain();
    }
}