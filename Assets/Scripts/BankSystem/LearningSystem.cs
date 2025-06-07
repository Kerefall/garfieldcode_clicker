using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class FinancialLiteracyGame : MonoBehaviour
{
    [Header("Теоретическая часть")]
    public GameObject theoryPanel;
    public Button nextTheoryButton;
    public Button prevTheoryButton;
    public Button closeTheoryButton;
    public TextMeshProUGUI theoryText;
    private int currentTheoryPage = 0;
    private int theoryFontSize = 80;

    [Header("Панель меню")]
    public GameObject teachingPanel;

    private string[] theoryPages = {
        "Инфляция – устойчивое повышение общего уровня цен на товары и услуги. Простыми словами – со временем за одинаковое количество денег можно будет купить всё меньше товаров и услуг.",
        "Инфляция влечёт за собой очень много последствий, но основные, которые имеют для тебя значение:\r\n1)\tДеньги на вашем счёте или в копилке теряют «ценность», цены растут\r\n2)\tДля снижения инфляции государство повышает ключевую ставку, о которой расскажем в другом разделе",
        "Ключевая ставка – процент, под который Центральный Банк выдаёт деньги коммерческим банкам. Следственно коммерческие банки выдают кредиты людям под более высокий процент.",
        "Если не вдаваться в подробности, то ключевая ставка в большей степени складывается на основании состояния экономики\r\nЦены растут слишком быстро (инфляция) – ЦБ повышает ставку – кредиты дорожают – люди меньше тратят – инфляция замедляется",
        "И наоборот. Цены падают слишком быстро (дефляция) – ЦБ понижает ставку – кредиты дешевеют – люди активней берут деньги – дефляция замедляется",
        "Вклад является хорошим доступным и понятным средством для того, чтобы минимизировать влияние инфляции на ваши сбережения.\r\nВы отдаёте деньги банку на хранение, а банк платит вам за это проценты. Чем выше ключевая ставка, тем выше проценты на вкладах.",
        "Как это работает? Банк выдаёт ваши деньги в кредиты под более высокий процент и разницу оставляет себе.\r\nОбычно, чем меньше срок, на которой открывается вклад, тем выше процент. Это из-за того, что банку проще предсказать поведение экономики в стране на короткий срок, чем на более длинный.",
        "Иногда случается так, что деньги нужны здесь и сейчас. В этом вам поможет такой финансовый инструмент, как кредит.\r\nКредит – когда вам дают деньги в долг, но с условием вернуть больше, чем взяли. Разница между «взял» и «вернул» — это процент, под который выдаётся кредит.",
        "Из чего процент складывается?\r\n1.\tВ первую очередь процент зависит от ключевой ставки. Чем она выше, тем дороже кредиты\r\n2.\tРиски банка. Да, выдавать кредиты для банка риск потерять деньги, поэтому если у вас плохая кредитная история или маленькая зарплата, то банк поднимет процент дабы перестраховаться\r\n3.\tПрибыль банка. Банк тоже хочет заработать и поэтому «накидывает» пару процентов",
        "Что такое переплата по кредиту?\r\nПереплата = Сумма кредита * Годовой процент * Срок в годах \r\nТо есть это то, сколько вы платите за возможность получить деньги здесь и сейчас.\r\nДля того, чтобы сократить переплату нужно уменьшать срок кредита и по возможности погашать досрочно.",
        "В досрочном погашении кредита может помочь рефинансирование. Если появляется возможность открыть кредит под более низкий процент, то может быть выгодным открыть новый и погасить им старый. Но нужно учитывать, что за рефинансирование банк может взымать комиссию.",
        "1.\t«Банк звонит»\r\n— Вам говорят, что ваш счёт взламывают, и просят:\r\no\tПеревести деньги на «безопасный счёт».\r\no\tНазвать данные карты, код из SMS или пароль от Госуслуг.\r\n→ Это обман! Банк никогда не просит такие данные.",
        "2.\t«Родственник просит денег»\r\no\tВ соцсетях или мессенджерах пишет «знакомый» с просьбой срочно перевести деньги\r\n→ Позвоните ему по видео (мошенники подделывают голос!).",
        "3.\t«Оплатите учёбу/тестирование»\r\no\tПриходит ссылка «от вуза/школы» с требованием оплатить или авторизоваться через Госуслуги.\r\n→ Проверьте домен (.ru/.рф/.su — официальные, остальные — подделки).",
        "4.\tФинансовые пирамиды\r\no\tВам обещают огромные доходы за «вложения» или приглашение друзей.\r\n→ Не верьте! Такие схемы рушатся, а деньги исчезают.",
        "Что делать, если попались?\r\n1.\tНемедленно заблокируйте карту/счёт через банк.\r\n2.\tПодайте заявление в полицию.\r\n3.\tПредупредите других!\r\nГлавное правило: не спешите и перепроверяйте информацию!\r\n"
    };

    [Header("Тестовая часть")]
    public GameObject testPanel;
    public TextMeshProUGUI questionText;
    public Button[] answerButtons;
    public Button confirmButton;
    public TextMeshProUGUI resultText;
    private int currentQuestion = 0;
    private bool[] selectedAnswers;

    private Question[] questions = {
        new Question(
            "В стране подняли ключевую ставку. Что можно предположить?",
            new string[] {
                "Наблюдается рост инфляции",
                "Сейчас самое выгодное время, чтобы взять кредит",
                "Сейчас выше проценты по вкладам"
            },
            new int[] { 0, 2 }
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
    public TextMeshProUGUI resultTextCredit;
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
            1,
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
    public Button startTheoryButton;
    public Button startTestButton;
    public Button startCreditGameButton;
    public Button startSecurityGameButton;
    public Button backToMenuButton;

    void Start()
    {
        // Теория
        theoryText.fontSize = theoryFontSize;
        nextTheoryButton.onClick.AddListener(NextTheoryPage);
        prevTheoryButton.onClick.AddListener(PrevTheoryPage);
        closeTheoryButton.onClick.AddListener(CloseTheory);
        UpdateTheoryText();

        // Навигация
        startTheoryButton.onClick.AddListener(() => ShowPanel(theoryPanel));
        startTestButton.onClick.AddListener(() => {
            ShowPanel(testPanel);
            InitializeTest();
        });
        startCreditGameButton.onClick.AddListener(() => {
            ShowPanel(creditGamePanel);
            InitializeCreditGame();
        });
        startSecurityGameButton.onClick.AddListener(() => {
            ShowPanel(securityGamePanel);
            ShowScamScenario();
        });
        backToMenuButton.onClick.AddListener(() => ShowPanel(teachingPanel));

        // Тесты
        selectedAnswers = new bool[answerButtons.Length];
        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i;
            answerButtons[i].onClick.AddListener(() => SelectAnswer(index));
        }
        confirmButton.onClick.AddListener(ConfirmAnswer);

        // Игра с кредитом
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
        for (int i = 0; i < responseButtons.Length; i++)
        {
            int responseIndex = i;
            responseButtons[i].onClick.AddListener(() => HandleScamResponse(responseIndex));
        }

        // Активируем панель меню по умолчанию
        ShowPanel(teachingPanel);
    }

    void ShowPanel(GameObject panelToShow)
    {
        teachingPanel.SetActive(panelToShow == teachingPanel);
        theoryPanel.SetActive(panelToShow == theoryPanel);
        testPanel.SetActive(panelToShow == testPanel);
        creditGamePanel.SetActive(panelToShow == creditGamePanel);
        securityGamePanel.SetActive(panelToShow == securityGamePanel);
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

    void PrevTheoryPage()
    {
        currentTheoryPage--;
        if (currentTheoryPage < 0)
        {
            currentTheoryPage = theoryPages.Length - 1;
        }
        UpdateTheoryText();
    }

    void UpdateTheoryText()
    {
        theoryText.text = theoryPages[currentTheoryPage];

        // Обновляем состояние кнопок навигации
        prevTheoryButton.interactable = (currentTheoryPage > 0);
        nextTheoryButton.interactable = (currentTheoryPage < theoryPages.Length - 1);
    }

    void CloseTheory()
    {
        ShowPanel(teachingPanel);
    }
    #endregion

    #region Тестовая часть
    void InitializeTest()
    {
        currentQuestion = 0;
        resultText.text = "";
        selectedAnswers = new bool[answerButtons.Length];
        ShowQuestion();
    }

    void ShowQuestion()
    {
        if (currentQuestion >= questions.Length)
        {
            resultText.text = "Тест завершен!";
            StartCoroutine(ReturnToMenuAfterDelay(2f));
            return;
        }

        // Сбрасываем выбранные ответы
        for (int i = 0; i < selectedAnswers.Length; i++)
        {
            selectedAnswers[i] = false;
        }

        questionText.text = questions[currentQuestion].question;
        resultText.text = "";

        // Убедимся, что у нас достаточно кнопок для вариантов ответов
        int answersCount = questions[currentQuestion].answers.Length;
        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (i < answersCount)
            {
                answerButtons[i].gameObject.SetActive(true);
                answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = questions[currentQuestion].answers[i];
                answerButtons[i].image.color = Color.white;
            }
            else
            {
                answerButtons[i].gameObject.SetActive(false);
            }
        }
    }

    void SelectAnswer(int answerIndex)
    {
        // Проверяем, что индекс в пределах массива ответов текущего вопроса
        if (answerIndex >= questions[currentQuestion].answers.Length) return;

        selectedAnswers[answerIndex] = !selectedAnswers[answerIndex];
        answerButtons[answerIndex].image.color = selectedAnswers[answerIndex] ? Color.yellow : Color.white;
    }

    void ConfirmAnswer()
    {
        bool allCorrect = true;
        bool anySelected = false;

        // Проверяем, все ли правильные ответы выбраны и нет ли лишних
        for (int i = 0; i < questions[currentQuestion].answers.Length; i++)
        {
            bool shouldBeSelected = System.Array.IndexOf(questions[currentQuestion].correctAnswers, i) >= 0;

            if (selectedAnswers[i] != shouldBeSelected)
            {
                allCorrect = false;
            }

            if (selectedAnswers[i])
            {
                anySelected = true;
            }
        }

        if (!anySelected)
        {
            resultText.text = "Выберите хотя бы один ответ!";
            return;
        }

        if (allCorrect)
        {
            resultText.text = "Правильно!";
            // Подсвечиваем правильные ответы зеленым
            foreach (int correctIndex in questions[currentQuestion].correctAnswers)
            {
                answerButtons[correctIndex].image.color = Color.green;
            }
        }
        else
        {
            resultText.text = "Неправильно!";
            // Подсвечиваем правильные ответы зеленым, а выбранные неправильные - красным
            foreach (int correctIndex in questions[currentQuestion].correctAnswers)
            {
                answerButtons[correctIndex].image.color = Color.green;
            }
            for (int i = 0; i < questions[currentQuestion].answers.Length; i++)
            {
                if (selectedAnswers[i] && System.Array.IndexOf(questions[currentQuestion].correctAnswers, i) < 0)
                {
                    answerButtons[i].image.color = Color.red;
                }
            }
        }

        StartCoroutine(NextQuestionAfterDelay(2f));
    }

    IEnumerator NextQuestionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        currentQuestion++;
        ShowQuestion();
    }

    IEnumerator ReturnToMenuAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowPanel(teachingPanel);
    }
    #endregion

    #region Мини-игра с кредитом
    void InitializeCreditGame()
    {
        amountSlider.value = 50000;
        UpdateAmountText(amountSlider.value);
        resultTextCredit.text = "Выберите сумму и срок кредита";
        finalResultText.text = "";
        earlyRepayment = false;
        refinanced = false;
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
                resultTextCredit.text = "Отличный выбор!";
                break;
            case 1: // 3 года
                creditTerm = 3;
                interestRate = 0.12f;
                resultTextCredit.text = "Первый вариант более выгоден";
                break;
            case 2: // 5 лет
                creditTerm = 5;
                interestRate = 0.10f;
                resultTextCredit.text = "Первый вариант более выгоден";
                break;
        }

        CalculateOverpayment();
    }

    void CalculateOverpayment()
    {
        overpayment = creditAmount * interestRate * creditTerm;
        resultTextCredit.text += $"\nПереплата составит: {overpayment:F0} руб.";
    }

    void EarlyRepayment(bool doRepayment)
    {
        earlyRepayment = doRepayment;
        overpayment = creditAmount * interestRate * creditTerm * (doRepayment ? 0.7f : 1f);
        resultTextCredit.text = doRepayment ?
            "Хороший выбор, переплата сократилась на 30%" :
            "Переплата не сократилась";
    }

    void Refinance(bool doRefinance)
    {
        refinanced = doRefinance;
        overpayment *= doRefinance ? 0.9f : 1f;
        resultTextCredit.text = doRefinance ?
            "Переплата сократилась на 10%" :
            "Переплата не сократилась";
        ShowFinalResult();
    }

    void ExplainRefinance()
    {
        resultTextCredit.text = "Рефинансирование - замена текущего кредита на новый с лучшими условиями. " +
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
            currentScam = 0;
            ShowPanel(teachingPanel);
            return;
        }

        scamMessageText.text = scams[currentScam].message;
        securityResultText.text = "";

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
        StartCoroutine(NextScamAfterDelay(2f));
    }

    IEnumerator NextScamAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        securityResultText.text = "";
        ShowScamScenario();
    }
    #endregion
}

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