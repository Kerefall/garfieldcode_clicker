using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using System.Globalization;

public class Clicker : MonoBehaviour
{
    public static Clicker Instance;

    private float Money // Total user's money
    {
        get => PlayerPrefs.GetFloat("Money", 0);
        set => PlayerPrefs.SetFloat("Money", value);
    }

    private int ClickGain 
    {
        get => PlayerPrefs.GetInt("ClickGain", 1);
        set => PlayerPrefs.SetInt("ClickGain", value);
    }

    private float PassiveProfit // Passive money
    {
        get => PlayerPrefs.GetFloat("PassiveProfit", 1);
        set => PlayerPrefs.SetFloat("PassiveProfit", value);
    }


    [SerializeField]
    private TextMeshProUGUI moneyText;

    public void Awake() // For using methods of this class in other files
    {
        Instance = this;
    }

    public void Start()
    {
        Update();
        CalculateOfflineMoney();
    }

    public void Click()
    {
        Money += ClickGain;
        EffectController.Instance.SetClickEffect(ClickGain);
        Update();
    }

    public void Update()
    {
        moneyText.text = FormatNumber(Money);
    }

    private void OnApplicationQuit()
        => PlayerPrefs.SetString("LastPlayedTime", DateTime.UtcNow.ToString());

    private void CalculateOfflineMoney()
    {
        var lastPlayedTimeString = PlayerPrefs.GetString("LastPlayedTime", null);

        if (lastPlayedTimeString == null)
            return;

        var lastPlayedTime = DateTime.Parse(lastPlayedTimeString);
        var maxTime = 6 * 60 * 60; // Maximum time kogda player offline
        var offlineTime = ((float)(DateTime.UtcNow - lastPlayedTime).TotalSeconds);

        offlineTime = (offlineTime > maxTime) ? maxTime : offlineTime;

        var offlineMoney = offlineTime * PassiveProfit;
        Money += offlineMoney;
        Debug.Log($"Offine time : {offlineTime}, offline money: {offlineMoney}");
    }

    private string FormatNumber(float number)
    {
        CultureInfo culture = new CultureInfo("en-US");
        return number.ToString("N2", culture);
    }

}
