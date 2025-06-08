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
        "Из чего процент складывается?\r\n1.\tВ первую очередь процент зависит от ключевой ставки. Чем она выше, тем дороже кредиты\r\n",
        "2. Риски банка. Да, выдавать кредиты для банка риск потерять деньги, поэтому если у вас плохая кредитная история или маленькая зарплата, то банк поднимет процент дабы перестраховаться\r\n",
        "3.\tПрибыль банка. Банк тоже хочет заработать и поэтому «накидывает» пару процентов",
        "Что такое переплата по кредиту?\r\nПереплата = Сумма кредита * Годовой процент * Срок в годах \r\nТо есть это то, сколько вы платите за возможность получить деньги здесь и сейчас.\r\nДля того, чтобы сократить переплату нужно уменьшать срок кредита и по возможности погашать досрочно.",
        "В досрочном погашении кредита может помочь рефинансирование. Если появляется возможность открыть кредит под более низкий процент, то может быть выгодным открыть новый и погасить им старый. Но нужно учитывать, что за рефинансирование банк может взымать комиссию.",
        "1.\t«Банк звонит»\r\n— Вам говорят, что ваш счёт взламывают, и просят:\r\no\tПеревести деньги на «безопасный счёт».\r\no\tНазвать данные карты, код из SMS или пароль от Госуслуг.\r\n→ Это обман! Банк никогда не просит такие данные.",
        "2.\t«Родственник просит денег»\r\no\tВ соцсетях или мессенджерах пишет «знакомый» с просьбой срочно перевести деньги\r\n→ Позвоните ему по видео (мошенники подделывают голос!).",
        "3.\t«Оплатите учёбу/тестирование»\r\no\tПриходит ссылка «от вуза/школы» с требованием оплатить или авторизоваться через Госуслуги.\r\n→ Проверьте домен (.ru/.рф/.su — официальные, остальные — подделки).",
        "4.\tФинансовые пирамиды\r\no\tВам обещают огромные доходы за «вложения» или приглашение друзей.\r\n→ Не верьте! Такие схемы рушатся, а деньги исчезают.",
        "5.\t«Оплата услуг»\r\no\tПриходит ссылка для оплаты коммунальных или других услуг с неофициального домена\r\n→ Все государственные сайты имеют домены .ru, .рф, .su",
        "Что делать, если попались?\r\n1.\tНемедленно заблокируйте карту/счёт через банк.\r\n2.\tПодайте заявление в полицию.\r\n3.\tПредупредите других!\r\nГлавное правило: не спешите и перепроверяйте информацию!\r\n"
    };

    [Header("Тестовая часть")]
    public GameObject testPanel;
    public TextMeshProUGUI questionText;
    public Button[] answerButtons;
    public TextMeshProUGUI[] answerTexts;
    public Button confirmButton;
    public Button backFromTestButton;
    public TextMeshProUGUI resultText;
    private int currentQuestion = 0;
    private bool[] selectedAnswers;
    private int score;

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

    private Question[] questions = {
        new Question(
            "В стране подняли ключевую ставку. Что можно предположить?",
            new string[] { "Наблюдается рост инфляции", "Сейчас самое выгодное время, чтобы взять кредит", "Сейчас выше проценты по вкладам" },
            new int[] { 0, 2 }
        ),
        new Question(
            "В стране понизили ключевую ставку. Что можно предположить?",
            new string[] { "Наблюдается рост инфляции", "Стало более выгодно брать кредит", "Наблюдается снижение инфляции" },
            new int[] { 1, 2 }
        ),
        new Question(
            "Что из перечисленного поможет уменьшить итоговую переплату по кредиту?",
            new string[] { "Увеличение срока кредита", "Досрочное погашение", "Рефинансирование под более низкий процент" },
            new int[] { 1, 2 }
        ),
        new Question(
            "Вам звонит «сотрудник банка» и говорит, что ваш счёт пытаются взломать. Какое его действие должно вас насторожить?",
            new string[] { "Просьба назвать код из SMS", "Предложение заблокировать карту", "Просьба перевести деньги на 'безопасный счёт'" },
            new int[] { 0, 2 }
        ),
        new Question(
            "Что такое инфляция?",
            new string[] { "Снижение общего уровня цен", "Процесс, при котором деньги со временем теряют свою покупательную способность", "Увеличение количества денег в обращении" },
            new int[] { 1 }
        )
    };

    [Header("Мини-игра с кредитом: Общие элементы")]
    public GameObject creditGamePanel;
    public Slider amountSlider;
    public TextMeshProUGUI amountText;
    public TextMeshProUGUI resultTextCredit;
    public TextMeshProUGUI finalResultText;

    [Header("Мини-игра с кредитом: Группы этапов")]
    public GameObject termSelectionGroup;
    public GameObject earlyRepaymentGroup;
    public GameObject refinanceGroup;

    [Header("Мини-игра с кредитом: Кнопки")]
    public Button[] termButtons;
    public Button[] earlyRepaymentButtons;
    public Button[] refinanceButtons;

    private float creditAmount;
    private int creditTerm;
    private float interestRate;
    private float initialOverpayment;
    private float finalOverpayment;
    private bool hasChosenTerm = false;

    [Header("Финансовая безопасность")]
    public GameObject securityGamePanel;
    public TextMeshProUGUI scamMessageText;
    public Button[] responseButtons_Security;
    public TextMeshProUGUI securityResultText;

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
        ),
        new ScamScenario(
            "Не забудьте оплатить обучение! Ссылка для оплаты: http://university.pay.net",
            new string[] { "Перейти и оплатить", "Игнорировать" },
            1,
            "Официальные платежи принимаются только на сайтах с доменами .ru, .рф, .su. Эта ссылка - мошенническая!"
        ),
        new ScamScenario(
            "Для оплаты коммунальных услуг перейдите по ссылке и введите данные карты. Ссылка: http://hsc.net",
            new string[] { "Оплатить", "Игнорировать" },
            1,
            "Никогда не вводите данные карты на подозрительных сайтах! Официальные платежи принимаются только на сайтах с доменами .ru, .рф, .su"
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
        // Установка размера шрифта для всех кнопок
        SetAllButtonFontSizes(55f);

        // === Настройка обработчиков кнопок ===
        if (nextTheoryButton != null) nextTheoryButton.onClick.AddListener(NextTheoryPage);
        if (prevTheoryButton != null) prevTheoryButton.onClick.AddListener(PrevTheoryPage);
        if (closeTheoryButton != null) closeTheoryButton.onClick.AddListener(CloseTheory);

        if (startTheoryButton != null) startTheoryButton.onClick.AddListener(() => ShowPanel(theoryPanel));
        if (startTestButton != null) startTestButton.onClick.AddListener(() => { ShowPanel(testPanel); InitializeTest(); });
        if (startCreditGameButton != null) startCreditGameButton.onClick.AddListener(() => { ShowPanel(creditGamePanel); InitializeCreditGame(); });
        if (startSecurityGameButton != null) startSecurityGameButton.onClick.AddListener(() => { ShowPanel(securityGamePanel); InitializeSecurityGame(); });
        if (backToMenuButton != null) backToMenuButton.onClick.AddListener(() => ShowPanel(teachingPanel));
        if (backFromTestButton != null) backFromTestButton.onClick.AddListener(() => ShowPanel(teachingPanel));

        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i;
            if (answerButtons[i] != null) answerButtons[i].onClick.AddListener(() => SelectAnswer(index));
        }
        if (confirmButton != null) confirmButton.onClick.AddListener(ConfirmAnswer);

        if (amountSlider != null) amountSlider.onValueChanged.AddListener(UpdateAmountText);
        for (int i = 0; i < termButtons.Length; i++)
        {
            int index = i;
            if (termButtons[i] != null) termButtons[i].onClick.AddListener(() => SelectTerm(index));
        }
        for (int i = 0; i < earlyRepaymentButtons.Length; i++)
        {
            int index = i;
            if (earlyRepaymentButtons[i] != null) earlyRepaymentButtons[i].onClick.AddListener(() => HandleEarlyRepayment(index));
        }
        for (int i = 0; i < refinanceButtons.Length; i++)
        {
            int index = i;
            if (refinanceButtons[i] != null) refinanceButtons[i].onClick.AddListener(() => HandleRefinance(index));
        }

        for (int i = 0; i < responseButtons_Security.Length; i++)
        {
            int responseIndex = i;
            if (responseButtons_Security[i] != null) responseButtons_Security[i].onClick.AddListener(() => HandleScamResponse(responseIndex));
        }

        ShowPanel(teachingPanel);
    }

    void ShowPanel(GameObject panelToShow)
    {
        if (teachingPanel != null) teachingPanel.SetActive(panelToShow == teachingPanel);
        if (theoryPanel != null) theoryPanel.SetActive(panelToShow == theoryPanel);
        if (testPanel != null) testPanel.SetActive(panelToShow == testPanel);
        if (creditGamePanel != null) creditGamePanel.SetActive(panelToShow == creditGamePanel);
        if (securityGamePanel != null) securityGamePanel.SetActive(panelToShow == securityGamePanel);
    }

    #region Теоретическая часть
    void NextTheoryPage()
    {
        if (currentTheoryPage < theoryPages.Length - 1)
        {
            currentTheoryPage++;
            UpdateTheoryText();
        }
    }

    void PrevTheoryPage()
    {
        if (currentTheoryPage > 0)
        {
            currentTheoryPage--;
            UpdateTheoryText();
        }
    }

    void UpdateTheoryText()
    {
        if (theoryText != null) theoryText.text = theoryPages[currentTheoryPage];
        if (prevTheoryButton != null) prevTheoryButton.interactable = (currentTheoryPage > 0);
        if (nextTheoryButton != null) nextTheoryButton.interactable = (currentTheoryPage < theoryPages.Length - 1);
    }

    void CloseTheory() { ShowPanel(teachingPanel); }
    #endregion

    #region Тестовая часть
    void InitializeTest()
    {
        score = 0;
        currentQuestion = 0;
        selectedAnswers = new bool[answerButtons.Length];

        if (resultText != null) resultText.text = "";
        if (questionText != null) questionText.gameObject.SetActive(true);

        if (confirmButton != null) { confirmButton.gameObject.SetActive(true); confirmButton.interactable = false; }
        if (backFromTestButton != null) backFromTestButton.gameObject.SetActive(false);

        ShowQuestion();
    }

    void ShowQuestion()
    {
        if (currentQuestion >= questions.Length)
        {
            EndTest();
            return;
        }

        if (questionText != null) questionText.text = questions[currentQuestion].question;
        if (resultText != null) resultText.text = "";

        for (int i = 0; i < answerButtons.Length; i++)
        {
            bool hasAnswer = i < questions[currentQuestion].answers.Length;
            if (answerButtons[i] != null)
            {
                answerButtons[i].gameObject.SetActive(hasAnswer);
                answerButtons[i].interactable = true;
                answerButtons[i].image.color = Color.white;
            }

            if (hasAnswer && i < answerTexts.Length && answerTexts[i] != null)
            {
                answerTexts[i].text = questions[currentQuestion].answers[i];
            }
            if (i < selectedAnswers.Length)
            {
                selectedAnswers[i] = false;
            }
        }

        if (confirmButton != null) confirmButton.interactable = false;
    }

    void SelectAnswer(int answerIndex)
    {
        if (answerIndex >= questions[currentQuestion].answers.Length || answerIndex >= selectedAnswers.Length) return;

        selectedAnswers[answerIndex] = !selectedAnswers[answerIndex];
        answerButtons[answerIndex].image.color = selectedAnswers[answerIndex] ? Color.yellow : Color.white;

        if (confirmButton != null) confirmButton.interactable = System.Array.Exists(selectedAnswers, x => x);
    }

    void ConfirmAnswer()
    {
        if (currentQuestion >= questions.Length) return;

        if (confirmButton != null) confirmButton.interactable = false;
        foreach (var button in answerButtons)
        {
            if (button != null) button.interactable = false;
        }

        bool allCorrect = true;
        for (int i = 0; i < questions[currentQuestion].answers.Length; i++)
        {
            if (i >= selectedAnswers.Length || i >= answerButtons.Length)
            {
                allCorrect = false;
                break;
            }

            bool isCorrectAnswer = System.Array.IndexOf(questions[currentQuestion].correctAnswers, i) != -1;
            if (selectedAnswers[i] != isCorrectAnswer)
            {
                allCorrect = false;
            }
        }

        for (int i = 0; i < questions[currentQuestion].answers.Length; i++)
        {
            if (i >= answerButtons.Length) break;

            bool isCorrect = System.Array.IndexOf(questions[currentQuestion].correctAnswers, i) != -1;
            if (isCorrect)
            {
                answerButtons[i].image.color = Color.green;
            }
            else if (selectedAnswers[i] && !isCorrect)
            {
                answerButtons[i].image.color = Color.red;
            }
        }

        if (allCorrect)
        {
            if (resultText != null) resultText.text = "Правильно!";
            score++;
        }
        else
        {
            if (resultText != null) resultText.text = "Неправильно!";
        }

        currentQuestion++;
        StartCoroutine(NextQuestionAfterDelay(2.0f));
    }

    IEnumerator NextQuestionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowQuestion();
    }

    void EndTest()
    {
        if (questionText != null) questionText.gameObject.SetActive(false);
        if (confirmButton != null) confirmButton.gameObject.SetActive(false);
        foreach (var button in answerButtons)
        {
            if (button != null) button.gameObject.SetActive(false);
        }

        if (resultText != null)
        {
            resultText.text = $"Тест завершен!\nВаш результат: {score} из {questions.Length}";
        }
        else
        {
            Debug.LogError("ОШИБКА: UI элемент 'resultText' не назначен в инспекторе!");
        }

        if (backFromTestButton != null)
        {
            backFromTestButton.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("ОШИБКА: Кнопка 'backFromTestButton' не назначена в инспекторе!");
        }
    }
    #endregion

    #region Мини-игра с кредитом
    void InitializeCreditGame()
    {
        if (amountSlider != null)
        {
            amountSlider.value = amountSlider.minValue;
            UpdateAmountText(amountSlider.value);
            amountSlider.interactable = true;
        }
        hasChosenTerm = false;
        if (resultTextCredit != null) resultTextCredit.text = "Выберите сумму и срок кредита";
        if (finalResultText != null) finalResultText.gameObject.SetActive(false);
        if (backFromTestButton != null) backFromTestButton.gameObject.SetActive(true);

        if (termSelectionGroup != null) termSelectionGroup.SetActive(true);
        if (earlyRepaymentGroup != null) earlyRepaymentGroup.SetActive(false);
        if (refinanceGroup != null) refinanceGroup.SetActive(false);
    }

    void UpdateAmountText(float value)
    {
        creditAmount = value;
        if (amountText != null) amountText.text = $"Сумма кредита: {creditAmount:F0} руб.";
    }

    void SelectTerm(int termIndex)
    {
        if (hasChosenTerm) return;

        switch (termIndex)
        {
            case 0: creditTerm = 1; interestRate = 0.15f; resultTextCredit.text = "Отличный выбор!"; break;
            case 1: creditTerm = 3; interestRate = 0.12f; resultTextCredit.text = "Первый вариант более выгоден."; break;
            case 2: creditTerm = 5; interestRate = 0.10f; resultTextCredit.text = "Первый вариант более выгоден."; break;
        }
        hasChosenTerm = true;
        if (amountSlider != null) amountSlider.interactable = false;

        initialOverpayment = creditAmount * interestRate * creditTerm;
        finalOverpayment = initialOverpayment;
        resultTextCredit.text += $"\nПереплата составит: {initialOverpayment:F0} руб.";

        StartCoroutine(ShowNextGroup(termSelectionGroup, earlyRepaymentGroup, 2.5f));
    }

    void HandleEarlyRepayment(int choiceIndex)
    {
        string feedback = "";
        bool advanceToNextStep = true;
        switch (choiceIndex)
        {
            case 0:
                finalOverpayment = initialOverpayment * 0.7f;
                feedback = "Хороший выбор, переплата сократилась на 30%";
                break;
            case 1:
                feedback = "Переплата не сократилась";
                break;
            case 2:
                feedback = "Подумайте еще раз.";
                advanceToNextStep = false;
                break;
        }
        resultTextCredit.text = feedback;
        if (advanceToNextStep)
        {
            StartCoroutine(ShowNextGroup(earlyRepaymentGroup, refinanceGroup, 2.5f));
        }
    }

    void HandleRefinance(int choiceIndex)
    {
        string feedback = "";
        bool finishGame = true;
        float refinanceReduction = finalOverpayment * 0.1f;
        switch (choiceIndex)
        {
            case 0:
                finalOverpayment -= refinanceReduction;
                feedback = "Переплата сократилась на 10%";
                break;
            case 1:
                feedback = "Переплата не сократилась";
                break;
            case 2:
                feedback = "Рекомендуем прочесть теоретический материал.";
                finishGame = false;
                break;
        }
        resultTextCredit.text = feedback;
        if (finishGame)
        {
            if (refinanceGroup != null) refinanceGroup.SetActive(false);
            ShowFinalResult();
            StartCoroutine(ReturnToMenuAfterDelay(4f));
        }
    }

    IEnumerator ReturnToMenuAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowPanel(teachingPanel);
    }

    IEnumerator ShowNextGroup(GameObject groupToHide, GameObject groupToShow, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (groupToHide != null) groupToHide.SetActive(false);
        if (groupToShow != null) groupToShow.SetActive(true);
        if (resultTextCredit != null) resultTextCredit.text = "Выберите действие:";
    }

    void ShowFinalResult()
    {
        if (finalResultText != null)
        {
            finalResultText.gameObject.SetActive(true);
            finalResultText.text = $"Итоговая переплата по кредиту составила: {finalOverpayment:F0} руб.";
        }
        if (resultTextCredit != null) resultTextCredit.text = "";
    }
    #endregion

    #region Финансовая безопасность
    void InitializeSecurityGame()
    {
        currentScam = 0;
        if (backFromTestButton != null) backFromTestButton.gameObject.SetActive(true);
        ShowScamScenario();
    }

    void ShowScamScenario()
    {
        if (currentScam >= scams.Length)
        {
            ShowPanel(teachingPanel);
            return;
        }

        if (scamMessageText != null) scamMessageText.text = scams[currentScam].message;
        if (securityResultText != null) securityResultText.text = "";

        for (int i = 0; i < responseButtons_Security.Length; i++)
        {
            if (responseButtons_Security[i] == null) continue;

            if (i < scams[currentScam].responses.Length)
            {
                responseButtons_Security[i].gameObject.SetActive(true);
                var textComponent = responseButtons_Security[i].GetComponentInChildren<TextMeshProUGUI>();
                if (textComponent != null) textComponent.text = scams[currentScam].responses[i];
            }
            else
            {
                responseButtons_Security[i].gameObject.SetActive(false);
            }
        }
    }

    void HandleScamResponse(int responseIndex)
    {
        if (securityResultText != null)
        {
            securityResultText.text = responseIndex == scams[currentScam].correctResponse
                ? "Правильно! " + scams[currentScam].explanation
                : "Опасность! " + scams[currentScam].explanation;
        }
        currentScam++;
        StartCoroutine(NextScamAfterDelay(3.5f));
    }

    IEnumerator NextScamAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (securityResultText != null) securityResultText.text = "";
        ShowScamScenario();
    }
    #endregion

    #region Вспомогательные методы
    private void SetAllButtonFontSizes(float size)
    {
        // Тексты на кнопках ответов в тесте
        foreach (var text in answerTexts)
        {
            if (text != null)
            {
                text.fontSize = size;
            }
        }

        // Кнопки навигации
        SetButtonFontSize(startTheoryButton, size);
        SetButtonFontSize(startTestButton, size);
        SetButtonFontSize(startCreditGameButton, size);
        SetButtonFontSize(startSecurityGameButton, size);
        SetButtonFontSize(backToMenuButton, size);
        SetButtonFontSize(backFromTestButton, size);

        // Кнопки в теоретической части
        SetButtonFontSize(nextTheoryButton, size);
        SetButtonFontSize(prevTheoryButton, size);
        SetButtonFontSize(closeTheoryButton, size);

        // Кнопка подтверждения в тесте
        SetButtonFontSize(confirmButton, size);

        // Кнопки в мини-игре с кредитом
        SetButtonArrayFontSize(termButtons, size);
        SetButtonArrayFontSize(earlyRepaymentButtons, size);
        SetButtonArrayFontSize(refinanceButtons, size);

        // Кнопки в игре про фин. безопасность
        SetButtonArrayFontSize(responseButtons_Security, size);
    }

    private void SetButtonFontSize(Button button, float size)
    {
        if (button != null)
        {
            TextMeshProUGUI textComponent = button.GetComponentInChildren<TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.fontSize = size;
            }
        }
    }

    private void SetButtonArrayFontSize(Button[] buttonArray, float size)
    {
        if (buttonArray == null) return;
        foreach (var button in buttonArray)
        {
            SetButtonFontSize(button, size);
        }
    }
    #endregion
}