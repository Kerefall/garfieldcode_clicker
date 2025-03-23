using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;
using System;

public class Game : MonoBehaviour
{

    [SerializeField] public GameObject animationMoneyTextPrefab;

    [SerializeField] private int money = 0;
    [SerializeField] private int clickGain = 1;
    [SerializeField] private int upgradeClickCost = 25;
    [SerializeField] private int upgradeClickLvl = 1;
    
    [SerializeField] private int autoClickValue = 0;
    [SerializeField] private int autoClickerLvl = 0;
    [SerializeField] private const int autoClickerStartCost = 75;
    [SerializeField] private int autoClickerCost = autoClickerStartCost;
    [SerializeField] private float autoClickRate = 1f;

    [SerializeField] public Text moneyText;
    [SerializeField] public Text autoClickerText;
    [SerializeField] public Text upgradeText;

    [SerializeField] public GameObject shopPanel;
    [SerializeField] public GameObject settingsPanel;
    [SerializeField] public GameObject miniGamesPanel;



    // ���������� ����� ���� +1 +1 +5 +5 
    public void EffectsToMoneyGain(int value)
    {
        
        GameObject animationMoneyTextObject = Instantiate(animationMoneyTextPrefab, transform.parent);
        Text animationMoneyText = animationMoneyTextObject.GetComponent<Text>();
        animationMoneyText.text = "+" + value;
        StartCoroutine(MoveAndDelete(animationMoneyTextObject));
    }

    private IEnumerator MoveAndDelete(GameObject obj, float durationTime = 1f)
    {
        RectTransform rectTransform = obj.GetComponent<RectTransform>();
        Vector2 randomDirection = UnityEngine.Random.insideUnitCircle.normalized * 490f;
        float elapsedTime = 0f;
        while (elapsedTime < durationTime)
        {
            rectTransform.anchoredPosition += randomDirection * Time.deltaTime;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Destroy(obj);
    }

    // �������� ��� ����� 
    public void Click()
    {
        money += clickGain ;
        StartCoroutine(ButtonAnimation());
        EffectsToMoneyGain(clickGain);
        UpdateMoneyText();
    }


    // �������� ��� ����� �� �������
    private IEnumerator ButtonAnimation()
    {
        Vector3 originalScale = transform.localScale;
        transform.localScale = originalScale * 0.9f;
        yield return new WaitForSeconds(0.1f);
        transform.localScale = originalScale;

    }
    
    // ��������� �����
    public void UpgradeClick()
    {
        if (upgradeClickLvl <= 10)
        {
            if (money >= upgradeClickCost)
            {
                money -= upgradeClickCost;
                clickGain++;
                upgradeClickLvl++;
                UpdateMoneyText();
                upgradeClickCost += 15;
            }
            else
            {
                Debug.Log("������������ �������");
            }
        }
        else
        {
            if (money >= upgradeClickCost)
            {
                money -= upgradeClickCost;
                clickGain += 3;
                upgradeClickCost *= 2;
                upgradeClickLvl++;
                UpdateMoneyText();
            }
            else
            {
                Debug.Log("������������ �������");
            }
        }
    }

    // ��������� �����
    public void BuyAutoClicker()
    {
        if (money >= autoClickerCost)
        {
            autoClickerLvl++;
            autoClickValue += 1;
            autoClickerCost = Mathf.RoundToInt(autoClickerStartCost * Mathf.Pow(1.65f, autoClickerLvl));
            UpdateAutoClickerText();
            UpdateMoneyText();
        }
    }
    public void UpdateAutoClickerText()
    {
       // ���� ����� �������� ������� �����������
    }


    // ��������� ����� ��� 
    private IEnumerator AutoGenerateMoney()
    {
        while (true)
        {
            if (autoClickValue > 0)
            {
                money += autoClickValue;
                EffectsToMoneyGain(autoClickValue);
                UpdateMoneyText();
            }
            yield return new WaitForSeconds(autoClickRate);
        }
    }

    private void OnAplicationQuit() 
        => PlayerPrefs.SetString("LastPlayedTime", DateTime.UtcNow.ToString());

    private void CalculateOfflineIncome()
    {
        var lastPlayedTime = DateTime.Parse(PlayerPrefs.GetString("LastPlayedTime"), null);

        if (lastPlayedTime == null) return;

        var maxTimeOut = 6 * 60 * 60;
        var secondsOut = (DateTime.UtcNow - lastPlayedTime).TotalSeconds;

        secondsOut = (secondsOut > maxTimeOut) ? maxTimeOut : secondsOut; // condition ? consequent : alternative 

        money += (int)secondsOut * autoClickValue;
        Debug.Log($"Time out: {secondsOut} outMoney: {(int)secondsOut * autoClickValue}");
    }

    private void UpdateMoneyText()
    {
        moneyText.text = money.ToString() + "$";
    }
    private void UpdateUpgradeText()
    {
        upgradeText.text = "�������� ($" + upgradeClickCost + ") | �������: " + upgradeClickLvl;
    }
    
    public void Settings()
    {
        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }

    public void UpgradeShop()
    {
        shopPanel.SetActive(!shopPanel.activeSelf);
    }

    public void MinigamesMenu()
    {
        miniGamesPanel.SetActive(!miniGamesPanel.activeSelf);
    }
    
   
}
