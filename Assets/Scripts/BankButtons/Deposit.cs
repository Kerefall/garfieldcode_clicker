using UnityEngine;
using UnityEngine.EventSystems;

public class deposit : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private GameObject depositPanel;

    public void OnPointerClick(PointerEventData eventData)
    {
        depositPanel.SetActive(!depositPanel.activeSelf);
    }
}