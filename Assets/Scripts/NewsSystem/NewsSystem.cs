using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;

public class MailSystem : MonoBehaviour
{
    public GameObject mailTab;
    public Button activateSystemButton;
    public GameObject mailMessagePrefab;
    public Transform messagesContainer;

    private float educationTimer = 0f; // таймер оплаты обучения
    private float homeTimer = 0f; // таймер оплаты общаги
    private float scamTimer = 0f; // таймер скама

    private int educationAmount = 10000; // стоимость обучения
    private int homeAmount = 1000; // стоимомость общаги

    private float playerMoney;

    public int isSystemActive
    {
        get => PlayerPrefs.GetInt("IsNewsSystemActive", 0);
        private set => PlayerPrefs.SetInt("IsNewsSystemActive", value);
    }

    private void Start()
    {
        activateSystemButton.onClick.AddListener(ActivateSystem);
        playerMoney = Clicker.Instance.Money;
    }

    private void Update()
    {
        if (isSystemActive == 0) return;
        else ActivateSystem();

            educationTimer += Time.deltaTime;
        homeTimer += Time.deltaTime;
        scamTimer += Time.deltaTime;

        // чек таймеров
        if (educationTimer >= 10f) // 7 мин
        {
            educationTimer = 0f;
            TriggerEducationEvent();
        }

        if (homeTimer >= 300f) // 5 мин
        {
            homeTimer = 0f;
            TriggerHomeEvent();
        }

        if (scamTimer >= 300f) // 5 мин
        {
            scamTimer = 0f;
            TriggerRandomScamEvent();
        }
    }

    private void ActivateSystem()
    {
        isSystemActive = 1;
        activateSystemButton.gameObject.SetActive(false);
    }

    private void TriggerEducationEvent()
    {
        float chance = Random.Range(0f, 1f);

        if (chance <= 0.6f)
        {
            // норм платеж 60%
            CreateEducationMessage(
                "Оплатите обучение (fin.pay.РФ): сумма " + educationAmount + " рублей",
                true,
                "опробуйте инструмент Кредит, если не хватает",
                "в следующий раз сумма будет выше"
            );
        }
        else
        {
            // скам 40%
            CreateEducationMessage(
                "Оплатите обучение (f1n.pay.net): сумма " + educationAmount + " рублей",
                false,
                "",
                "в следующий раз сумма будет выше"
            );
        }
    }

    private void CreateEducationMessage(string messageText, bool checkMoney, string payNote, string ignoreNote)
    {
        GameObject messageObj = Instantiate(mailMessagePrefab, messagesContainer);
        MailMessage message = messageObj.GetComponent<MailMessage>();

        message.SetMessage(messageText);

        Button payButton = message.AddButton("Оплатить", () => {
            if (checkMoney)
            {
                if (playerMoney >= educationAmount)
                {
                    playerMoney -= educationAmount;
                    Destroy(messageObj);
                }
            }
            else
            {
                // скамнулись мамонты
                playerMoney = 0f;
                message.ShowAlert("Вы стали жертвой мошенников и лишились средств, обращайте внимание на ссылки!");
            }
        });

        if (checkMoney)
        {
            payButton.interactable = playerMoney >= educationAmount;
            message.AddNoteToButton(payButton, payNote);
        }

        Button ignoreButton = message.AddButton("Игнорировать", () => {
            if (checkMoney)
            {
                educationAmount += 500;
                Destroy(messageObj);
            }
            else
                message.ShowAlert("Вы молодец, это были мошенники!");
        });

        message.AddNoteToButton(ignoreButton, ignoreNote);
    }

    private void TriggerHomeEvent()
    {
        float chance = Random.Range(0f, 1f);

        if (chance <= 0.6f)
        {
            // норм платеж 60%
            CreateHomeMessage(
                "Оплатите общежитие (fin.pay.RU): сумма " + homeAmount + " рублей",
                true,
                "опробуйте инструмент Кредит, если не хватает",
                "в следующий раз сумма будет выше"
            );
        }
        else
        {
            // скам 40%
            CreateHomeMessage(
                "Оплатите общежитие (f1n.pay.net): сумма " + homeAmount + " рублей",
                false,
                "",
                "в следующий раз сумма будет выше"
            );
        }
    }

    private void CreateHomeMessage(string messageText, bool checkMoney, string payNote, string ignoreNote)
    {
        // Аналогично CreateTuitionMessage, но с dormitoryAmount и +100 при игноре
    }

    private void TriggerRandomScamEvent()
    {
        int randomEvent = Random.Range(0, 3);

        switch (randomEvent)
        {
            case 0:
                CreateWalletHackMessage();
                break;
            case 1:
                CreateMomScamMessage();
                break;
            case 2:
                CreateUniversityTestMessage();
                break;
        }
    }

    private void CreateWalletHackMessage()
    {
        GameObject messageObj = Instantiate(mailMessagePrefab, messagesContainer);
        MailMessage message = messageObj.GetComponent<MailMessage>();

        message.SetMessage("Ваш кошелёк пытаются взломать, срочно введите данные с вашей карты и мы убережём ваши средства");

        message.AddButton("Ввести данные", () => {
            playerMoney = 0f;
            message.ShowAlert("Вы стали жертвой мошенников и лишились средств, банки не требуют данных карты!");
        });

        message.AddButton("Игнорировать", () => {
            message.ShowAlert("Вы молодец, это были мошенники!");
        });
    }

    private void CreateMomScamMessage()
    {
        GameObject messageObj = Instantiate(mailMessagePrefab, messagesContainer);
        MailMessage message = messageObj.GetComponent<MailMessage>();

        message.SetMessage("Привет, сынок, скинь, пожалуйста, 10 тысяч по этому номеру, срочно нужно, мы тебе потом обязательно вернём. Люблю тебя!");

        message.AddButton("Перевести деньги", () => {
            if (playerMoney >= 10000)
            {
                playerMoney -= 10000f;
                message.ShowAlert("Вы стали жертвой мошенников и лишились средств, лучше уточняйте такое по видеозвонку!");
            }
        });

        message.AddButton("Позвонить маме и уточнить", () => {
            message.ShowAlert("Вы молодец, это были мошенники!");
        });
    }

    private void CreateUniversityTestMessage()
    {
        GameObject messageObj = Instantiate(mailMessagePrefab, messagesContainer);
        MailMessage message = messageObj.GetComponent<MailMessage>();

        message.SetMessage("Здравствуйте! Вам пишет администрация университета. Вам нужно пройти обязательное государственное тестирование по ссылке http://edu1est.net/\nДля прохождения авторизируйтесь через госуслуги");

        message.AddButton("Перейти по ссылке", () => {
            playerMoney = 0f;
            message.ShowAlert("Вы стали жертвой мошенников и лишились средств, обращайте внимание на ссылки!");
        });

        message.AddButton("Уточнить у классного руководителя", () => {
            message.ShowAlert("Вы молодец, это были мошенники!");
        });
    }
}