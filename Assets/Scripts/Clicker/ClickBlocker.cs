using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image), typeof(CanvasGroup))]
public class MobileClickBlocker : MonoBehaviour, IPointerClickHandler
{
    [Header("Настройки")]
    [SerializeField] private bool blockClicks = true;
    [SerializeField][Range(0.1f, 1f)] private float minAlphaToBlock = 0.5f;

    private Image image;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        image = GetComponent<Image>();
        canvasGroup = GetComponent<CanvasGroup>();

        // Автонастройка если компоненты не настроены
        if (image.sprite == null)
        {
            image.color = new Color(0, 0, 0, 0.01f); // Почти прозрачный
        }

        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (blockClicks && image.color.a >= minAlphaToBlock)
        {
            // Полная блокировка клика
            eventData.pointerPress = null;
            eventData.pointerEnter = null;

            // Для Android/iOS
#if UNITY_IOS || UNITY_ANDROID
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    eventData.pointerCurrentRaycast = new RaycastResult();
                }
            }
#endif
        }
    }
}