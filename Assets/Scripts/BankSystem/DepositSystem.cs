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

    [Header("��������� ������")]
    [SerializeField] private float annualRate = 0.10f;
    [SerializeField] private float maxDepositDuration = 365f; // Max time vklada
    [Tooltip("����� ������ �������� ����� � �������")]
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
            "1 �����", "3 ������", "6 �������", "1 ���", "2 ����", "3 ����", "5 ���"
        });
    }
     
    private void UpdateUI()
    {
        currentBalanceText.text = $"������: {Clicker.Instance.Money:F2}";
        depositBalanceText.text = $"{depositAmount:F2}";
        interestRateText.text = $"{(annualRate - 0.05f) * 100}%";

        if (depositAmount > 0)
        {
            var timeLeft = depositDurationDays - depositDays;
            timeLeftText.text = $"������� ��������: {timeLeft}d ";
        }
        else
        {
            timeLeftText.text = "��� �������� �������";
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
            Debug.Log("����� ��� ������ ������ ����� -- ��� �������� �������.");
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
                Debug.Log($"�������� �����: +{circleInterest:F2}");
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

        Debug.Log($"����� ��������! ���������: {depositAmount:F2} + ��������: {depositInterest:F2}");

        depositAmount = 0;
        depositInterest = 0;

        if (interestCoroutine != null) StopCoroutine(interestCoroutine);
        if (timerCoroutine != null) StopCoroutine(timerCoroutine);

        UpdateUI();
    }
}