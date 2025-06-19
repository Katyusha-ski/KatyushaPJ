using UnityEngine;

public class ButtonSFX : MonoBehaviour
{
    public static ButtonSFX Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public AudioClip buttonClickSFX;

    public void PlayBtnSFX()
    {
        AudioManager.Instance.PlaySFX(buttonClickSFX);
    }
}
