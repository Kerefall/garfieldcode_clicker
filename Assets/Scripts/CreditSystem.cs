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

    [Header("��������� �������")]
    [SerializeField] private float annualInterestRate = 0.15f; // ������� ���������� ������
    [SerializeField] private float maxCreditDuration = 365f; // ������������ ���� ������� � ����
    [SerializeField] private float minCreditAmount = 100f; // ����������� ����� �������
    [SerializeField] private float penaltyRate = 0.2f; // ����� �� ���������
    [Tooltip("����� ������ �������� ����� � �������")]
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
        currentBalanceText.text = $"������: {playerBalance:F2}";
        creditDebtText.text = $"�������������: {creditAmount + creditInterest:F2}";
        interestRateText.text = $"���������� ������: {annualInterestRate * 100}% �������";

        if (creditAmount > 0)
        {
            TimeSpan timeLeft = creditEndTime - DateTime.Now;
            if (timeLeft.TotalSeconds > 0)
            {
                timeLeftText.text = $"������� ��������: {timeLeft.Days}d {timeLeft.Hours}� {timeLeft.Minutes}�";
                timeLeftText.color = Color.white;
            }
            else
            {
                timeLeftText.text = $"����������! �����: {penaltyRate * 100}%";
                timeLeftText.color = Color.red;
            }
        }
        else
        {
            timeLeftText.text = "��� �������� ��������";
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
                Debug.Log($"����������� ����� �������: {minCreditAmount}");
                return;
            }

            if (creditAmount > 0)
            {
                Debug.Log("� ��� ��� ���� �������� ������!");
                return;
            }

            if (durationDays <= maxCreditDuration)
            {
                creditAmount = amount;
                creditDurationDays = durationDays;
                creditInterest = 0f;
                isOverdue = false;

                playerBalance += amount; // �������� ������ �� ����
                creditEndTime = DateTime.Now.AddDays(durationDays);

                if (interestCoroutine != null) StopCoroutine(interestCoroutine);
                if (timerCoroutine != null) StopCoroutine(timerCoroutine);

                interestCoroutine = StartCoroutine(CalculateInterest());
                timerCoroutine = StartCoroutine(CreditTimer());

                UpdateUI();
                Debug.Log($"������ �������! �����: {amount:F2}, ����: {durationDays} ����");
            }
            else
            {
                Debug.Log($"������������ ���� �������: {maxCreditDuration} ����");
            }
        }
    }

    public void RepayCredit()
    {
        if (creditAmount > 0)
        {
            float totalDebt = creditAmount + creditInterest;

            // ���������, ���� �� ���������
            if (DateTime.Now > creditEndTime)
            {
                totalDebt *= (1 + penaltyRate); // ��������� �����
                Debug.Log("���������! �������� �����.");
            }

            if (playerBalance >= totalDebt)
            {
                playerBalance -= totalDebt;

                Debug.Log($"������ �������! �����: {creditAmount:F2} + ��������: {creditInterest:F2}" +
                          (DateTime.Now > creditEndTime ? $" + �����: {totalDebt - (creditAmount + creditInterest):F2}" : ""));

                creditAmount = 0;
                creditInterest = 0;
                isOverdue = false;

                if (interestCoroutine != null) StopCoroutine(interestCoroutine);
                if (timerCoroutine != null) StopCoroutine(timerCoroutine);

                UpdateUI();
            }
            else
            {
                Debug.Log($"������������ ������� ��� ���������! �����: {totalDebt:F2}, � ���: {playerBalance:F2}");
            }
        }
    }

    private IEnumerator CalculateInterest()
    {
        while (creditAmount > 0)
        {
            yield return new WaitForSeconds(oneCircleTime * 60f);

            // ������������ �������� �� ����
            float dailyRate = annualInterestRate / 365f;
            float circleInterest = creditAmount * dailyRate * (oneCircleTime / 1440f); // 1440 ����� � ���
            creditInterest += circleInterest;

            UpdateUI();
            Debug.Log($"��������� ��������: +{circleInterest:F2}");
        }
    }

    private IEnumerator CreditTimer()
    {
        while (creditAmount > 0)
        {
            UpdateUI();

            // ��������� ���������
            if (DateTime.Now > creditEndTime && !isOverdue)
            {
                isOverdue = true;
                Debug.Log("������ ���������! �������� ����������� ������.");
            }

            yield return new WaitForSeconds(1f);
        }
    }
}