using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Mixer")]
    public AudioMixer audioMixer;

    public float MusicVolume
    {
        get
        {
            audioMixer.GetFloat("MusicVolume", out float value);
            return Mathf.Pow(10, value / 20); // Convert dB to [0,1]
        }
    }

    public float SFXVolume
    {
        get
        {
            audioMixer.GetFloat("SFXVolume", out float value);
            return Mathf.Pow(10, value / 20);
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetMusicVolume(float value)
    {
        // value: 0~1, convert to dB
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20);
    }

    public void SetSFXVolume(float value)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20);
    }

    public AudioSource audioSFX;

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null || audioSFX == null) return;
        audioSFX.clip = clip;
        audioSFX.Play();
    }
}
