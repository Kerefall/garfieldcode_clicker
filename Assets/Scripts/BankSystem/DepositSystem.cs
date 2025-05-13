using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using TMPro;

public class BankSystem : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject depositPanel;
    public TMP_InputField depositAmountInput; // Deposit
    public TMP_Dropdown depositDurationDropdown; // Time vklada
    public TextMeshProUGUI currentBalanceText;
    public TextMeshProUGUI depositBalanceText;
    public TextMeshProUGUI interestRateText;
    public TextMeshProUGUI timeLeftText;
    public Button depositButton;
    public Button withdrawButton;

    [Header("Настройки вклада")]
    [SerializeField] private float annualRate = 0.10f;
    [SerializeField] private float maxDepositDuration = 365f; // Max time vklada
    [Tooltip("Время одного игрового круга в минутах")]
    [SerializeField] private float oneCircleTime = 5f;

    //private float playerBalance = Clicker.Instance.Money;
    private float depositAmount = 0f;
    private float depositInterest = 0f;
    private int depositDurationDays = 0;
    private float depositDays;

    private Coroutine interestCoroutine;
    private Coroutine timerCoroutine;

    void Start()
    {
        UpdateUI();
        depositButton.onClick.AddListener(CreateDeposit);
        withdrawButton.onClick.AddListener(WithdrawDeposit);
        depositDurationDropdown.ClearOptions();
        depositDurationDropdown.AddOptions(new System.Collections.Generic.List<string>
        {
            "1 месяц", "3 месяца", "6 месяцев", "1 год", "2 года", "3 года", "5 лет"
        });
    }
     
    private void UpdateUI()
    {
        currentBalanceText.text = $"Баланс: {Clicker.Instance.Money:F2}";
        depositBalanceText.text = $"{depositAmount:F2}";
        interestRateText.text = $"{(annualRate - 0.05f) * 100}%";

        if (depositAmount > 0)
        {
            var timeLeft = depositDurationDays - depositDays;
            timeLeftText.text = $"Времени осталось: {timeLeft}d ";
        }
        else
        {
            timeLeftText.text = "Нет активных вкладов";
        }
    }

    public void CreateDeposit()
    {

        if (float.TryParse(depositAmountInput.text, out float amount))
        {

            int durationIndex = depositDurationDropdown.value;
            int[] durationDaysOptions = { 30, 90, 180, 365, 730, 1095, 1825 };

            if (durationIndex >= durationDaysOptions.Length) return;

            int durationDays = durationDaysOptions[durationIndex];

            if (amount > 0 && durationDays <= maxDepositDuration && amount <= Clicker.Instance.Money)
            {
                Debug.Log("Dep was created");
                Clicker.Instance.Money -= amount;
                depositAmount = amount;
                depositDurationDays = durationDays;
                depositInterest = 0f;

                if (interestCoroutine != null) StopCoroutine(interestCoroutine);
                if (timerCoroutine != null) StopCoroutine(timerCoroutine);

                interestCoroutine = StartCoroutine(CalculateInterest());
                timerCoroutine = StartCoroutine(DepositTimer());

                UpdateUI();
            }
        }
    }

    public void WithdrawDeposit()
    {
        if (depositAmount > 0)
        {
            Clicker.Instance.Money += depositAmount;

            depositAmount = 0;
            depositInterest = 0;

            if (interestCoroutine != null) StopCoroutine(interestCoroutine);
            if (timerCoroutine != null) StopCoroutine(timerCoroutine);

            UpdateUI();
            Debug.Log("Вклад был закрыт раньше срока -- все проценты сгорели.");
        }
    }

    private IEnumerator CalculateInterest()
    {
        while (true)
        {
            yield return new WaitForSeconds(oneCircleTime * 60f);

            if (depositDays < depositDurationDays)
            {
                var depositRate = (annualRate - 0.05f) * 100;
                var circleInterest = (depositAmount * depositRate * depositDurationDays) / (100 * 365);
                depositInterest += circleInterest;
                depositDays += 1;

                UpdateUI();
                Debug.Log($"Проценты круга: +{circleInterest:F2}");
            }
        }
    }

    private IEnumerator DepositTimer()
    {
        while (depositDays < depositDurationDays)
        {
            UpdateUI();
            yield return new WaitForSeconds(1f); 
        }

        CompleteDeposit();
    }

    private void CompleteDeposit()
    {
        Clicker.Instance.Money += depositAmount + depositInterest;

        Debug.Log($"Вклад завершен! Зачислено: {depositAmount:F2} + проценты: {depositInterest:F2}");

        depositAmount = 0;
        depositInterest = 0;

        if (interestCoroutine != null) StopCoroutine(interestCoroutine);
        if (timerCoroutine != null) StopCoroutine(timerCoroutine);

        UpdateUI();
    }
}