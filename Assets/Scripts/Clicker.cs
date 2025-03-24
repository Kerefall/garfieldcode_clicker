using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

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
        moneyText.text = "$" + FormatNumber(Money);
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetString("LastPlayedTime", DateTime.UtcNow.ToString());
    }

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

        // Define a set of suffixes
        string[] suffixes = { "K", "M", "B", "T" };
        int suffixIndex = 0;

        // As long as the number is greater than 1000, divide it by 1000 and increase the suffix index
        while (number >= 1000 && suffixIndex < suffixes.Length)
        {
            number /= 1000;
            suffixIndex++;
        }

        // format number and add suffix
        return $"{number:0.0}{suffixes[suffixIndex - 1]}";
    }

}
