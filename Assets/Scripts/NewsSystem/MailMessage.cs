using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MailMessage : MonoBehaviour
{
    public TextMeshProUGUI messageText;
    public Transform buttonsParent;
    public GameObject buttonPrefab;
    public GameObject alertPopup;
    public TextMeshProUGUI alertText;

    public void SetMessage(string text)
    {
        messageText.text = text;
    }

    public Button AddButton(string buttonText, UnityEngine.Events.UnityAction action)
    {
        GameObject buttonObj = Instantiate(buttonPrefab, buttonsParent);
        Button button = buttonObj.GetComponent<Button>();
        TextMeshProUGUI text = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

        text.text = buttonText;
        button.onClick.AddListener(action);


        LayoutRebuilder.ForceRebuildLayoutImmediate(buttonsParent.GetComponent<RectTransform>());
        return button;
    }

    public void AddNoteToButton(Button button, string noteText)
    {
        if (string.IsNullOrEmpty(noteText)) return;

        GameObject noteObj = new GameObject("Note");
        noteObj.transform.SetParent(button.transform);
        TextMeshProUGUI noteTextComponent = noteObj.AddComponent<TextMeshProUGUI>();

        noteTextComponent.text = noteText;
        noteTextComponent.fontSize = 10;
        noteTextComponent.color = Color.gray;

        // TODO: RectTransform для позиционирования заметки
    }

    public void ShowAlert(string message)
    {
        alertText.text = message;
        alertPopup.SetActive(true);
    }
}