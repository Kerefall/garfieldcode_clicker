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

    private void Start()
    {
        InitializeUpgrades();
        UpdateUI();
    }

    private void InitializeUpgrades()
    {
        for (int i = 0; i < upgrades.Length; i++)
        {
            if (upgrades[i] == null) continue;

            // Загружаем сохраненный уровень (если нужно)
            upgrades[i].currentLevel = PlayerPrefs.GetInt($"Upgrade_{i}_Level", 0);
            upgrades[i].currentCost = upgrades[i].baseCost * (upgrades[i].currentLevel + 1);

            // Назначаем кнопку с правильным индексом
            int index = i;
            upgrades[i].button.onClick.RemoveAllListeners();
            upgrades[i].button.onClick.AddListener(() => BuyUpgrade(index));
        }
    }

    private void BuyUpgrade(int upgradeIndex)
    {
        if (upgradeIndex < 0 || upgradeIndex >= upgrades.Length) return;

        Upgrade upgrade = upgrades[upgradeIndex];
        if (Clicker.Instance.Money >= upgrade.currentCost)
        {
            // Применяем улучшение
            Clicker.Instance.Money -= upgrade.currentCost;
            Clicker.Instance.ClickGain += upgrade.clickGain;
            upgrade.currentLevel++;

            // Пересчитываем стоимость
            upgrade.currentCost = upgrade.baseCost * (upgrade.currentLevel + 1);

            // Сохраняем прогресс
            PlayerPrefs.SetInt($"Upgrade_{upgradeIndex}_Level", upgrade.currentLevel);

            // Обновляем интерфейс
            UpdateSingleUpgradeUI(upgradeIndex);
            Clicker.Instance.Update();

            Debug.Log($"Куплено {upgrade.upgradeName}. Уровень: {upgrade.currentLevel}");
        }
    }

    private void UpdateUI()
    {
        if (clickGainText != null)
            clickGainText.text = $"+{Clicker.Instance.ClickGain}/клик";

        for (int i = 0; i < upgrades.Length; i++)
        {
            UpdateSingleUpgradeUI(i);
        }
    }

    private void UpdateSingleUpgradeUI(int index)
    {
        if (index < 0 || index >= upgrades.Length) return;

        Upgrade upgrade = upgrades[index];
        if (upgrade == null) return;

        // Обновляем стоимость
        if (upgrade.costText != null)
            upgrade.costText.text = upgrade.currentCost.ToString();

        // Обновляем уровень
        if (upgrade.levelText != null)
            upgrade.levelText.text = $"Ур. {upgrade.currentLevel}";

        // Обновляем доступность кнопки
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
                PlayerPrefs.DeleteKey($"Upgrade_{i}_Level");
            }
        }
        Clicker.Instance.ClickGain = 1;
        UpdateUI();
    }
}