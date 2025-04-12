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

    [Header("��������� ������")]
    [SerializeField] private float annualRate = 0.10f;
    [SerializeField] private float maxDepositDuration = 365f; // Max time vklada
    [Tooltip("����� ������ �������� ����� � �������")]
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
        currentBalanceText.text = $"������: {playerBalance:F2}";
        depositBalanceText.text = $"���� ������: {depositAmount:F2}";
        interestRateText.text = $"���������� ������: {(annualRate - 0.05f) * 100}% � ����";

        if (depositAmount > 0)
        {
            TimeSpan timeLeft = depositEndTime - DateTime.Now;
            timeLeftText.text = $"������� ��������: {timeLeft.Days}d {timeLeft.Hours}� {timeLeft.Minutes}�";
        }
        else
        {
            timeLeftText.text = "��� �������� �������";
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
            Debug.Log("����� ��� ������ ������ ����� -- ��� �������� �������.");
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
                Debug.Log($"�������� �����: +{circleInterest:F2}");
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

        Debug.Log($"����� ��������! ���������: {depositAmount:F2} + ��������: {depositInterest:F2}");

        depositAmount = 0;
        depositInterest = 0;

        if (interestCoroutine != null) StopCoroutine(interestCoroutine);
        if (timerCoroutine != null) StopCoroutine(timerCoroutine);

        UpdateUI();
    }
}