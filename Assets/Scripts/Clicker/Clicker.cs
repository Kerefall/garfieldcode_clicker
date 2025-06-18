using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;

public class Clicker : MonoBehaviour
{
    public static Clicker Instance { get; private set; }

    public float Money
    {
        get => PlayerPrefs.GetFloat("Money", 0);
        set
        {
            PlayerPrefs.SetFloat("Money", value);
            UpdateUI();
        }
    }

    public int ClickGain
    {
        get => PlayerPrefs.GetInt("ClickGain", 1);
        set
        {
            PlayerPrefs.SetInt("ClickGain", value);
            UpdateUI();
        }
    }

    public float PassiveProfit
    {
        get => PlayerPrefs.GetFloat("PassiveProfit", 0);
        set => PlayerPrefs.SetFloat("PassiveProfit", value);
    }

    [SerializeField] private TextMeshProUGUI moneyText;
    public float annualRate = 0.1f;

    private bool _nextIsPositive = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        CalculateOfflineMoney();
        UpdateUI();
        InvokeRepeating(nameof(ChangeRate), 300f, 300f); // 300 сек = 5 минут
    }

    private void ChangeRate()
    {
        float randomValue = UnityEngine.Random.value; // Random.value даёт 0..1
        double change = randomValue switch
        {
            < 0.5f => 1.0,  // 50%
            < 0.8f => 2.0,  // 30%
            _ => 3.0        // 20%
        };

        annualRate += (float)(_nextIsPositive ? change : -change);
        _nextIsPositive = !_nextIsPositive;

        Debug.Log($"New rate: {annualRate}%");
    }

    [ContextMenu("Сбросить весь прогресс")]
    public void ResetAllProgress()
    {
        // Сбрасываем основные значения
        PlayerPrefs.DeleteKey("Money");
        PlayerPrefs.DeleteKey("ClickGain");
        PlayerPrefs.DeleteKey("PassiveProfit");
        PlayerPrefs.DeleteKey("LastPlayedTime");

        // Сбрасываем значения в памяти
        Money = 0;
        ClickGain = 1;
        PassiveProfit = 0;

        PlayerPrefs.Save();
        UpdateUI();
        Debug.Log("Весь прогресс сброшен!");
    }

    public void Click()
    {
        Money += ClickGain;
        EffectController.Instance?.SetClickEffect(ClickGain);
    }

    public void UpdateUI()
    {
        if (moneyText != null)
            moneyText.text = FormatNumber(Money);
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetString("LastPlayedTime", DateTime.UtcNow.ToString());
    }

    private void CalculateOfflineMoney()
    {
        var lastPlayedTimeString = PlayerPrefs.GetString("LastPlayedTime", null);
        if (string.IsNullOrEmpty(lastPlayedTimeString)) return;

        if (DateTime.TryParse(lastPlayedTimeString, out var lastPlayedTime))
        {
            var maxTime = 6 * 60 * 60;
            var offlineTime = Mathf.Min((float)(DateTime.UtcNow - lastPlayedTime).TotalSeconds, maxTime);
            var offlineMoney = offlineTime * PassiveProfit;
            Money += offlineMoney;
            Debug.Log($"Offline time: {offlineTime}, offline money: {offlineMoney}");
        }
    }

    private string FormatNumber(float number)
    {
        return number.ToString("N2", CultureInfo.InvariantCulture);
    }
}