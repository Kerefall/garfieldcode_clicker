using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Clicker : MonoBehaviour
{
    public static Clicker Instance;

    public float Money // Total user's money
    {
        get => PlayerPrefs.GetFloat("Money", 1);
        set => PlayerPrefs.SetFloat("Money", value);
    }

    private int ClickGain 
    {
        get => PlayerPrefs.GetInt("ClickGain", 1);
        set => PlayerPrefs.SetInt("ClickGain", value);
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
    }

    public void Click()
    {
        Money += 1 * ClickGain;
        EffectController.Instance.SetClickEffect(1 * ClickGain);
        Update();
    }

    public void Update()
    {
        moneyText.text = "$" + Money;
    }

}
