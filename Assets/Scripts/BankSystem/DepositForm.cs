using UnityEngine;
using UnityEngine.UI;

public class DepositForm : MonoBehaviour
{
    public InputField depositAmountInput;
    public Dropdown depositTermDropdown;

    void Start()
    {
        // ��������� Dropdown ��� ������ �����
        depositTermDropdown.ClearOptions();
        depositTermDropdown.AddOptions(new System.Collections.Generic.List<string>
        {
            "1 �����", "3 ������", "6 �������", "1 ���", "2 ����", "3 ����", "5 ���"
        });

        // ����������� ����� ������ ���� ��� �����
        depositAmountInput.contentType = InputField.ContentType.IntegerNumber;
    }

    // ����� ��� ��������� ������ �����
    public void GetDepositData(out int amount, out int termIndex)
    {
        // ������ ��������� �����
        int.TryParse(depositAmountInput.text, out amount);

        // �������� ��������� ����
        termIndex = depositTermDropdown.value;
    }
}