using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using TMPro;

public class BankSystem : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject depositPanel;
    public InputField depositAmountInput; // Deposit
    public InputField depositDurationInput; // Time vklada
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

    private float playerBalance = 1000f;
    private float depositAmount = 0f;
    private float depositInterest = 0f;
    private float depositDurationDays = 0f;
    private DateTime depositEndTime;

    private Coroutine interestCoroutine;
    private Coroutine timerCoroutine;

    void Start()
    {
        UpdateUI();
        depositButton.onClick.AddListener(CreateDeposit);
        withdrawButton.onClick.AddListener(WithdrawDeposit);
    }

    private void UpdateUI()
    {
        currentBalanceText.text = $"Баланс: {playerBalance:F2}";
        depositBalanceText.text = $"Счёт вклада: {depositAmount:F2}";
        interestRateText.text = $"Процентная ставка: {(annualRate - 0.05f) * 100}% в круг";

        if (depositAmount > 0)
        {
            TimeSpan timeLeft = depositEndTime - DateTime.Now;
            timeLeftText.text = $"Времени осталось: {timeLeft.Days}d {timeLeft.Hours}ч {timeLeft.Minutes}м";
        }
        else
        {
            timeLeftText.text = "Нет активных вкладов";
        }
    }

    public void CreateDeposit()
    {
        if (float.TryParse(depositAmountInput.text, out float amount) &&
            float.TryParse(depositDurationInput.text, out float durationDays))
        {
            if (amount <= 0 || durationDays <= 0) return;

            if (amount <= playerBalance && durationDays <= maxDepositDuration)
            {
                playerBalance -= amount;
                depositAmount = amount;
                depositDurationDays = durationDays;
                depositInterest = 0f;

                depositEndTime = DateTime.Now.AddDays(durationDays);

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
            playerBalance += depositAmount;

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

            if (DateTime.Now < depositEndTime)
            {
                var depositRate = (annualRate - 0.05f) * 100;
                var circleInterest = (depositAmount * depositRate * depositDurationDays) / (100 * 365);
                depositInterest += circleInterest;

                UpdateUI();
                Debug.Log($"Проценты круга: +{circleInterest:F2}");
            }
        }
    }

    private IEnumerator DepositTimer()
    {
        while (DateTime.Now < depositEndTime)
        {
            UpdateUI();
            yield return new WaitForSeconds(1f); 
        }

        CompleteDeposit();
    }

    private void CompleteDeposit()
    {
        playerBalance += depositAmount + depositInterest;

        Debug.Log($"Вклад завершен! Зачислено: {depositAmount:F2} + проценты: {depositInterest:F2}");

        depositAmount = 0;
        depositInterest = 0;

        if (interestCoroutine != null) StopCoroutine(interestCoroutine);
        if (timerCoroutine != null) StopCoroutine(timerCoroutine);

        UpdateUI();
    }
}