using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    [Header("References")]
    public Toggle toggleMusic;
    public Slider sliderVolumeMusic;
    public AudioSource audioSource; 

    private float volume;

    private void Start()
    {
        // Загружаем сохранённые настройки
        Load();

        // Настраиваем начальные значения UI
        sliderVolumeMusic.value = volume;
        toggleMusic.isOn = volume > 0;

        // Подписываемся на изменения UI
        sliderVolumeMusic.onValueChanged.AddListener(OnVolumeChanged);
        toggleMusic.onValueChanged.AddListener(OnToggleChanged);

        // Применяем настройки звука
        UpdateAudioVolume();
    }

    private void OnVolumeChanged(float newVolume)
    {
        volume = newVolume;
        toggleMusic.isOn = volume > 0; // Обновляем Toggle в зависимости от громкости
        Save();
        UpdateAudioVolume();
    }

    private void OnToggleChanged(bool isOn)
    {
        volume = isOn ? sliderVolumeMusic.value : 0; // Если Toggle выключен, звук отключается
        Save();
        UpdateAudioVolume();
    }

    private void UpdateAudioVolume()
    {
        if (audioSource != null)
        {
            audioSource.volume = volume;
        }
    }

    private void Save()
    {
        PlayerPrefs.SetFloat("volume", volume);
        PlayerPrefs.Save(); // Важно сохранять изменения!
    }

    private void Load()
    {
        volume = PlayerPrefs.GetFloat("volume", 0.5f); // Значение по умолчанию — 50%
    }
}