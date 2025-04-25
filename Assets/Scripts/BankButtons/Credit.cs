using UnityEngine;
using UnityEngine.EventSystems;

public class credit : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private GameObject creditPanel;

    public void OnPointerClick(PointerEventData eventData)
    {
        creditPanel.SetActive(!creditPanel.activeSelf);
    }
}