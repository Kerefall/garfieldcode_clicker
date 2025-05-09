using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class EnhancedStockMarketGame : MonoBehaviour
{
    [Header("Основные элементы UI")]
    public GameObject gamePanel;
    public GameObject levelSelectionPanel;
    public TextMeshProUGUI balanceText;
    public TextMeshProUGUI sharesText;
    public TextMeshProUGUI currentPriceText;
    public TextMeshProUGUI profitText;
    public TextMeshProUGUI dayText;
    public TextMeshProUGUI companyInfoText;
    public TextMeshProUGUI newsText;
    public TMP_InputField sharesInputField;
    public Button buyButton;
    public Button sellButton;
    public Button newDayButton;
    public Button level1Button;
    public Button level2Button;
    public Button level3Button;

    [Header("График акций")]
    public RectTransform graphContainer;
    public GameObject linePrefab;
    public GameObject pointPrefab;
    private List<GameObject> graphElements = new List<GameObject>();

    [Header("Настройки игры")]
    public float[] initialBalances = { 5000f, 10000f, 20000f };
    public int maxDays = 30;
    public float newsEffectDuration = 3f;

    [System.Serializable]
    public class Company
    {
        public string name;
        public string description;
        public float initialPrice;
        public float baseVolatility;
        public Color companyColor;
        public string[] positiveNews;
        public string[] negativeNews;
    }

    public Company[] companies;

    private float balance;
    private int sharesOwned = 0;
    private float currentPrice;
    private float buyPrice;
    private List<float> priceHistory = new List<float>();
    private int currentDay = 0;
    private bool gameActive = false;
    private int currentCompanyIndex = 0;
    private int currentLevel = 0;
    private Tweener priceTweener;
    private Coroutine newsCoroutine;

    void Start()
    {
        buyButton.onClick.AddListener(BuyShares);
        sellButton.onClick.AddListener(SellShares);
        newDayButton.onClick.AddListener(NewDay);
        level1Button.onClick.AddListener(() => StartLevel(0));
        level2Button.onClick.AddListener(() => StartLevel(1));
        level3Button.onClick.AddListener(() => StartLevel(2));

        sellButton.interactable = false;
        newDayButton.interactable = false;
        sharesInputField.onValueChanged.AddListener(ValidateShareInput);

        ShowLevelSelection();
    }

    void ShowLevelSelection()
    {
        levelSelectionPanel.SetActive(true);
        gamePanel.SetActive(false);
    }

    void StartLevel(int level)
    {
        currentLevel = level;
        levelSelectionPanel.SetActive(false);
        gamePanel.SetActive(true);
        StartGame();
    }

    void StartGame()
    {
        balance = initialBalances[currentLevel];
        currentCompanyIndex = 0;
        LoadCompany(currentCompanyIndex);
        gameActive = true;

        UpdateUI();
        ClearGraph();
        AddPointToGraph(0, currentPrice);
    }

    void LoadCompany(int companyIndex)
    {
        currentCompanyIndex = companyIndex;
        Company company = companies[companyIndex];

        currentPrice = company.initialPrice;
        priceHistory.Clear();
        priceHistory.Add(currentPrice);
        currentDay = 0;

        companyInfoText.text = $"<b>{company.name}</b>\n{company.description}";
        companyInfoText.color = company.companyColor;

        UpdateUI();
    }

    void ValidateShareInput(string input)
    {
        if (string.IsNullOrEmpty(input)) return;

        if (!int.TryParse(input, out int shares))
        {
            sharesInputField.text = "";
            return;
        }

        int maxAffordable = Mathf.FloorToInt(balance / currentPrice);
        if (shares > maxAffordable)
        {
            sharesInputField.text = maxAffordable.ToString();
        }
    }

    void UpdateUI()
    {
        balanceText.text = $"Баланс: {balance:C}";
        sharesText.text = $"Акций: {sharesOwned}";
        currentPriceText.text = $"Цена: {currentPrice:C}/акция";
        dayText.text = $"День: {currentDay}/{maxDays}";

        if (sharesOwned > 0)
        {
            float currentValue = sharesOwned * currentPrice;
            float profit = currentValue - (sharesOwned * buyPrice);
            profitText.text = $"Прибыль: {profit:C} ({profit / (sharesOwned * buyPrice) * 100:F1}%)";
            profitText.color = profit >= 0 ? Color.green : Color.red;
        }
        else
        {
            profitText.text = "Прибыль: $0.00";
            profitText.color = Color.white;
        }

        // Обновляем максимальное значение в поле ввода
        int maxShares = Mathf.FloorToInt(balance / currentPrice);
        sharesInputField.placeholder.GetComponent<TextMeshProUGUI>().text = $"Макс: {maxShares}";
    }

    void BuyShares()
    {
        int sharesToBuy;
        if (string.IsNullOrEmpty(sharesInputField.text))
        {
            sharesToBuy = Mathf.FloorToInt(balance / currentPrice);
        }
        else
        {
            sharesToBuy = Mathf.Clamp(int.Parse(sharesInputField.text), 1, Mathf.FloorToInt(balance / currentPrice));
        }

        if (sharesToBuy > 0)
        {
            sharesOwned += sharesToBuy;
            balance -= sharesToBuy * currentPrice;
            buyPrice = currentPrice;
            sellButton.interactable = true;
            buyButton.interactable = false;
            newDayButton.interactable = true;

            Debug.Log($"Куплено {sharesToBuy} акций {companies[currentCompanyIndex].name} по {currentPrice:C}");
        }

        sharesInputField.text = "";
        UpdateUI();
    }

    void SellShares()
    {
        if (sharesOwned > 0)
        {
            balance += sharesOwned * currentPrice;
            Debug.Log($"Продано {sharesOwned} акций {companies[currentCompanyIndex].name} по {currentPrice:C}");
            sharesOwned = 0;
            sellButton.interactable = false;
            buyButton.interactable = true;

            if (currentDay >= maxDays)
            {
                gameActive = false;
                newDayButton.interactable = false;
                Debug.Log("Игра завершена!");
            }
        }

        UpdateUI();
    }

    void NewDay()
    {
        if (!gameActive || currentDay >= maxDays) return;

        currentDay++;
        float newPrice = CalculateNewPrice(currentPrice);

        // Анимация изменения цены
        if (priceTweener != null && priceTweener.IsActive())
            priceTweener.Kill();

        priceTweener = DOTween.To(() => currentPrice, x => currentPrice = x, newPrice, 0.5f)
            .OnUpdate(UpdateUI)
            .OnComplete(() => {
                priceHistory.Add(currentPrice);
                AddPointToGraph(currentDay, currentPrice);
                CheckForNewsEvent();
            });

        if (currentDay >= maxDays)
        {
            newDayButton.interactable = false;
            Debug.Log("Достигнут максимальный срок удержания акций!");
        }

        UpdateUI();
    }

    float CalculateNewPrice(float lastPrice)
    {
        Company company = companies[currentCompanyIndex];

        // Базовое изменение цены
        float changePercent = 2f * company.baseVolatility * (Random.value - 0.5f);
        float newPrice = lastPrice * (1f + changePercent);

        // Гарантируем, что цена не уйдет в ноль
        newPrice = Mathf.Max(newPrice, lastPrice * 0.1f);

        return newPrice;
    }

    void CheckForNewsEvent()
    {
        // 30% вероятность новости
        if (Random.value > 0.3f) return;

        Company company = companies[currentCompanyIndex];
        string newsMessage = "";
        float effectMultiplier = 1f;

        if (Random.value > 0.5f && company.positiveNews.Length > 0)
        {
            // Положительная новость
            newsMessage = company.positiveNews[Random.Range(0, company.positiveNews.Length)];
            effectMultiplier = 1.5f + Random.value; // 1.5x - 2.5x
        }
        else if (company.negativeNews.Length > 0)
        {
            // Отрицательная новость
            newsMessage = company.negativeNews[Random.Range(0, company.negativeNews.Length)];
            effectMultiplier = 0.2f + Random.value * 0.3f; // 0.2x - 0.5x
        }

        if (!string.IsNullOrEmpty(newsMessage))
        {
            if (newsCoroutine != null)
                StopCoroutine(newsCoroutine);

            newsCoroutine = StartCoroutine(ShowNews(newsMessage, effectMultiplier));
        }
    }

    IEnumerator ShowNews(string message, float effectMultiplier)
    {
        newsText.text = message;
        newsText.color = effectMultiplier > 1f ? Color.green : Color.red;
        newsText.gameObject.SetActive(true);

        // Применяем эффект новости
        float originalPrice = currentPrice;
        float targetPrice = currentPrice * effectMultiplier;

        priceTweener = DOTween.To(() => currentPrice, x => currentPrice = x, targetPrice, 0.5f)
            .OnUpdate(UpdateUI);

        yield return new WaitForSeconds(newsEffectDuration);

        // Возвращаем цену к "нормальному" уровню с некоторой коррекцией
        float newBasePrice = targetPrice * (0.8f + Random.value * 0.4f); // 0.8x - 1.2x от новой цены
        priceTweener = DOTween.To(() => currentPrice, x => currentPrice = x, newBasePrice, 0.5f)
            .OnUpdate(UpdateUI)
            .OnComplete(() => {
                newsText.gameObject.SetActive(false);
                priceHistory[priceHistory.Count - 1] = currentPrice; // Обновляем последнюю цену
            });
    }

    void AddPointToGraph(int day, float price)
    {
        // Нормализуем значения для позиционирования на графике
        float xPosition = day * (graphContainer.rect.width / maxDays);

        // Находим минимальную и максимальную цену для масштабирования
        float minPrice = Mathf.Min(priceHistory.ToArray());
        float maxPrice = Mathf.Max(priceHistory.ToArray());
        float priceRange = maxPrice - minPrice;
        if (priceRange == 0) priceRange = 1f;

        float yPosition = ((price - minPrice) / priceRange) * graphContainer.rect.height;

        // Создаем точку
        GameObject point = Instantiate(pointPrefab, graphContainer);
        RectTransform pointRect = point.GetComponent<RectTransform>();
        pointRect.anchoredPosition = new Vector2(xPosition, yPosition);
        point.GetComponent<Image>().color = companies[currentCompanyIndex].companyColor;
        graphElements.Add(point);

        // Соединяем точки линиями
        if (day > 0)
        {
            float prevX = (day - 1) * (graphContainer.rect.width / maxDays);
            float prevY = ((priceHistory[day - 1] - minPrice) / priceRange) * graphContainer.rect.height;

            GameObject line = Instantiate(linePrefab, graphContainer);
            RectTransform lineRect = line.GetComponent<RectTransform>();
            Vector2 dir = (new Vector2(xPosition, yPosition) - new Vector2(prevX, prevY);
            float distance = dir.magnitude;

            lineRect.sizeDelta = new Vector2(distance, 2f);
            lineRect.anchoredPosition = new Vector2(prevX, prevY) + dir / 2f;
            lineRect.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
            line.GetComponent<Image>().color = companies[currentCompanyIndex].companyColor;

            graphElements.Add(line);
        }
    }

    void ClearGraph()
    {
        foreach (GameObject element in graphElements)
        {
            Destroy(element);
        }
        graphElements.Clear();
    }

    public void SwitchCompany(int direction)
    {
        if (sharesOwned > 0)
        {
            Debug.Log("Сначала продайте акции текущей компании!");
            return;
        }

        currentCompanyIndex = (currentCompanyIndex + direction + companies.Length) % companies.Length;
        LoadCompany(currentCompanyIndex);
    }

    public void ResetGame()
    {
        if (priceTweener != null && priceTweener.IsActive())
            priceTweener.Kill();

        if (newsCoroutine != null)
            StopCoroutine(newsCoroutine);

        ClearGraph();
        StartGame();
    }
}