using UnityEngine;
using UnityEngine.EventSystems;

public class investition : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private GameObject investitionPanel;

    public void OnPointerClick(PointerEventData eventData)
    {
        investitionPanel.SetActive(!investitionPanel.activeSelf);
    }
}