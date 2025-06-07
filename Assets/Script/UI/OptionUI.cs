using UnityEngine.UI;
using UnityEngine.Audio;

public class OptionUI : MenuUI
{
    public Slider musicSlider;
    public Slider sfxSlider;

    private void Start()
    {
        // Gán giá trị ban đầu cho slider (nếu có AudioManager)
        musicSlider.value = AudioManager.Instance.MusicVolume;
        sfxSlider.value = AudioManager.Instance.SFXVolume;

        // Lắng nghe sự kiện thay đổi
        musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
    }

    private void OnMusicVolumeChanged(float value)
    {
        AudioManager.Instance.SetMusicVolume(value);
    }

    private void OnSFXVolumeChanged(float value)
    {
        AudioManager.Instance.SetSFXVolume(value);
    }
}

