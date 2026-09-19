using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private GameObject shopButton;

    /// <summary>Đăng ký nút mở shop 1 lần trong prefab GameUIRoot (cùng prefab nên gán được).
    /// Các trigger ngoài scene gọi hàm này thay vì giữ ref trực tiếp.</summary>
    public void SetShopButtonActive(bool isActive)
    {
        if (shopButton != null)
            shopButton.SetActive(isActive);
    }

    public void PlayBtnSfx()
    {
        ButtonSFX.Instance.PlayBtnSFX();
    }

    public void LoadNextScene() 
    {
        GameSceneController.Instance.LoadNextScene();
    }
    public void RestartCurrentScene()
    {
        GameSceneController.Instance.RestartCurrentScene();
    }

    public void LoadMainMenu()
    {
        GameSceneController.Instance.LoadMainMenu();
    }

}
