using UnityEngine;

public class ButtonSFX : MonoBehaviour
{
    public AudioClip buttonClickSFX;

    public void PlayBtnSFX()
    {
        AudioManager.Instance.PlaySFX(buttonClickSFX);
    }
}
