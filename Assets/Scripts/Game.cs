using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;

public class Game : MonoBehaviour
{
    /////////////////
    // --- GAME ---//
    /////////////////

    [Header("Money / Upgrade / AutoClicker")]
    [SerializeField] public GameObject animationMoneyTextPrefab;
    [SerializeField] private int money = 0;
    [SerializeField] private int clickGain = 1;
    [SerializeField] private int upgradeClickCost = 25;
    [SerializeField] private int upgradeClickLvl = 1;

    [SerializeField] private int autoClickValue = 0;
    [SerializeField] private int autoClickerLvl = 0;
    [SerializeField] private const int autoClickerStartCost = 75;
    [SerializeField] private int autoClickerCost = autoClickerStartCost;
    [SerializeField] private float autoClickRate = 1f;

    [Header("UI Elements")]
    [SerializeField] public Text moneyText;
    [SerializeField] public Text autoClickerText;
    [SerializeField] public Text upgradeText;
    [SerializeField] public GameObject shopPanel;
    [SerializeField] public GameObject settingsPanel;
    [SerializeField] public GameObject miniGamesPanel;

    ////////////////////////////
    // --- ПАРАМЕТРЫ ДЛЯ ДОСТИЖЕНИЙ ---
    ////////////////////////////
    // Дополнительные игровые параметры, которые могут влиять на достижения
    [Header("Дополнительные игровые параметры")]
    [SerializeField] private bool completedAllTraining = false; // «Грамотей»
    [SerializeField] private bool creditRepaid = false;           // «Дисциплина»
    [SerializeField] private int avoidedScamCount = 0;             // «Успел среагировать»
    [SerializeField] private int educationPaymentsCount = 0;       // «Прилежный студент»
    [SerializeField] private int dormPaymentsCount = 0;            // «Жизнь общажная»

    // Для достижения "Как быстро достать 1000 рублей?"
    private const int maxClickUpgradeLevel = 11;
    private const int autoClickerMaxLevel = 10;

    ////////////////////////////
    // --- ДОСТИЖЕНИЯ / UI ---//
    ////////////////////////////

    [Header("UI Достижений")]
    [SerializeField] private Transform achievementShelf;             // Панель, куда будут добавляться элементы достижений
    [SerializeField] private GameObject achievementEntryPrefab;        // Префаб одного достижения (с кнопкой, Image (имя "Icon") и Text (имя "Title"))
    [SerializeField] private GameObject achievementModal;              // Модальное окно для описания достижения
    [SerializeField] private Image achievementModalIcon;
    [SerializeField] private Text achievementModalTitle;
    [SerializeField] private Text achievementModalDescription;

    // Список достижений
    private List<Achievement> achievements = new List<Achievement>();

    // Вложенный класс для описания достижения
    [Serializable]
    public class Achievement
    {
        public string id;
        public string title;
        public string description;
        public Sprite icon;
        public bool unlocked;
        // Функция для проверки разблокировки, получает текущий объект Game
        public Func<Game, bool> condition;

        public Achievement(string id, string title, string description, Sprite icon, Func<Game, bool> condition)
        {
            this.id = id;
            this.title = title;
            this.description = description;
            this.icon = icon;
            this.condition = condition;
            this.unlocked = false;
        }
    }

    ////////////////////////////
    // --- LIFE CYCLE ---//
    ////////////////////////////

    void Start()
    {
        // При запуске рассчитаем оффлайн-доход (если требуется)
        CalculateOfflineIncome();
        UpdateMoneyText();
        UpdateUpgradeText();

        // Запустим автогенерацию денег
        StartCoroutine(AutoGenerateMoney());

        // Инициализируем систему достижений
        InitializeAchievements();
        RefreshAchievementsDisplay();
    }

    ////////////////////////////
    // --- МЕТОДЫ ИГРЫ ---//
    ////////////////////////////

    // Эффекты для визуального отображения прибавки денег (анимация + текста)
    public void EffectsToMoneyGain(int value)
    {
        GameObject animationMoneyTextObject = Instantiate(animationMoneyTextPrefab, transform.parent);
        Text animationMoneyText = animationMoneyTextObject.GetComponent<Text>();
        animationMoneyText.text = "+" + value;
        StartCoroutine(MoveAndDelete(animationMoneyTextObject));
    }

