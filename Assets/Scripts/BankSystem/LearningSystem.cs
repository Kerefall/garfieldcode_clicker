using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class FinancialLiteracyGame : MonoBehaviour
{
    [Header("Главное меню")]
    public GameObject mainMenuPanel;
    public Button theoryMenuButton;
    public Button testMenuButton;
    public Button creditGameButton;
    public Button securityGameButton;

    [Header("Теоретическая часть")]
    public GameObject theoryPanel;
    public Button nextTheoryButton;
    public Button prevTheoryButton;
    public Button closeTheoryButton;
    public TextMeshProUGUI theoryText;
    private int currentTheoryPage = 0;

    [Header("Выбор темы теории")]
    public GameObject theorySelectionPanel;
    public Button creditTheoryButton;
    public Button depositTheoryButton;
    public Button rateTheoryButton;
    public Button securityTheoryButton;
    public Button inflationTheoryButton;
    public Button backToMainMenuFromTheoryButton;

    [Header("Тестовая часть")]
    public GameObject testPanel;
    public TextMeshProUGUI questionText;
    public Button[] answerButtons;
    public TextMeshProUGUI[] answerTexts;
    public Button confirmButton;
    public Button backToMainMenuFromTestButton;
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

    [Header("Мини-игра с кредитом")]
    public GameObject creditGamePanel;
    public Slider amountSlider;
    public TextMeshProUGUI amountText;
    public Button confirmAmountButton;
    public TextMeshProUGUI instructionText;
    public Button[] choiceButtons;
    public TextMeshProUGUI finalResultText;
    public TextMeshProUGUI selectedAmountText;
    public Button closeResultButton;
    public Button backToMainMenuFromCreditGameButton;

    private float creditAmount;
    private int creditTerm;
    private float interestRate;
    private float initialOverpayment;
    private float finalOverpayment;
    private int currentStep = 1;

    private string[] creditInstructions = {
        "Выберите сумму кредита с помощью ползунка и подтвердите выбор",
        "Выберите срок кредита:",
        "У вас появились \"свободные\" деньги. Погасить часть кредита досрочно?",
        "Появился вариант кредита с более низким процентом. Комиссия за рефинансирование не делает эту операцию невыгодной. Провести рефинансирование?"
    };

    private string[][] creditChoices = {
        new string[] {},
        new string[] { "1 год (15%)", "1,5 года (13%)", "2 года (1%)" },
        new string[] { "Инвестирую деньги", "Оплачу досрочно", "Я не знаю" },
        new string[] { "Да", "Нет", "Что такое рефинансирование?" }
    };

    [Header("Финансовая безопасность")]
    public GameObject securityGamePanel;
    public TextMeshProUGUI scamMessageText;
    public Button[] responseButtons_Security;
    public TextMeshProUGUI securityResultText;
    public Button backToMainMenuFromSecurityButton;

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

    private string[][] theoryPagesByTopic = {
        // Кредит
        new string[] {
            "Иногда случается так, что деньги нужны здесь и сейчас. В этом вам поможет такой финансовый инструмент, как кредит.\r\nКредит – когда вам дают деньги в долг, но с условием вернуть больше, чем взяли. Разница между «взял» и «вернул» — это процент, под который выдаётся кредит.",
            "Из чего процент складывается?\r\n1.\tВ первую очередь процент зависит от ключевой ставки. Чем она выше, тем дороже кредиты",
            "2. Риски банка. Да, выдавать кредиты для банка риск потерять деньги, поэтому если у вас плохая крединая история или маленькая зарплата, то банк поднимет процент дабы перестраховаться",
            "3.\tПрибыль банка. Банк тоже хочет заработать и поэтому «накидывает» пару процентов",
            "Что такое переплата по кредиту?\r\nПереплата = Сумма кредита * Годовой процент * Срок в годах \r\nТо есть это то, сколько вы платите за возможность получить деньги здесь и сейчас.",
            "При оплате кредитов в первую очередь оплачивается переплата, а потом уже остальное. Для того, чтобы сократить переплату нужно уменьшать срок кредита и по возможности погашать досрочно.",
            "Но только если вы ещё не выплатили переплату. В противном случае лучше держать свободные деньги на накопительных счетах или инвестировать.",
            "В досрочном погашении кредита может помочь рефинансирование. Если появляется возможность открыть кредит под более низкий процент, то может быть выгодным открыть новый и погасить им старый. Но нужно учитывать, что за рефинансирование банк может взымать комиссию."
        },
        // Вклад
        new string[] {
            "Вклад является хорошим доступным и понятным средством для того, чтобы минимизировать влияние инфляции на ваши сбережения.\r\nВы отдаёте деньги банку на хранение, а банк платит вам за это проценты. Чем выше ключевая ставка, тем выше проценты на вкладах.",
            "Как это работает? Банк выдаёт ваши деньги в кредиты под более высокий процент и разницу оставляет себе.\r\nОбычно, чем меньше срок, на которой открывается вклад, тем выше процент. Это из-за того, что банку проще предсказать поведение экономики в стране на короткий срок, чем на более длинный."
        },
        // Ключевая ставка
        new string[] {
            "Ключевая ставка – процент, под который Центральный Банк выдаёт деньги коммерческим банкам. Следственно коммерческие банки выдают кредиты людям под более высокий процент.",
            "Если не вдаваться в подробности, то ключевая ставка в большей степени складывается на основании состояния экономики",
            "Цены растут слишком быстро (инфляция) – ЦБ повышает ставку – кредиты дорожают – покупательская способность снижается, магазины борются за покупателей путём снижения цен – инфляция замедляется и наоборот",
            "Цены падают слишком быстро (дефляция) – ЦБ понижает ставку – кредиты дешевеют – покупательская способность повышается, магазины поднимают цены – дефляция замедляется"
        },
        // Финансовая безопасность
        new string[] {
            "1.\t«Банк звонит»\r\n— Вам говорят, что ваш счёт взламывают, и просят:\r\no\tПеревести деньги на «безопасный счёт».\r\no\tНазвать данные карты, код из SMS или пароль от Госуслуг.\r\n→ Это обман! Банк никогда не просит такие данные.",
            "2.\t«Родственник просит денег»\r\no\tВ соцсетях или мессенджерах пишет «знакомый» с просьбой срочно перевести деньги\r\n→ Позвоните ему по видео (мошенники подделывают голос!).",
            "3.\t«Оплатите учёбу/тестирование»\r\no\tПриходит ссылка «от вуза/школы» с требованием оплатить или авторизоваться через Госуслуги.\r\n→ Проверьте домен (.ru/.рф/.su — официальные, остальные — подделки).",
            "4.\tФинансовые пирамиды\r\no\tВам обещают огромные доходы за «вложения» или приглашение друзей.\r\n→ Не верьте! Такие схемы рушатся, а деньги исчезают.",
            "5.\t«Оплата услуг»\r\no\tПриходит ссылка для оплаты коммунальных или других услуг с неофициального домена\r\n→ Все государственные сайты имеют домены .ru, .рф, .su",
            "Что делать, если попались?\r\n1.\tНемедленно заблокируйте карту/счёт через банк.\r\n2.\tПодайте заявление в полицию.\r\n3.\tПредупредите других!\r\nГлавное правило: не спешите и перепроверяйте информацию!"
        },
        // Инфляция
        new string[] {
            "Инфляция – устойчивое повышение общего уровня цен на товары и услуги. Простыми словами – со временем за одинаковое количество денег можно будет купить всё меньше товаров и услуг.",
            "Инфляция появляется, когда денег в стране становится слишком много, товаров не хватает, дорожают бензин и зарубежные товары, а люди и компании заранее поднимают цены, ожидая подорожания.",
            "Инфляция влечёт за собой очень много последствий, но основные, которые имеют для тебя значение:\r\n1)\tДеньги на вашем счёте или в копилке теряют «ценность», цены растут\r\n2)\tДля снижения инфляции государство повышает ключевую ставку, о которой расскажем в другом разделе",
            "Если инфляция — это рост цен (когда деньги обесцениваются), то дефляция — обратный процесс, когда цены падают, а деньги дорожают, но это сейчас редкое явление, так как большинство стран целенаправленно поддерживают умеренную инфляцию."
        }
    };

    private string[] currentTheoryPages;

    [Header("Звуковые эффекты")]
    public AudioSource audioSource;
    public AudioClip correctAnswerSound;
    public AudioClip wrongAnswerSound;

    void Start()
    {
        SetAllButtonFontSizes(65f);

        if (amountSlider != null)
        {
            amountSlider.minValue = 10000;
            amountSlider.maxValue = 300000;
            amountSlider.value = 100000;
        }

        // Навигация главного меню
        if (theoryMenuButton != null) theoryMenuButton.onClick.AddListener(() => ShowPanel(theorySelectionPanel));
        if (testMenuButton != null) testMenuButton.onClick.AddListener(() => { ShowPanel(testPanel); InitializeTest(); });
        if (creditGameButton != null) creditGameButton.onClick.AddListener(StartCreditGame);
        if (securityGameButton != null) securityGameButton.onClick.AddListener(() => { ShowPanel(securityGamePanel); InitializeSecurityGame(); });

        // Навигация теории
        if (nextTheoryButton != null) nextTheoryButton.onClick.AddListener(NextTheoryPage);
        if (prevTheoryButton != null) prevTheoryButton.onClick.AddListener(PrevTheoryPage);
        if (closeTheoryButton != null) closeTheoryButton.onClick.AddListener(() => ShowPanel(theorySelectionPanel));
        if (backToMainMenuFromTheoryButton != null) backToMainMenuFromTheoryButton.onClick.AddListener(() => ShowPanel(mainMenuPanel));

        // Кнопки выбора темы теории
        if (creditTheoryButton != null) creditTheoryButton.onClick.AddListener(() => StartTheory(0));
        if (depositTheoryButton != null) depositTheoryButton.onClick.AddListener(() => StartTheory(1));
        if (rateTheoryButton != null) rateTheoryButton.onClick.AddListener(() => StartTheory(2));
        if (securityTheoryButton != null) securityTheoryButton.onClick.AddListener(() => StartTheory(3));
        if (inflationTheoryButton != null) inflationTheoryButton.onClick.AddListener(() => StartTheory(4));

        // Тест
        if (backToMainMenuFromTestButton != null) backToMainMenuFromTestButton.onClick.AddListener(() => ShowPanel(mainMenuPanel));

        // Кредитная игра
        if (amountSlider != null) amountSlider.onValueChanged.AddListener(UpdateAmountText);
        if (confirmAmountButton != null) confirmAmountButton.onClick.AddListener(ConfirmAmount);
        if (closeResultButton != null) closeResultButton.onClick.AddListener(CloseFinalResult);
        if (backToMainMenuFromCreditGameButton != null) backToMainMenuFromCreditGameButton.onClick.AddListener(() => ShowPanel(mainMenuPanel));

        // Финансовая безопасность
        if (backToMainMenuFromSecurityButton != null) backToMainMenuFromSecurityButton.onClick.AddListener(() => ShowPanel(mainMenuPanel));

        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i;
            if (answerButtons[i] != null) answerButtons[i].onClick.AddListener(() => SelectAnswer(index));
        }
        if (confirmButton != null) confirmButton.onClick.AddListener(ConfirmAnswer);

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            int index = i;
            if (choiceButtons[i] != null) choiceButtons[i].onClick.AddListener(() => HandleCreditChoice(index));
        }

        for (int i = 0; i < responseButtons_Security.Length; i++)
        {
            int responseIndex = i;
            if (responseButtons_Security[i] != null) responseButtons_Security[i].onClick.AddListener(() => HandleScamResponse(responseIndex));
        }

        ShowPanel(mainMenuPanel);
    }

    void ShowPanel(GameObject panelToShow)
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(panelToShow == mainMenuPanel);
        if (theorySelectionPanel != null) theorySelectionPanel.SetActive(panelToShow == theorySelectionPanel);
        if (theoryPanel != null) theoryPanel.SetActive(panelToShow == theoryPanel);
        if (testPanel != null) testPanel.SetActive(panelToShow == testPanel);
        if (creditGamePanel != null) creditGamePanel.SetActive(panelToShow == creditGamePanel);
        if (securityGamePanel != null) securityGamePanel.SetActive(panelToShow == securityGamePanel);
    }

    #region Теоретическая часть
    void StartTheory(int topicIndex)
    {
        if (topicIndex >= 0 && topicIndex < theoryPagesByTopic.Length)
        {
            currentTheoryPages = theoryPagesByTopic[topicIndex];
            currentTheoryPage = 0;
            UpdateTheoryText();
            ShowPanel(theoryPanel);
        }
    }

    void NextTheoryPage()
    {
        if (currentTheoryPage < currentTheoryPages.Length - 1)
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
        if (theoryText != null && currentTheoryPages != null && currentTheoryPage < currentTheoryPages.Length)
        {
            theoryText.text = currentTheoryPages[currentTheoryPage];
        }
        if (prevTheoryButton != null) prevTheoryButton.interactable = (currentTheoryPage > 0);
        if (nextTheoryButton != null) nextTheoryButton.interactable = (currentTheoryPage < currentTheoryPages.Length - 1);
    }
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
        if (backToMainMenuFromTestButton != null) backToMainMenuFromTestButton.gameObject.SetActive(false);

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

                TextMeshProUGUI buttonText = answerButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null && hasAnswer)
                {
                    buttonText.text = questions[currentQuestion].answers[i];
                    buttonText.fontSize = 65;
                }
                else if (hasAnswer && i < answerTexts.Length && answerTexts[i] != null)
                {
                    answerTexts[i].text = questions[currentQuestion].answers[i];
                    answerTexts[i].fontSize = 65;
                }
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
            PlaySound(correctAnswerSound);
            score++;
        }
        else
        {
            if (resultText != null) resultText.text = "Неправильно!";
            PlaySound(wrongAnswerSound);
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
            resultText.fontSize = 65;
        }

        if (backToMainMenuFromTestButton != null) backToMainMenuFromTestButton.gameObject.SetActive(true);
    }
    #endregion

    #region Мини-игра с кредитом
    void StartCreditGame()
    {
        ShowPanel(creditGamePanel);
        currentStep = 1;
        InitializeCreditGame();
    }

    void InitializeCreditGame()
    {
        if (amountSlider != null)
        {
            amountSlider.minValue = 10000;
            amountSlider.maxValue = 300000;
            amountSlider.value = 100000;
            UpdateAmountText(amountSlider.value);
            amountSlider.interactable = true;
            amountSlider.gameObject.SetActive(true);
        }

        if (confirmAmountButton != null)
        {
            confirmAmountButton.gameObject.SetActive(true);
            confirmAmountButton.interactable = true;
        }

        if (selectedAmountText != null)
        {
            selectedAmountText.gameObject.SetActive(false);
        }

        if (finalResultText != null)
        {
            finalResultText.gameObject.SetActive(false);
        }

        if (closeResultButton != null)
        {
            closeResultButton.gameObject.SetActive(false);
        }

        if (backToMainMenuFromCreditGameButton != null)
        {
            backToMainMenuFromCreditGameButton.gameObject.SetActive(false);
        }

        foreach (var button in choiceButtons)
        {
            if (button != null) button.gameObject.SetActive(false);
        }

        UpdateCreditGameUI();
    }

    void UpdateCreditGameUI()
    {
        if (currentStep < 1 || currentStep > creditInstructions.Length) return;

        if (instructionText != null)
        {
            instructionText.text = creditInstructions[currentStep - 1];
        }

        if (currentStep >= 2 && currentStep <= 4)
        {
            for (int i = 0; i < choiceButtons.Length; i++)
            {
                if (choiceButtons[i] != null)
                {
                    bool shouldShow = i < creditChoices[currentStep - 1].Length;
                    choiceButtons[i].gameObject.SetActive(shouldShow);

                    if (shouldShow)
                    {
                        TextMeshProUGUI buttonText = choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                        if (buttonText != null)
                        {
                            buttonText.text = creditChoices[currentStep - 1][i];
                        }
                    }
                }
            }
        }
    }

    void UpdateAmountText(float value)
    {
        creditAmount = value;
        if (amountText != null)
        {
            amountText.text = $"Сумма кредита: {creditAmount:F0} руб.";
        }
    }

    void ConfirmAmount()
    {
        if (currentStep == 1)
        {
            if (amountSlider != null) amountSlider.gameObject.SetActive(false);
            if (confirmAmountButton != null) confirmAmountButton.gameObject.SetActive(false);

            if (selectedAmountText != null)
            {
                selectedAmountText.text = $"Выбранная сумма: {creditAmount:F0} руб.";
                selectedAmountText.gameObject.SetActive(true);
            }

            currentStep = 2;
            UpdateCreditGameUI();
        }
    }

    void HandleCreditChoice(int choiceIndex)
    {
        switch (currentStep)
        {
            case 2:
                HandleTermSelection(choiceIndex);
                break;
            case 3:
                HandleEarlyRepayment(choiceIndex);
                break;
            case 4:
                HandleRefinance(choiceIndex);
                break;
        }
    }

    void HandleTermSelection(int choiceIndex)
    {
        foreach (var button in choiceButtons)
        {
            if (button != null) button.gameObject.SetActive(false);
        }

        switch (choiceIndex)
        {
            case 0:
                creditTerm = 1;
                interestRate = 0.15f;
                if (instructionText != null) instructionText.text = "Отличный выбор!";
                break;
            case 1:
                creditTerm = 3;
                interestRate = 0.12f;
                if (instructionText != null) instructionText.text = "Первый вариант более выгоден";
                break;
            case 2:
                creditTerm = 5;
                interestRate = 0.10f;
                if (instructionText != null) instructionText.text = "Первый вариант более выгоден";
                break;
        }

        initialOverpayment = creditAmount * interestRate * creditTerm;
        finalOverpayment = initialOverpayment;
        if (instructionText != null) instructionText.text += $"\nПереплата составит: {initialOverpayment:F0} руб.";

        currentStep = 3;
        StartCoroutine(ShowNextStepAfterDelay(2.5f));
    }

    void HandleEarlyRepayment(int choiceIndex)
    {
        foreach (var button in choiceButtons)
        {
            if (button != null) button.gameObject.SetActive(false);
        }

        string feedback = "";
        switch (choiceIndex)
        {
            case 0:
                finalOverpayment = initialOverpayment * 0.7f;
                feedback = "Вы преумножили капитал в 1,5 раза";
                PlaySound(correctAnswerSound);
                break;
            case 1:
                feedback = "Переплата не сократилась";
                PlaySound(wrongAnswerSound);
                break;
            case 2:
                feedback = "Подумайте еще раз";
                StartCoroutine(ShowNextStepAfterDelay(1.5f));
                return;
        }

        if (instructionText != null) instructionText.text = feedback;

        currentStep = 4;
        StartCoroutine(ShowNextStepAfterDelay(2.5f));
    }

    void HandleRefinance(int choiceIndex)
    {
        foreach (var button in choiceButtons)
        {
            if (button != null) button.gameObject.SetActive(false);
        }

        string feedback = "";
        switch (choiceIndex)
        {
            case 0:
                finalOverpayment *= 0.9f;
                feedback = "Переплата сократилась на 10%";
                PlaySound(correctAnswerSound);
                break;
            case 1:
                feedback = "Переплата не сократилась";
                PlaySound(wrongAnswerSound);
                break;
            case 2:
                feedback = "Рекомендуем прочесть теоретический материал";
                StartCoroutine(ShowNextStepAfterDelay(1.5f));
                return;
        }

        if (instructionText != null) instructionText.text = feedback;

        StartCoroutine(ShowFinalResultWithDelay(2f));
    }

    IEnumerator ShowFinalResultWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (instructionText != null)
        {
            instructionText.text = "";
        }

        ShowFinalResult();
    }

    void ShowFinalResult()
    {
        if (finalResultText != null)
        {
            finalResultText.gameObject.SetActive(true);
            finalResultText.text = $"Итоговый результат:\n\n" +
                                 $"Сумма кредита: {creditAmount:F0} руб.\n" +
                                 $"Срок: {creditTerm} год(а)\n" +
                                 $"Процентная ставка: {interestRate * 100}%\n\n" +
                                 $"Изначальная переплата: {initialOverpayment:F0} руб.\n" +
                                 $"Итоговая переплата: {finalOverpayment:F0} руб.\n" +
                                 $"Экономия: {initialOverpayment - finalOverpayment:F0} руб.";
        }

        if (closeResultButton != null)
        {
            closeResultButton.gameObject.SetActive(true);
        }

        if (backToMainMenuFromCreditGameButton != null)
        {
            backToMainMenuFromCreditGameButton.gameObject.SetActive(true);
        }
    }

    void CloseFinalResult()
    {
        if (finalResultText != null)
            finalResultText.gameObject.SetActive(false);

        if (closeResultButton != null)
            closeResultButton.gameObject.SetActive(false);

        ShowPanel(mainMenuPanel);
    }

    IEnumerator ShowNextStepAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        UpdateCreditGameUI();
    }
    #endregion

    #region Финансовая безопасность
    void InitializeSecurityGame()
    {
        currentScam = 0;
        if (backToMainMenuFromSecurityButton != null) backToMainMenuFromSecurityButton.gameObject.SetActive(true);
        ShowScamScenario();
    }

    void ShowScamScenario()
    {
        if (currentScam >= scams.Length)
        {
            ShowPanel(mainMenuPanel);
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

        if (responseIndex == scams[currentScam].correctResponse)
        {
            PlaySound(correctAnswerSound);
        }
        else
        {
            PlaySound(wrongAnswerSound);
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
        foreach (var text in answerTexts)
        {
            if (text != null)
            {
                text.fontSize = size;
            }
        }

        SetButtonFontSize(theoryMenuButton, size);
        SetButtonFontSize(testMenuButton, size);
        SetButtonFontSize(creditGameButton, size);
        SetButtonFontSize(securityGameButton, size);

        SetButtonFontSize(creditTheoryButton, size);
        SetButtonFontSize(depositTheoryButton, size);
        SetButtonFontSize(rateTheoryButton, size);
        SetButtonFontSize(securityTheoryButton, size);
        SetButtonFontSize(inflationTheoryButton, size);
        SetButtonFontSize(backToMainMenuFromTheoryButton, size);

        SetButtonFontSize(nextTheoryButton, size);
        SetButtonFontSize(prevTheoryButton, size);
        SetButtonFontSize(closeTheoryButton, size);

        SetButtonFontSize(confirmButton, size);
        SetButtonFontSize(backToMainMenuFromTestButton, size);

        SetButtonFontSize(confirmAmountButton, size);
        SetButtonFontSize(closeResultButton, size);
        SetButtonFontSize(backToMainMenuFromCreditGameButton, size);

        SetButtonFontSize(backToMainMenuFromSecurityButton, size);

        foreach (var button in choiceButtons)
        {
            SetButtonFontSize(button, size);
        }

        foreach (var button in responseButtons_Security)
        {
            SetButtonFontSize(button, size);
        }
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

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
    #endregion
}