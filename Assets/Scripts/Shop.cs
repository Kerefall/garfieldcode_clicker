using Unity.VisualScripting;
using UnityEngine.EventSystems;
using UnityEngine;

public class Shop : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private GameObject settingsPanel;

    public void OnPointerClick(PointerEventData eventData)
    {
        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }
}
