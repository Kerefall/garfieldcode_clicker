//using UnityEngine;
//using TMPro;
//using UnityEngine.UI;

//public class AutoClickerUpgrade : MonoBehaviour
//{
//    [System.Serializable]
//    public class Upgrade
//    {
//        public string upgradeName;
//        public int baseCost = 10;
//        public int incomePerSecond = 1;
//        public float costMultiplier = 1.15f;

//        [Space]
//        public Button purchaseButton;
//        public TextMeshProUGUI costText;
//        public TextMeshProUGUI levelText;

//        [HideInInspector] public int currentLevel;
//        [HideInInspector] public int currentCost;
//    }

//    [Header("Настройки")]
//    [SerializeField] private float incomeInterval = 1f;
//    [SerializeField] private TextMeshProUGUI incomePerSecondText;

//    [Header("Улучшения")]
//    [SerializeField] private Upgrade[] upgrades = new Upgrade[7];

//    private float _timer;
//    private int _totalIncomePerSecond;

//    private void Start()
//    {
//        Debug.Log("[AutoClicker] Инициализация системы автокликера");
//        InitializeUpgrades();
//        LoadProgress();
//        UpdateUI();
//    }

//    private void Update()
//    {
//        _timer += Time.deltaTime;
//        if (_timer >= incomeInterval)
//        {
//            GenerateIncome();
//            _timer = 0f;
//        }
//    }

//    private void InitializeUpgrades()
//    {
//        Debug.Log($"[AutoClicker] Инициализация {upgrades.Length} улучшений");
//        for (int i = 0; i < upgrades.Length; i++)
//        {
//            if (upgrades[i] == null) continue;

//            int index = i;
//            upgrades[i].purchaseButton.onClick.AddListener(() => BuyUpgrade(index));
//            Debug.Log($"[AutoClicker] Кнопка улучшения '{upgrades[i].upgradeName}' готова");
//        }
//    }

//    private void BuyUpgrade(int upgradeIndex)
//    {
//        if (upgradeIndex < 0 || upgradeIndex >= upgrades.Length)
//        {
//            Debug.LogError($"[AutoClicker] Неверный индекс улучшения: {upgradeIndex}");
//            return;
//        }

//        var upgrade = upgrades[upgradeIndex];
//        if (Clicker.Instance.Money >= upgrade.currentCost)
//        {
//            Debug.Log($"[AutoClicker] Покупка улучшения '{upgrade.upgradeName}' за {upgrade.currentCost}$");

//            Clicker.Instance.Money -= upgrade.currentCost;
//            upgrade.currentLevel++;
//            upgrade.currentCost = CalculateCost(upgrade);

//            Debug.Log($"[AutoClicker] Улучшение '{upgrade.upgradeName}' теперь уровня {upgrade.currentLevel}");
//            Debug.Log($"[AutoClicker] Следующая покупка будет стоить {upgrade.currentCost}$");
//            Debug.Log($"[AutoClicker] Это улучшение теперь приносит {upgrade.incomePerSecond * upgrade.currentLevel}/сек");

//            SaveProgress();
//            UpdateUI();
//        }
//        else
//        {
//            Debug.Log($"[AutoClicker] Недостаточно денег для улучшения '{upgrade.upgradeName}'. Нужно {upgrade.currentCost}$, есть {Clicker.Instance.Money}$");
//        }
//    }

//    private int CalculateCost(Upgrade upgrade)
//    {
//        return Mathf.RoundToInt(upgrade.baseCost * Mathf.Pow(upgrade.costMultiplier, upgrade.currentLevel));
//    }

//    private void GenerateIncome()
//    {
//        if (_totalIncomePerSecond > 0)
//        {
//            Debug.Log($"[AutoClicker] Генерация пассивного дохода: +{_totalIncomePerSecond}$");
//            Clicker.Instance.Money += _totalIncomePerSecond;
//            Clicker.Instance.Update();
//        }
//    }

//    private void UpdateUI()
//    {
//        _totalIncomePerSecond = 0;

//        foreach (var upgrade in upgrades)
//        {
//            _totalIncomePerSecond += upgrade.currentLevel * upgrade.incomePerSecond;

//            if (upgrade.costText != null)
//                upgrade.costText.text = upgrade.currentCost.ToString();

//            if (upgrade.levelText != null)
//                upgrade.levelText.text = $"Ур. {upgrade.currentLevel}";

//            if (upgrade.purchaseButton != null)
//                upgrade.purchaseButton.interactable = Clicker.Instance.Money >= upgrade.currentCost;
//        }

//        if (incomePerSecondText != null)
//            incomePerSecondText.text = $"+{_totalIncomePerSecond}/сек";

//        Debug.Log($"[AutoClicker] Обновление UI. Общий доход: {_totalIncomePerSecond}/сек");
//    }

//    private void LoadProgress()
//    {
//        Debug.Log("[AutoClicker] Загрузка сохраненного прогресса");
//        for (int i = 0; i < upgrades.Length; i++)
//        {
//            upgrades[i].currentLevel = PlayerPrefs.GetInt($"AutoUpgrade_{i}_Level", 0);
//            upgrades[i].currentCost = CalculateCost(upgrades[i]);
//            Debug.Log($"[AutoClicker] Улучшение '{upgrades[i].upgradeName}' загружено (уровень {upgrades[i].currentLevel})");
//        }
//    }

//    private void SaveProgress()
//    {
//        Debug.Log("[AutoClicker] Сохранение прогресса");
//        for (int i = 0; i < upgrades.Length; i++)
//        {
//            PlayerPrefs.SetInt($"AutoUpgrade_{i}_Level", upgrades[i].currentLevel);
//        }
//        PlayerPrefs.Save();
//    }

//    [ContextMenu("Сбросить прогресс")]
//    public void ResetProgress()
//    {
//        Debug.Log("[AutoClicker] Сброс прогресса");
//        for (int i = 0; i < upgrades.Length; i++)
//        {
//            upgrades[i].currentLevel = 0;
//            PlayerPrefs.DeleteKey($"AutoUpgrade_{i}_Level");
//        }
//        UpdateUI();
//    }
//}