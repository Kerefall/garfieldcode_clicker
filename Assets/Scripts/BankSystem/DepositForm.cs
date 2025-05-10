using UnityEngine;
using UnityEngine.UI;

public class DepositForm : MonoBehaviour
{
    public InputField depositAmountInput;
    public Dropdown depositTermDropdown;

    void Start()
    {
        // Настройка Dropdown для выбора срока
        depositTermDropdown.ClearOptions();
        depositTermDropdown.AddOptions(new System.Collections.Generic.List<string>
        {
            "1 месяц", "3 месяца", "6 месяцев", "1 год", "2 года", "3 года", "5 лет"
        });

        // Ограничение ввода только цифр для суммы
        depositAmountInput.contentType = InputField.ContentType.IntegerNumber;
    }

    // Метод для получения данных формы
    public void GetDepositData(out int amount, out int termIndex)
    {
        // Парсим введенную сумму
        int.TryParse(depositAmountInput.text, out amount);

        // Получаем выбранный срок
        termIndex = depositTermDropdown.value;
    }
}