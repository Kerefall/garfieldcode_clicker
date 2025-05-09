using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using TMPro;

public class CreditSystem : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject creditPanel;
    public InputField creditAmountInput;
    public InputField creditDurationInput;
    public TextMeshProUGUI currentBalanceText;
    public TextMeshProUGUI creditDebtText;
    public TextMeshProUGUI interestRateText;
    public TextMeshProUGUI timeLeftText;
    public Button takeCreditButton;
    public Button repayCreditButton;

    [Header("Настройки кредита")]
    [SerializeField] private float annualInterestRate = 0.15f; // Годовая процентная ставка
    [SerializeField] private float maxCreditDuration = 365f; // Максимальный срок кредита в днях
    [SerializeField] private float minCreditAmount = 100f; // Минимальная сумма кредита
    [SerializeField] private float penaltyRate = 0.2f; // Штраф за просрочку
    [Tooltip("Время одного игрового круга в минутах")]
    [SerializeField] private float oneCircleTime = 5f;

    private float playerBalance = 1000f;
    private float creditAmount = 0f;
    private float creditInterest = 0f;
    private float creditDurationDays = 0f;
    private DateTime creditEndTime;
    private bool isOverdue = false;

    private Coroutine interestCoroutine;
    private Coroutine timerCoroutine;

    void Start()
    {
        UpdateUI();
        takeCreditButton.onClick.AddListener(TakeCredit);
        repayCreditButton.onClick.AddListener(RepayCredit);
    }

    private void UpdateUI()
    {
        currentBalanceText.text = $"Баланс: {playerBalance:F2}";
        creditDebtText.text = $"Задолженность: {creditAmount + creditInterest:F2}";
        interestRateText.text = $"Процентная ставка: {annualInterestRate * 100}% годовых";

        if (creditAmount > 0)
        {
            TimeSpan timeLeft = creditEndTime - DateTime.Now;
            if (timeLeft.TotalSeconds > 0)
            {
                timeLeftText.text = $"Времени осталось: {timeLeft.Days}d {timeLeft.Hours}ч {timeLeft.Minutes}м";
                timeLeftText.color = Color.white;
            }
            else
            {
                timeLeftText.text = $"Просрочено! Штраф: {penaltyRate * 100}%";
                timeLeftText.color = Color.red;
            }
        }
        else
        {
            timeLeftText.text = "Нет активных кредитов";
            timeLeftText.color = Color.white;
        }
    }

    public void TakeCredit()
    {
        if (float.TryParse(creditAmountInput.text, out float amount) &&
            float.TryParse(creditDurationInput.text, out float durationDays))
        {
            if (amount < minCreditAmount || durationDays <= 0)
            {
                Debug.Log($"Минимальная сумма кредита: {minCreditAmount}");
                return;
            }

            if (creditAmount > 0)
            {
                Debug.Log("У вас уже есть активный кредит!");
                return;
            }

            if (durationDays <= maxCreditDuration)
            {
                creditAmount = amount;
                creditDurationDays = durationDays;
                creditInterest = 0f;
                isOverdue = false;

                playerBalance += amount; // Получаем деньги на счет
                creditEndTime = DateTime.Now.AddDays(durationDays);

                if (interestCoroutine != null) StopCoroutine(interestCoroutine);
                if (timerCoroutine != null) StopCoroutine(timerCoroutine);

                interestCoroutine = StartCoroutine(CalculateInterest());
                timerCoroutine = StartCoroutine(CreditTimer());

                UpdateUI();
                Debug.Log($"Кредит получен! Сумма: {amount:F2}, срок: {durationDays} дней");
            }
            else
            {
                Debug.Log($"Максимальный срок кредита: {maxCreditDuration} дней");
            }
        }
    }

    public void RepayCredit()
    {
        if (creditAmount > 0)
        {
            float totalDebt = creditAmount + creditInterest;

            // Проверяем, есть ли просрочка
            if (DateTime.Now > creditEndTime)
            {
                totalDebt *= (1 + penaltyRate); // Добавляем штраф
                Debug.Log("Просрочка! Начислен штраф.");
            }

            if (playerBalance >= totalDebt)
            {
                playerBalance -= totalDebt;

                Debug.Log($"Кредит погашен! Сумма: {creditAmount:F2} + проценты: {creditInterest:F2}" +
                          (DateTime.Now > creditEndTime ? $" + штраф: {totalDebt - (creditAmount + creditInterest):F2}" : ""));

                creditAmount = 0;
                creditInterest = 0;
                isOverdue = false;

                if (interestCoroutine != null) StopCoroutine(interestCoroutine);
                if (timerCoroutine != null) StopCoroutine(timerCoroutine);

                UpdateUI();
            }
            else
            {
                Debug.Log($"Недостаточно средств для погашения! Нужно: {totalDebt:F2}, у вас: {playerBalance:F2}");
            }
        }
    }

    private IEnumerator CalculateInterest()
    {
        while (creditAmount > 0)
        {
            yield return new WaitForSeconds(oneCircleTime * 60f);

            // Рассчитываем проценты за круг
            float dailyRate = annualInterestRate / 365f;
            float circleInterest = creditAmount * dailyRate * (oneCircleTime / 1440f); // 1440 минут в дне
            creditInterest += circleInterest;

            UpdateUI();
            Debug.Log($"Начислены проценты: +{circleInterest:F2}");
        }
    }

    private IEnumerator CreditTimer()
    {
        while (creditAmount > 0)
        {
            UpdateUI();

            // Проверяем просрочку
            if (DateTime.Now > creditEndTime && !isOverdue)
            {
                isOverdue = true;
                Debug.Log("Кредит просрочен! Начинают начисляться штрафы.");
            }

            yield return new WaitForSeconds(1f);
        }
    }
}