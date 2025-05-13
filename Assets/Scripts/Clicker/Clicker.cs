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