    private IEnumerator MoveAndDelete(GameObject obj, float durationTime = 1f)
    {
        RectTransform rectTransform = obj.GetComponent<RectTransform>();
        Vector2 randomDirection = UnityEngine.Random.insideUnitCircle.normalized * 490f;
        float elapsedTime = 0f;
        while (elapsedTime < durationTime)
        {
            rectTransform.anchoredPosition += randomDirection * Time.deltaTime;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Destroy(obj);
    }

    // При нажатии – прибавляем деньги и запускаем анимацию кнопки
    public void Click()
    {
        money += clickGain;
        StartCoroutine(ButtonAnimation());
        EffectsToMoneyGain(clickGain);
        UpdateMoneyText();
        UpdateAchievements();
    }

    private IEnumerator ButtonAnimation()
    {
        Vector3 originalScale = transform.localScale;
        transform.localScale = originalScale * 0.9f;
        yield return new WaitForSeconds(0.1f);
        transform.localScale = originalScale;
    }

    // Апгрейд клика
    public void UpgradeClick()
    {
        if (upgradeClickLvl <= 10)
        {
            if (money >= upgradeClickCost)
            {
                money -= upgradeClickCost;
                clickGain++;
                upgradeClickLvl++;
                UpdateMoneyText();
                upgradeClickCost += 15;
            }
            else
            {
                Debug.Log("Недостаточно денег для апгрейда клика");
            }
        }
        else
        {
            if (money >= upgradeClickCost)
            {
                money -= upgradeClickCost;
                clickGain += 3;
                upgradeClickCost *= 2;
                upgradeClickLvl++;
                UpdateMoneyText();
            }
            else
            {
                Debug.Log("Недостаточно денег для апгрейда клика");
            }
        }
        UpdateUpgradeText();
        UpdateAchievements();
    }

    // Покупка автокликера
    public void BuyAutoClicker()
    {
        if (money >= autoClickerCost)
        {
            autoClickerLvl++;
            autoClickValue += 1;
            money -= autoClickerCost;
            autoClickerCost = Mathf.RoundToInt(autoClickerStartCost * Mathf.Pow(1.65f, autoClickerLvl));
            UpdateAutoClickerText();
            UpdateMoneyText();
            UpdateAchievements();
        }
    }

    public void UpdateAutoClickerText()
    {
        // Здесь можно обновить текст для автокликера, если требуется.
        if (autoClickerText != null)
            autoClickerText.text = "AutoClick lvl: " + autoClickerLvl + " | Cost: " + autoClickerCost;
    }

    private IEnumerator AutoGenerateMoney()
    {
        while (true)
        {
            if (autoClickValue > 0)
            {
                money += autoClickValue;
                EffectsToMoneyGain(autoClickValue);
                UpdateMoneyText();
                UpdateAchievements();
            }
            yield return new WaitForSeconds(autoClickRate);
        }
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetString("LastPlayedTime", DateTime.UtcNow.ToString());
    }

    // Рассчитываем оффлайн-доход, если игрок отсутствовал некоторое время
    private void CalculateOfflineIncome()
    {
        if (!PlayerPrefs.HasKey("LastPlayedTime")) return;

        string lastPlayedStr = PlayerPrefs.GetString("LastPlayedTime");
        DateTime lastPlayedTime;
        if (DateTime.TryParse(lastPlayedStr, out lastPlayedTime))
        {
            var maxTimeOut = 6 * 60 * 60;
            var secondsOut = (DateTime.UtcNow - lastPlayedTime).TotalSeconds;
            secondsOut = (secondsOut > maxTimeOut) ? maxTimeOut : secondsOut;
            money += (int)secondsOut * autoClickValue;
            Debug.Log($"Offline time: {secondsOut} sec, added money: {(int)secondsOut * autoClickValue}");
        }
    }

    private void UpdateMoneyText()
    {
        if (moneyText != null)
            moneyText.text = money.ToString() + "$";
    }
    private void UpdateUpgradeText()
    {
        if (upgradeText != null)
            upgradeText.text = "Upgrade ($" + upgradeClickCost + ") | Level: " + upgradeClickLvl;
    }

    public void Settings()
    {
        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }
    public void UpgradeShop()
    {
        shopPanel.SetActive(!shopPanel.activeSelf);
    }
    public void MinigamesMenu()
    {
        miniGamesPanel.SetActive(!miniGamesPanel.activeSelf);
    }

    ////////////////////////////
    // --- ДОСТИЖЕНИЯ ---//
    ////////////////////////////

    // Инициализация достижений (загрузка спрайтов из Resources, задание условий на основе текущей игры)
    private void InitializeAchievements()
    {
        // Загружаем спрайты иконок достижений
        Sprite coinIcon = Resources.Load<Sprite>("Icons/coin_100");
        Sprite scrollIcon = Resources.Load<Sprite>("Icons/scroll");
        Sprite safeIcon = Resources.Load<Sprite>("Icons/safe");
        Sprite brokenContractIcon = Resources.Load<Sprite>("Icons/broken_contract");
        Sprite moneybagIcon = Resources.Load<Sprite>("Icons/moneybag");
        Sprite shieldIcon = Resources.Load<Sprite>("Icons/shield");
        Sprite graduationCapIcon = Resources.Load<Sprite>("Icons/graduation_cap");
        Sprite houseIcon = Resources.Load<Sprite>("Icons/house");
        Sprite chestIcon = Resources.Load<Sprite>("Icons/chest");

        // a. «Первые деньги» – накопление первых 100 рублей
        achievements.Add(new Achievement(
            "first_money",
            "Первые деньги",
            "Накопление первых 100 рублей",
            coinIcon,
            (game) => game.money >= 100));

        // b. «Грамотей» – пройти все блоки обучения
        achievements.Add(new Achievement(
            "gramotey",
            "Грамотей",
            "Пройти все блоки обучения",
            scrollIcon,
            (game) => game.completedAllTraining));

        // c. «Инвестор-новичок» – открыть первый вклад (здесь условие считаем привязкой к покупке автокликера)
        achievements.Add(new Achievement(
            "investor_novice",
            "Инвестор-новичок",
            "Открыть первый вклад (купить автокликер)",
            safeIcon,
            (game) => game.autoClickerLvl >= 1));

        // d. «Дисциплина» – погасить кредит
        achievements.Add(new Achievement(
            "discipline",
            "Дисциплина",
            "Погасить кредит",
            brokenContractIcon,
            (game) => game.creditRepaid));

        // e. «Как быстро достать 1000 рублей?» – достичь максимального уровня улучшения клика или автокликера
        achievements.Add(new Achievement(
            "get_1000",
            "Как быстро достать 1000 рублей?",
            "Достичь максимального уровня улучшения клика или автокликера",
            moneybagIcon,
            (game) => (game.upgradeClickLvl >= maxClickUpgradeLevel || game.autoClickerLvl >= autoClickerMaxLevel)));

        // f. «Успел среагировать» – 5 раз не попасться на мошеннические махинации
        achievements.Add(new Achievement(
            "reacted",
            "Успел среагировать",
            "5 раз не попасться на мошеннические махинации",
            shieldIcon,
            (game) => game.avoidedScamCount >= 5));

        // g. «Прилежный студент» – 5 раз оплатить обучение
        achievements.Add(new Achievement(
            "diligent_student",
            "Прилежный студент",
            "5 раз оплатить обучение",
            graduationCapIcon,
            (game) => game.educationPaymentsCount >= 5));

        // h. «Жизнь общажная» – 5 раз оплатить проживание в общежитии
        achievements.Add(new Achievement(
            "dorm_life",
            "Жизнь общажная",
            "5 раз оплатить проживание в общежитии",
            houseIcon,
            (game) => game.dormPaymentsCount >= 5));

        // i. «Богач» – накопить 100 000 рублей
        achievements.Add(new Achievement(
            "rich_man",
            "Богач",
            "Накопить 100 000 рублей",
            chestIcon,
            (game) => game.money >= 100000));
    }

    // Проверка условий разблокировки для всех достижений
    private void UpdateAchievements()
    {
        bool needRefresh = false;
        foreach (var ach in achievements)
        {
            if (!ach.unlocked && ach.condition(this))
            {
                ach.unlocked = true;
                Debug.Log("Достижение разблокировано: " + ach.title);
                // Здесь можно вызвать показ уведомления на экране
                needRefresh = true;
            }
        }
        if (needRefresh)
        {
            RefreshAchievementsDisplay();
        }
    }

    // Обновление отображения полочки достижений (очищаем панель и создаём элемент для каждого достижения)
    private void RefreshAchievementsDisplay()
    {
        if (achievementShelf == null) return;
        // Удаляем предыдущие элементы
        foreach (Transform child in achievementShelf)
        {
            Destroy(child.gameObject);
        }
        // Создаём новые элементы для каждого достижения
        foreach (var ach in achievements)
        {
            GameObject entry = Instantiate(achievementEntryPrefab, achievementShelf);
            SetupAchievementEntry(entry, ach);
        }
    }

    // Инициализация элемента достижения (заполнение и установка обработчика нажатия)
    private void SetupAchievementEntry(GameObject entry, Achievement achievement)
    {
        // Предполагается, что в префабе есть дочерний объект с именем "Icon" (Image) и "Title" (Text)
        Image iconImage = entry.transform.Find("Icon").GetComponent<Image>();
        Text titleText = entry.transform.Find("Title").GetComponent<Text>();
        if (iconImage != null)
        {
            iconImage.sprite = achievement.icon;
            // Если достижение не разблокировано – затемняем (пример с уменьшенной яркостью)
            iconImage.color = achievement.unlocked ? Color.white : new Color(0.5f, 0.5f, 0.5f, 1f);
        }
        if (titleText != null)
            titleText.text = achievement.title;

        // Если на префабе есть компонент Button, добавляем обработчик клика
        Button btn = entry.GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => {
                if (achievement.unlocked)
                    OpenAchievementModal(achievement);
            });
        }
    }

    // Открытие модального окна с описанием достижения
    public void OpenAchievementModal(Achievement achievement)
    {
        if (achievementModalIcon != null) achievementModalIcon.sprite = achievement.icon;
        if (achievementModalTitle != null) achievementModalTitle.text = achievement.title;
        if (achievementModalDescription != null) achievementModalDescription.text = achievement.description;
        if (achievementModal != null) achievementModal.SetActive(true);
    }

    // Метод закрытия модального окна (привяжите его к кнопке закрытия)
    public void CloseAchievementModal()
    {
        if (achievementModal != null) achievementModal.SetActive(false);
    }
}