using UnityEngine;
using UnityEngine.UI;

public class MusicToggleWithIcon : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite iconOn;  // Иконка при включенном звуке
    public Sprite iconOff; // Иконка при выключенном звуке

    [Header("References")]
    public Image toggleImage; // Image внутри Checkmark
    public Toggle toggle;
    public AudioSource audioSource;

    private void Start()
    {
        // Назначаем обработчик события
        toggle.onValueChanged.AddListener(OnToggleChanged);

        // Инициализируем иконку
        UpdateToggleIcon(toggle.isOn);
    }

    private void OnToggleChanged(bool isOn)
    {
        // Меняем иконку
        UpdateToggleIcon(isOn);

        // Включаем/выключаем звук
        if (audioSource != null)
        {
            audioSource.volume = isOn ? 1f : 0f;
        }
    }

    private void UpdateToggleIcon(bool isOn)
    {
        if (toggleImage != null)
        {
            toggleImage.sprite = isOn ? iconOn : iconOff;
        }
    }
}