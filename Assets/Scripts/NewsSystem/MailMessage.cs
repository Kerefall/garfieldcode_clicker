using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class MailMessage : MonoBehaviour
{
    [Header("Alert Settings")]
    public GameObject alertPrefab; // Перетащите сюда префаб Alert'а
    public Transform alertParent; // Обычно Main Canvas

    private GameObject currentAlert;

    public TextMeshProUGUI messageText;
    public Button payButton;
    public Button ignoreButton;
    //public TextMeshProUGUI payButtonNote;
    //public TextMeshProUGUI ignoreButtonNote;

    public void SetMessage(string text)
    {
        messageText.text = text;
    }

    public void SetupButtons(
        UnityEngine.Events.UnityAction payAction,
        UnityEngine.Events.UnityAction ignoreAction,
        string payNote = "",
        string ignoreNote = "")
    {
        // Настройка кнопки "Оплатить"
        payButton.onClick.RemoveAllListeners();
        payButton.onClick.AddListener(payAction);

        // Настройка кнопки "Игнорировать"
        ignoreButton.onClick.RemoveAllListeners();
        ignoreButton.onClick.AddListener(ignoreAction);
    }


    public void SetPayButtonInteractable(bool interactable)
    {
        payButton.interactable = interactable;
    }



    public void ShowAlert(string message, bool closeMessage = false)
    {
        // Удаляем предыдущий Alert, если есть
        if (currentAlert != null)
        {
            Destroy(currentAlert);
        }

        // Создаем новый Alert
        currentAlert = Instantiate(alertPrefab, alertParent);
        currentAlert.SetActive(true);

        // Настраиваем текст
        TextMeshProUGUI alertText = currentAlert.GetComponentInChildren<TextMeshProUGUI>();
        if (alertText != null)
        {
            alertText.text = message;
        }

        // Настраиваем кнопку
        Button okButton = currentAlert.GetComponentInChildren<Button>();
        if (okButton != null)
        {
            okButton.onClick.AddListener(() => {
                Destroy(currentAlert);
                if (closeMessage)
                {
                    Destroy(gameObject); // Уничтожаем текущее письмо
                }
            });
        }
    }
}
