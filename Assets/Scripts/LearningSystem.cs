using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class FinancialLiteracyGame : MonoBehaviour
{
    [Header("Теоретическая часть")]
    public GameObject theoryPanel;
    public Button nextTheoryButton;
    public TextMeshProUGUI theoryText;
    private int currentTheoryPage = 0;

    private string[] theoryPages = {
        "Инфляция – устойчивое повышение общего уровня цен на товары и услуги. Простыми словами – со временем за одинаковое количество денег можно будет купить всё меньше товаров и услуг.",
        "Ключевая ставка – процент, под который Центральный Банк выдаёт деньги коммерческим банкам. Если инфляция растёт, ЦБ повышает ставку, что делает кредиты дороже и замедляет инфляцию.",
        "Вклад - средство для минимизации влияния инфляции. Банк платит вам проценты за хранение денег. Чем выше ключевая ставка, тем выше проценты по вкладам.",
        "Кредит - когда вам дают деньги в долг с условием вернуть больше. Переплата = Сумма * Год. процент * Срок. Досрочное погашение сокращает переплату.",
        "Финансовая безопасность: 1) Никогда не сообщайте данные карты по телефону. 2) Проверяйте запросы родственников. 3) Проверяйте домены сайтов (.ru, .рф, .su)."
    };

    [Header("Тестовая часть")]
    public GameObject testPanel;
    public TextMeshProUGUI questionText;
    public Button[] answerButtons;
    private int currentQuestion = 0;

    private Question[] questions = {
        new Question(
            "В стране подняли ключевую ставку. Что можно предположить?",
            new string[] {
                "Наблюдается рост инфляции",
                "Сейчас самое выгодное время, чтобы взять кредит",
                "Сейчас выше проценты по вкладам"
            },
            new int[] { 0, 2 } // Правильные ответы (индексы)
        ),
        new Question(
            "В стране понизили ключевую ставку. Что можно предположить?",
            new string[] {
                "Наблюдается рост инфляции",
                "Стало более выгодно брать кредит",
                "Рост инфляции сократился"
            },
            new int[] { 1, 2 }
        )
    };

    [Header("Мини-игра с кредитом")]
    public GameObject creditGamePanel;
    public Slider amountSlider;
    public TextMeshProUGUI amountText;
    public Button[] termButtons;
    public TextMeshProUGUI resultText;
    public Button earlyRepaymentButton;
    public Button noRepaymentButton;
    public Button refinanceButton;
    public Button noRefinanceButton;
    public Button learnRefinanceButton;
    public TextMeshProUGUI finalResultText;

    private float creditAmount;
    private int creditTerm;
    private float interestRate;
    private float overpayment;
    private bool earlyRepayment = false;
    private bool refinanced = false;

    [Header("Финансовая безопасность")]
    public GameObject securityGamePanel;
    public TextMeshProUGUI scamMessageText;
    public Button[] responseButtons;
    public TextMeshProUGUI securityResultText;

    private int currentScam = 0;
    private ScamScenario[] scams = {
        new ScamScenario(
            "Ваш кошелёк пытаются взломать, срочно введите данные с вашей карты и мы убережём ваши средства",
            new string[] { "Ввод данных", "Игнорировать" },
            0,
            "Сотрудники банка никогда не просят данные карты! Если это случилось, немедленно заблокируйте счёт."
        ),
        new ScamScenario(
            "Привет, сынок, скинь, пожалуйста, 10 тысяч по этому номеру, срочно нужно, мы тебе потом обязательно вернём. Люблю тебя!",
            new string[] { "Перевести деньги", "Позвонить маме и уточнить" },
            1,
            "Всегда проверяйте подобные запросы! Если вас обманули, обратитесь в полицию."
        ),
        new ScamScenario(
            "Здравствуйте! Вам пишет администрация школы. До 15.11 нужно пройти обязательное государственное тестирование по ссылке http://edu1est.net/ Для прохождения авторизируйтесь через госуслуги",
            new string[] { "Перейти по ссылке", "Уточнить у классного руководителя" },
            1,
            "Государственные сайты имеют домены .ru, .рф, .su. Всегда проверяйте ссылки!"
        )
    };

    [Header("Навигация")]
    public Button startTestButton;
    public Button startCreditGameButton;
    public Button startSecurityGameButton;
    public Button backToMenuButton;

    void Start()
    {
        // Теория
        nextTheoryButton.onClick.AddListener(NextTheoryPage);
        UpdateTheoryText();

        // Тесты
        startTestButton.onClick.AddListener(() => {
            theoryPanel.SetActive(false);
            testPanel.SetActive(true);
            ShowQuestion();
        });

        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i; // Для замыкания
            answerButtons[i].onClick.AddListener(() => CheckAnswer(index));
        }

        // Игра с кредитом
        startCreditGameButton.onClick.AddListener(() => {
            theoryPanel.SetActive(false);
            creditGamePanel.SetActive(true);
            InitializeCreditGame();
        });

        amountSlider.onValueChanged.AddListener(UpdateAmountText);

        for (int i = 0; i < termButtons.Length; i++)
        {
            int termIndex = i;
            termButtons[i].onClick.AddListener(() => SelectTerm(termIndex));
        }

        earlyRepaymentButton.onClick.AddListener(() => EarlyRepayment(true));
        noRepaymentButton.onClick.AddListener(() => EarlyRepayment(false));
        refinanceButton.onClick.AddListener(() => Refinance(true));
        noRefinanceButton.onClick.AddListener(() => Refinance(false));
        learnRefinanceButton.onClick.AddListener(ExplainRefinance);

        // Финансовая безопасность
        startSecurityGameButton.onClick.AddListener(() => {
            theoryPanel.SetActive(false);
            securityGamePanel.SetActive(true);
            ShowScamScenario();
        });

        for (int i = 0; i < responseButtons.Length; i++)
        {
            int responseIndex = i;
            responseButtons[i].onClick.AddListener(() => HandleScamResponse(responseIndex));
        }

        // Навигация
        backToMenuButton.onClick.AddListener(ReturnToMenu);
    }

    #region Теоретическая часть
    void NextTheoryPage()
    {
        currentTheoryPage++;
        if (currentTheoryPage >= theoryPages.Length)
        {
            currentTheoryPage = 0;
        }
        UpdateTheoryText();
    }

    void UpdateTheoryText()
    {
        theoryText.text = theoryPages[currentTheoryPage];
    }
    #endregion

    #region Тестовая часть
    void ShowQuestion()
    {
        if (currentQuestion >= questions.Length)
        {
            testPanel.SetActive(false);
            theoryPanel.SetActive(true);
            currentQuestion = 0;
            return;
        }

        questionText.text = questions[currentQuestion].question;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (i < questions[currentQuestion].answers.Length)
            {
                answerButtons[i].gameObject.SetActive(true);
                answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = questions[currentQuestion].answers[i];
            }
            else
            {
                answerButtons[i].gameObject.SetActive(false);
            }
        }
    }

    void CheckAnswer(int answerIndex)
    {
        bool isCorrect = false;
        foreach (int correctIndex in questions[currentQuestion].correctAnswers)
        {
            if (answerIndex == correctIndex)
            {
                isCorrect = true;
                break;
            }
        }

        if (isCorrect)
        {
            Debug.Log("Правильно!");
        }
        else
        {
            Debug.Log("Неправильно!");
        }

        currentQuestion++;
        ShowQuestion();
    }
    #endregion

    #region Мини-игра с кредитом
    void InitializeCreditGame()
    {
        amountSlider.value = 50000;
        UpdateAmountText(amountSlider.value);
        resultText.text = "Выберите сумму и срок кредита";
        finalResultText.text = "";
    }

    void UpdateAmountText(float value)
    {
        creditAmount = value;
        amountText.text = $"Сумма кредита: {creditAmount:F0} руб.";
    }

    void SelectTerm(int termIndex)
    {
        switch (termIndex)
        {
            case 0: // 1 год
                creditTerm = 1;
                interestRate = 0.15f;
                resultText.text = "Отличный выбор!";
                break;
            case 1: // 3 года
                creditTerm = 3;
                interestRate = 0.12f;
                resultText.text = "Первый вариант более выгоден";
                break;
            case 2: // 5 лет
                creditTerm = 5;
                interestRate = 0.10f;
                resultText.text = "Первый вариант более выгоден";
                break;
        }

        CalculateOverpayment();
    }

    void CalculateOverpayment()
    {
        overpayment = creditAmount * interestRate * creditTerm;
        resultText.text += $"\nПереплата составит: {overpayment:F0} руб.";
    }

    void EarlyRepayment(bool doRepayment)
    {
        earlyRepayment = doRepayment;

        if (doRepayment)
        {
            overpayment *= 0.7f; // Сокращаем переплату на 30%
            resultText.text = "Хороший выбор, переплата сократилась на 30%";
        }
        else
        {
            resultText.text = "Переплата не сократилась";
        }
    }

    void Refinance(bool doRefinance)
    {
        refinanced = doRefinance;

        if (doRefinance)
        {
            overpayment *= 0.9f; // Сокращаем переплату еще на 10%
            resultText.text = "Переплата сократилась на 10%";
        }
        else
        {
            resultText.text = "Переплата не сократилась";
        }

        ShowFinalResult();
    }

    void ExplainRefinance()
    {
        resultText.text = "Рефинансирование - замена текущего кредита на новый с лучшими условиями. " +
                          "Может сократить переплату, но иногда включает комиссию.";
    }

    void ShowFinalResult()
    {
        finalResultText.text = $"Итоговая переплата: {overpayment:F0} руб.\n" +
                              $"{(earlyRepayment ? "✓ Досрочное погашение" : "✗ Без досрочного погашения")}\n" +
                              $"{(refinanced ? "✓ Рефинансирование" : "✗ Без рефинансирования")}";
    }
    #endregion

    #region Финансовая безопасность
    void ShowScamScenario()
    {
        if (currentScam >= scams.Length)
        {
            securityGamePanel.SetActive(false);
            theoryPanel.SetActive(true);
            currentScam = 0;
            return;
        }

        scamMessageText.text = scams[currentScam].message;

        for (int i = 0; i < responseButtons.Length; i++)
        {
            if (i < scams[currentScam].responses.Length)
            {
                responseButtons[i].gameObject.SetActive(true);
                responseButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = scams[currentScam].responses[i];
            }
            else
            {
                responseButtons[i].gameObject.SetActive(false);
            }
        }
    }

    void HandleScamResponse(int responseIndex)
    {
        if (responseIndex == scams[currentScam].correctResponse)
        {
            securityResultText.text = "Правильно! " + scams[currentScam].explanation;
        }
        else
        {
            securityResultText.text = "Опасность! " + scams[currentScam].explanation;
        }

        currentScam++;
        StartCoroutine(NextScamAfterDelay());
    }

    IEnumerator NextScamAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        securityResultText.text = "";
        ShowScamScenario();
    }
    #endregion

    #region Навигация
    void ReturnToMenu()
    {
        testPanel.SetActive(false);
        creditGamePanel.SetActive(false);
        securityGamePanel.SetActive(false);
        theoryPanel.SetActive(true);
    }
    #endregion
}

// Вспомогательные классы
[System.Serializable]
public class Question
{
    public string question;
    public string[] answers;
    public int[] correctAnswers;

    public Question(string q, string[] a, int[] ca)
    {
        question = q;
        answers = a;
        correctAnswers = ca;
    }
}

[System.Serializable]
public class ScamScenario
{
    public string message;
    public string[] responses;
    public int correctResponse;
    public string explanation;

    public ScamScenario(string msg, string[] res, int correct, string expl)
    {
        message = msg;
        responses = res;
        correctResponse = correct;
        explanation = expl;
    }
}