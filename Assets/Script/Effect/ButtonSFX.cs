using UnityEngine;
using UnityEngine.UI;

public class ButtonSFX : MonoBehaviour
{
    public static ButtonSFX Instance { get; private set; }
    public AudioClip buttonClickSFX;

    private void Awake()
    {
        var btn = GetComponent<Button>();
        if (btn != null)
            btn.onClick.AddListener(PlayBtnSFX);

        if (Instance == null)
            Instance = this;
    }

    public void PlayBtnSFX()
    {
        if (buttonClickSFX != null && AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(buttonClickSFX);
    }
}
