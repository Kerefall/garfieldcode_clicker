using UnityEngine;
using UnityEngine.UI;

public class Clicker : MonoBehaviour
{
    public static Clicker Instance;

    private float Money
    {
        get => PlayerPrefs.GetFloat("Money", 0);
        set => PlayerPrefs.SetFloat("Money", value);
    }

    private int ClickGain
    {
        get => PlayerPrefs.GetInt("ClickGain", 0);
        set => PlayerPrefs.SetInt("ClickGain", value);
    }

    [SerializeField] private Text moneyText;


    private void Start()
    {
        UpdateUI();
    }

    public void ClickedCoin()
    {
        Money += ClickGain;
        UpdateUI();
    }

    private void UpdateUI()
    {
        moneyText.text = (int)Money + "$";
    }
}
