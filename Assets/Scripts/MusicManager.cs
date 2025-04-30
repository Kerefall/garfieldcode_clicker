using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    public Toggle toggleMusic;
    public Slider sliderVolumeMusic;
    public AudioSource audio;
    public float volume;
    
    void Start()
    {
        Load();
        ValueMusic();

        toggleMusic = GameObject.FindGameObjectWithTag("Toggle").GetComponent<Toggle>();
        sliderVolumeMusic = GameObject.FindGameObjectWithTag("Slider").GetComponent<Slider>();
    }


    public void SliderMusic()
    {
        volume = sliderVolumeMusic.value;
        Save();
        ValueMusic();
    }

    public void ToggleMusic()
    {
        if (toggleMusic.isOn == true)
        {
            volume = 1;
        } 
        else 
        {
            volume = 0;
        }
        Save();
        ValueMusic();
    }

    private void ValueMusic()
    {
        audio.volume = volume;
        sliderVolumeMusic.value = volume;
        if (volume == 0){toggleMusic.isOn = false;} else {toggleMusic.isOn = true;}
    }

    private void Save()
    {
        PlayerPrefs.SetFloat("volume", volume);
    }

    private void Load()
    {
        volume = PlayerPrefs.GetFloat("volume", volume);
    }
}
