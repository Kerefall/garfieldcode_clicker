using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MailSystem : MonoBehaviour
{
    [Header("References")]
    public GameObject mailTab;
    public Button activateSystemButton;
    public GameObject mailMessagePrefab;
    public Transform messageDisplayArea;
    public GameObject noMessagesText;

    [Header("Timers")]
    public float educationInterval = 420f; // 7 минут
    public float homeInterval = 300f;     // 5 минут
    public float scamInterval = 300f;     // 5 минут

    [Header("Costs")]
    public int educationAmount = 10000;
    public int homeAmount = 1000;

    private float educationTimer = 0f;
    private float homeTimer = 0f;
    private float scamTimer = 0f;
    private Queue<GameObject> messageQueue = new Queue<GameObject>();
    private GameObject currentMessage;

    private int IsSystemActive
    {
        get => PlayerPrefs.GetInt("IsNewsSystemActive", 0);
        set => PlayerPrefs.SetInt("IsNewsSystemActive", value);
    }

    private void Start()
    {
        activateSystemButton.onClick.AddListener(ActivateSystem);
        noMessagesText.SetActive(false);
    }

    private void Update()
    {
        if (IsSystemActive == 0) return;

        UpdateTimers();
        CheckForNewMessages();
    }

    private void UpdateTimers()
    {
        educationTimer += Time.deltaTime;
        homeTimer += Time.deltaTime;
        scamTimer += Time.deltaTime;
    }

    private void CheckForNewMessages()
    {
        

        if (educationTimer >= educationInterval)
        {
            educationTimer = 0f;
            CreateEducationMessage();
        }

        if (homeTimer >= homeInterval)
        {
            homeTimer = 0f;
            CreateHomeMessage();
        }

        if (scamTimer >= scamInterval)
        {
            scamTimer = 0f;
            CreateRandomScamMessage();
        }

        if (currentMessage == null && messageQueue.Count > 0)
            ShowNextMessage();
    }

    private void ActivateSystem()
    {
        IsSystemActive = 1;
        activateSystemButton.gameObject.SetActive(false);
    }

    public void ShowNextMessage()
    {
        if (currentMessage != null)
        {
            Destroy(currentMessage);
            currentMessage = null;
        }

        if (messageQueue.Count > 0)
        {
            currentMessage = messageQueue.Dequeue();
            currentMessage.SetActive(true);
            noMessagesText.SetActive(false);
        }
        else
        {
            noMessagesText.SetActive(true);
        }
    }

    private void CreateEducationMessage()
    {
        bool isLegit = Random.Range(0f, 1f) <= 0.6f;
        string sender = isLegit ? "fin.pay.РФ" : "f1n.pay.net";

        GameObject messageObj = Instantiate(mailMessagePrefab, messageDisplayArea);
        messageObj.SetActive(false);

        MailMessage message = messageObj.GetComponent<MailMessage>();
        message.SetMessage($"Оплатите обучение ({sender}): сумма {educationAmount} рублей");

        message.SetupButtons(
            payAction: () => HandleEducationPayment(isLegit, message),
            ignoreAction: () => HandleEducationIgnore(isLegit, message)
        );

        if (isLegit)
        {
            message.SetPayButtonInteractable(Clicker.Instance.Money >= educationAmount);
        }

        messageQueue.Enqueue(messageObj);

        if (currentMessage == null && messageQueue.Count > 0)
        {
            ShowNextMessage();
        }
    }

    private void HandleEducationPayment(bool isLegit, MailMessage message)
    {
        if (isLegit)
        {
            if (Clicker.Instance.Money >= educationAmount)
            {
                Clicker.Instance.Money -= educationAmount;
                ShowNextMessage();
            }
        }
        else
        {
            Clicker.Instance.Money = 0f;
            message.ShowAlert("Вы стали жертвой мошенников и лишились средств!", true);
        }
    }

    private void HandleEducationIgnore(bool isLegit, MailMessage message)
    {
        if (isLegit)
        {
            educationAmount += 500;
        }
        message.ShowAlert(isLegit ? "Сумма увеличена!" : "Вы молодец, это были мошенники!", true);
    }

    private void CreateHomeMessage()
    {
        bool isLegit = Random.Range(0f, 1f) <= 0.6f;
        string sender = isLegit ? "fin.pay.RU" : "f1n.pay.net";

        GameObject messageObj = Instantiate(mailMessagePrefab, messageDisplayArea);
        messageObj.SetActive(false);

        MailMessage message = messageObj.GetComponent<MailMessage>();
        message.SetMessage($"Оплатите общежитие ({sender}): сумма {homeAmount} рублей");

        message.SetupButtons(
            payAction: () => HandleHomePayment(isLegit, message),
            ignoreAction: () => HandleHomeIgnore(isLegit, message),
            payNote: isLegit ? "Попробуйте кредит, если не хватает" : "",
            ignoreNote: "В следующий раз сумма будет выше"
        );

        if (isLegit)
        {
            message.SetPayButtonInteractable(Clicker.Instance.Money >= homeAmount);
        }

        messageQueue.Enqueue(messageObj);

        if (currentMessage == null && messageQueue.Count > 0)
        {
            ShowNextMessage();
        }
    }

    private void HandleHomePayment(bool isLegit, MailMessage message)
    {
        if (isLegit)
        {
            if (Clicker.Instance.Money >= homeAmount)
            {
                Clicker.Instance.Money -= homeAmount;
                ShowNextMessage();
            }
        }
        else
        {
            Clicker.Instance.Money = 0f;
            message.ShowAlert("Вы стали жертвой мошенников и лишились средств!", true);
        }
    }

    private void HandleHomeIgnore(bool isLegit, MailMessage message)
    {
        if (isLegit)
        {
            homeAmount += 100;
        }
        message.ShowAlert(isLegit ? "Сумма увеличена!" : "Вы молодец, это были мошенники!", true);
    }

    private void CreateRandomScamMessage()
    {
        int scamType = Random.Range(0, 3);

        switch (scamType)
        {
            case 0: CreateWalletHackMessage(); break;
            case 1: CreateMomScamMessage(); break;
            case 2: CreateUniversityTestMessage(); break;
        }
    }

    private void CreateWalletHackMessage()
    {
        GameObject messageObj = Instantiate(mailMessagePrefab, messageDisplayArea);
        messageObj.SetActive(false);

        MailMessage message = messageObj.GetComponent<MailMessage>();
        message.SetMessage("Ваш кошелёк пытаются взломать, срочно введите данные карты!");

        message.SetupButtons(
            payAction: () => {
                Clicker.Instance.Money = 0f;
                message.ShowAlert("Вы стали жертвой мошенников!", true);
            },
            ignoreAction: () => {
                message.ShowAlert("Вы молодец, это были мошенники!", true);
            }
        );

        messageQueue.Enqueue(messageObj);
    }

    private void CreateMomScamMessage()
    {
        GameObject messageObj = Instantiate(mailMessagePrefab, messageDisplayArea);
        messageObj.SetActive(false);

        MailMessage message = messageObj.GetComponent<MailMessage>();
        message.SetMessage("Привет, сынок, скинь 10 тысяч, срочно нужно!");

        message.SetupButtons(
            payAction: () => {
                if (Clicker.Instance.Money >= 10000)
                {
                    Clicker.Instance.Money -= 10000;
                    message.ShowAlert("Вы стали жертвой мошенников!", true);
                }
            },
            ignoreAction: () => {
                message.ShowAlert("Вы молодец, это были мошенники!", true);
            }
        );

        messageQueue.Enqueue(messageObj);
    }

    private void CreateUniversityTestMessage()
    {
        GameObject messageObj = Instantiate(mailMessagePrefab, messageDisplayArea);
        messageObj.SetActive(false);

        MailMessage message = messageObj.GetComponent<MailMessage>();
        message.SetMessage("Пройти обязательное тестирование по ссылке http://edu1est.net/");

        message.SetupButtons(
            payAction: () => {
                Clicker.Instance.Money = 0f;
                message.ShowAlert("Вы стали жертвой мошенников!", true);
            },
            ignoreAction: () => {
                message.ShowAlert("Вы молодец, это были мошенники!", true);
                ShowNextMessage();
            }
        );

        messageQueue.Enqueue(messageObj);
    }
}