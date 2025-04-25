using UnityEngine;
using UnityEngine.EventSystems;

public class Bank : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private GameObject learningPanel;

    public void OnPointerClick(PointerEventData eventData)
    {
        learningPanel.SetActive(!learningPanel.activeSelf);
    }
}
