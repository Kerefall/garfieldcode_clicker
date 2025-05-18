using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class CreditSystem : MonoBehaviour
{
    [Header("Элементы интерфейса")]
    public GameObject creditPanel;
    public TMP_InputField creditAmountInput;
    public TMP_Dropdown creditDurationDropdown;
    public TextMeshProUGUI currentBalanceText;
    public TextMeshProUGUI creditDebtText;
    public TextMeshProUGUI interestRateText;
    public TextMeshProUGUI timeLeftText;
    public TextMeshProUGUI nextPaymentText;
    public Button takeCreditButton;
    public Button repayCreditButton;
    public Button payInstallmentButton;

    [Header("Настройки кредита")]
    [SerializeField] private float annualInterestRate = 0.15f;
    [SerializeField] private float minCreditAmount = 100f;
    [SerializeField] private float penaltyRate = 0.2f;

    private int[] availableLoanTerms = { 1, 3, 6 };
    private float creditAmount = 0f;
    private float creditInterest = 0f;
    private DateTime creditEndTime;
    private DateTime nextPaymentDate;
    private bool isOverdue = false;
    private Coroutine interestCoroutine;
    private Coroutine timerCoroutine;
    private float gameTimeMultiplier = 43200f; // 1 день = 2 секунды (86400/2)
    private DateTime gameStartTime;
    private int remainingPayments;
    private float monthlyPayment;

    void Start()
    {
        gameStartTime = DateTime.Now;

        ConfigureDropdown();
        SetupEventListeners();
        LoadCreditData();
        UpdateBalanceImmediately();
        RefreshAllUI();

        if (creditAmount > 0)
        {
            interestCoroutine = StartCoroutine(InterestCalculation());
            timerCoroutine = StartCoroutine(CreditTimer());
        }
    }

    void Update()
    {
        if (creditAmount > 0)
        {
            UpdateTimerUI();
            UpdatePaymentUI();
        }
    }

    private void UpdateBalanceImmediately()
    {
        if (Clicker.Instance != null && currentBalanceText != null)
        {
            currentBalanceText.text = $"Баланс: {Clicker.Instance.Money:F2}₽";
            currentBalanceText.ForceMeshUpdate();
            Canvas.ForceUpdateCanvases();
        }
    }

    private void ConfigureDropdown()
    {
        creditDurationDropdown.ClearOptions();
        foreach (var term in availableLoanTerms)
        {
            creditDurationDropdown.options.Add(new TMP_Dropdown.OptionData($"{term} {(term == 1 ? "месяц" : term < 5 ? "месяца" : "месяцев")}"));
        }

        if (creditDurationDropdown.template != null)
        {
            var template = creditDurationDropdown.template;
            var scrollRect = template.GetComponent<ScrollRect>();
            if (scrollRect != null) scrollRect.scrollSensitivity = 20f;

            var item = template.Find("Viewport/Content/Item")?.GetComponent<LayoutElement>();
            if (item != null) item.minHeight = 40f;

            var itemText = template.Find("Viewport/Content/Item/Item Background/Item Label")?.GetComponent<TextMeshProUGUI>();
            if (itemText != null)
            {
                itemText.fontSize = 24;
                itemText.alignment = TextAlignmentOptions.MidlineLeft;
                itemText.margin = new Vector4(10, 0, 0, 0);
            }
        }
        creditDurationDropdown.RefreshShownValue();
    }

    private DateTime GetCurrentGameTime()
    {
        TimeSpan realTimePassed = DateTime.Now - gameStartTime;
        TimeSpan gameTimePassed = new TimeSpan((long)(realTimePassed.Ticks * gameTimeMultiplier));
        return gameStartTime + gameTimePassed;
    }

    private void SetupEventListeners()
    {
        takeCreditButton.onClick.AddListener(TakeCredit);
        repayCreditButton.onClick.AddListener(RepayFullCredit);
        payInstallmentButton.onClick.AddListener(PayMonthlyInstallment);
    }

    private void LoadCreditData()
    {
        creditAmount = PlayerPrefs.GetFloat("Credit_Amount", 0f);
        creditInterest = PlayerPrefs.GetFloat("Credit_Interest", 0f);
        remainingPayments = PlayerPrefs.GetInt("Remaining_Payments", 0);

        if (PlayerPrefs.HasKey("Credit_EndTime"))
        {
            long endTimeBinary = Convert.ToInt64(PlayerPrefs.GetString("Credit_EndTime"));
            creditEndTime = DateTime.FromBinary(endTimeBinary);
        }

        if (PlayerPrefs.HasKey("Next_Payment_Date"))
        {
            long paymentTimeBinary = Convert.ToInt64(PlayerPrefs.GetString("Next_Payment_Date"));
            nextPaymentDate = DateTime.FromBinary(paymentTimeBinary);
        }

        isOverdue = PlayerPrefs.GetInt("Credit_IsOverdue", 0) == 1;
    }

    private void SaveCreditData()
    {
        PlayerPrefs.SetFloat("Credit_Amount", creditAmount);
        PlayerPrefs.SetFloat("Credit_Interest", creditInterest);
        PlayerPrefs.SetString("Credit_EndTime", creditEndTime.ToBinary().ToString());
        PlayerPrefs.SetString("Next_Payment_Date", nextPaymentDate.ToBinary().ToString());
        PlayerPrefs.SetInt("Remaining_Payments", remainingPayments);
        PlayerPrefs.SetInt("Credit_IsOverdue", isOverdue ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void ClearCreditData()
    {
        PlayerPrefs.DeleteKey("Credit_Amount");
        PlayerPrefs.DeleteKey("Credit_Interest");
        PlayerPrefs.DeleteKey("Credit_EndTime");
        PlayerPrefs.DeleteKey("Next_Payment_Date");
        PlayerPrefs.DeleteKey("Remaining_Payments");
        PlayerPrefs.DeleteKey("Credit_IsOverdue");
        PlayerPrefs.Save();
    }

    private void RefreshAllUI()
    {
        UpdateBalanceUI();
        UpdateDebtUI();
        UpdateTimerUI();
        UpdatePaymentUI();
        ForceUIRebuild();
    }

    private void UpdateBalanceUI()
    {
        if (Clicker.Instance != null && currentBalanceText != null)
        {
            currentBalanceText.text = $"Баланс: {Clicker.Instance.Money:F2}₽";
            currentBalanceText.ForceMeshUpdate();
        }
    }

    private void UpdateDebtUI()
    {
        if (creditDebtText != null && interestRateText != null)
        {
            creditDebtText.text = $"Общий долг: {CalculateTotalDebt():F2}₽";
            interestRateText.text = $"Процентная ставка: {annualInterestRate * 100}% годовых";
            creditDebtText.ForceMeshUpdate();
        }
    }

    private void UpdateTimerUI()
    {
        if (timeLeftText != null && creditAmount > 0)
        {
            DateTime currentGameTime = GetCurrentGameTime();
            TimeSpan remaining = creditEndTime - currentGameTime;
            int remainingDays = (int)remaining.TotalDays;

            if (remainingDays > 0)
            {
                timeLeftText.text = $"Осталось дней: {remainingDays}";
                timeLeftText.color = Color.white;
            }
            else
            {
                timeLeftText.text = $"ПРОСРОЧЕНО! Штраф +{penaltyRate * 100}%";
                timeLeftText.color = Color.red;
            }
            timeLeftText.ForceMeshUpdate();
        }
    }

    private void UpdatePaymentUI()
    {
        if (nextPaymentText != null && creditAmount > 0)
        {
            DateTime currentGameTime = GetCurrentGameTime();
            TimeSpan remaining = nextPaymentDate - currentGameTime;
            int remainingDays = (int)remaining.TotalDays;

            nextPaymentText.text = $"Следующий платёж: {monthlyPayment:F2}₽ (через {remainingDays} дн.)";
            nextPaymentText.color = remainingDays <= 3 ? Color.yellow : Color.white;
        }
    }

    private void ForceUIRebuild()
    {
        Canvas.ForceUpdateCanvases();
        if (timeLeftText != null) LayoutRebuilder.ForceRebuildLayoutImmediate(timeLeftText.rectTransform);
        if (currentBalanceText != null) LayoutRebuilder.ForceRebuildLayoutImmediate(currentBalanceText.rectTransform);
        if (nextPaymentText != null) LayoutRebuilder.ForceRebuildLayoutImmediate(nextPaymentText.rectTransform);
    }

    private float CalculateTotalDebt()
    {
        float total = creditAmount + creditInterest;
        return isOverdue ? total * (1 + penaltyRate) : total;
    }

    public void TakeCredit()
    {
        if (!float.TryParse(creditAmountInput.text, out float amount) || amount < minCreditAmount)
        {
            Debug.LogError($"Минимальная сумма кредита: {minCreditAmount}₽");
            return;
        }

        if (creditAmount > 0)
        {
            Debug.LogError("У вас уже есть активный кредит!");
            return;
        }

        int term = availableLoanTerms[creditDurationDropdown.value];
        creditAmount = amount;
        creditEndTime = GetCurrentGameTime().AddDays(term * 30); // 30 дней = 60 секунд
        nextPaymentDate = GetCurrentGameTime().AddDays(30); // Первый платёж через месяц
        remainingPayments = term;

        // Рассчитываем ежемесячный платёж (аннуитетный)
        float monthlyRate = annualInterestRate / 12f;
        monthlyPayment = (amount * monthlyRate) / (1 - Mathf.Pow(1 + monthlyRate, -term));

        Clicker.Instance.Money += amount;
        Clicker.Instance.UpdateUI();

        if (interestCoroutine != null) StopCoroutine(interestCoroutine);
        if (timerCoroutine != null) StopCoroutine(timerCoroutine);

        interestCoroutine = StartCoroutine(InterestCalculation());
        timerCoroutine = StartCoroutine(CreditTimer());

        SaveCreditData();
        RefreshAllUI();

        Debug.Log($"Взят кредит на сумму: {amount:F2}₽, сроком на {term} {(term == 1 ? "месяц" : term < 5 ? "месяца" : "месяцев")}");
        Debug.Log($"Ежемесячный платёж: {monthlyPayment:F2}₽");
    }

    public void PayMonthlyInstallment()
    {
        if (creditAmount <= 0) return;

        DateTime currentGameTime = GetCurrentGameTime();
        if (currentGameTime < nextPaymentDate)
        {
            Debug.Log("Ещё рано вносить платёж!");
            return;
        }

        if (Clicker.Instance.Money >= monthlyPayment)
        {
            Clicker.Instance.Money -= monthlyPayment;
            Clicker.Instance.UpdateUI();

            // Уменьшаем основной долг
            float principalPayment = monthlyPayment - (creditAmount * (annualInterestRate / 12f));
            creditAmount -= principalPayment;

            remainingPayments--;
            nextPaymentDate = nextPaymentDate.AddDays(30); // Следующий платёж через месяц

            if (remainingPayments <= 0)
            {
                creditAmount = 0;
                creditInterest = 0;
                if (interestCoroutine != null) StopCoroutine(interestCoroutine);
                if (timerCoroutine != null) StopCoroutine(timerCoroutine);
                ClearCreditData();
                Debug.Log("Кредит полностью погашен!");
            }

            SaveCreditData();
            RefreshAllUI();
            Debug.Log($"Внесён платёж: {monthlyPayment:F2}₽. Осталось платежей: {remainingPayments}");
        }
        else
        {
            Debug.LogError($"Недостаточно средств для платежа! Нужно: {monthlyPayment:F2}₽");
        }
    }

    public void RepayFullCredit()
    {
        if (creditAmount <= 0) return;

        float totalDebt = CalculateTotalDebt();

        if (Clicker.Instance.Money >= totalDebt)
        {
            Clicker.Instance.Money -= totalDebt;
            Clicker.Instance.UpdateUI();

            creditAmount = 0;
            creditInterest = 0;

            if (interestCoroutine != null) StopCoroutine(interestCoroutine);
            if (timerCoroutine != null) StopCoroutine(timerCoroutine);

            ClearCreditData();
            RefreshAllUI();

            Debug.Log("Кредит полностью погашен досрочно!");
        }
        else
        {
            Debug.LogError($"Недостаточно средств! Нужно: {totalDebt:F2}₽, у вас: {Clicker.Instance.Money:F2}₽");
        }
    }

    private IEnumerator InterestCalculation()
    {
        while (creditAmount > 0)
        {
            yield return new WaitForSeconds(14f); // Начисление каждые 14 секунд (игровая неделя)

            // Начисляем проценты только на оставшуюся сумму
            float weeklyRate = annualInterestRate / 52f;
            creditInterest += creditAmount * weeklyRate;

            SaveCreditData();
            UpdateDebtUI();
        }
    }

    private IEnumerator CreditTimer()
    {
        while (creditAmount > 0)
        {
            DateTime currentGameTime = GetCurrentGameTime();

            // Проверка просрочки платежа
            if (currentGameTime > nextPaymentDate && !isOverdue)
            {
                isOverdue = true;
                creditInterest += monthlyPayment * penaltyRate; // Штраф за просрочку
                Debug.LogWarning("Просрочка платежа! Начислен штраф.");
            }

            // Проверка окончания срока кредита
            if (currentGameTime > creditEndTime)
            {
                isOverdue = true;
                SaveCreditData();
                UpdateTimerUI();
                UpdateDebtUI();
                Debug.LogWarning("Кредит просрочен!");
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus) SaveCreditData();
    }

    void OnApplicationQuit()
    {
        SaveCreditData();
    }
}