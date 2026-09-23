using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private GameObject shopButton;
    [Tooltip("Root HUD gameplay (Main UI). Tắt khi về menu để HUD cũ khỏi đè menu, bật lại khi vào scene chơi.")]
    [SerializeField] private GameObject mainUI;

    /// <summary>Đăng ký nút mở shop 1 lần trong prefab GameUIRoot (cùng prefab nên gán được).
    /// Các trigger ngoài scene gọi hàm này thay vì giữ ref trực tiếp.</summary>
    public void SetShopButtonActive(bool isActive)
    {
        if (shopButton != null)
            shopButton.SetActive(isActive);
    }

    public void SetGameplayUIActive(bool active)
    {
        if (mainUI != null)
            mainUI.SetActive(active);
    }

    /// <summary>Gọi khi vào MainMenuScene: tắt HUD, đóng panel gameplay
    /// đang mở dở, gỡ pause kẹt. Chỉ ẩn object cấp panel (không đụng cell/
    /// slot lẻ để lần mở sau không bị trống).</summary>
    public void ResetForMainMenu()
    {
        SetShopButtonActive(false);
        SetGameplayUIActive(false);
        Time.timeScale = 1f;

        HidePanel<InventoryUI>();
        HidePanel<ShopUI>();
        HidePanel<SkillPanelUI>();
        // KHÔNG HidePanel<QuestListUI>/<QuestDetailUI> riêng: chúng nằm trong cụm
        // "Inventory UI" đã bị tắt ở trên. Tắt lẻ từng con làm chúng không tự hồi
        // được (Show chỉ bật object con bên trong), trong khi bật/tắt ở cấp cụm
        // cha thì TabController + OnEnable/Refresh tự hồi phục đầy đủ.
        HidePanel<CharacterStatsUI>();
        HidePanel<DialogueUI>();
        HidePanel<GameOverUI>();
        HidePanel<VictoryUI>();
        HidePanel<MenuUI>();
        HidePanel<OptionUI>();
        HidePanel<MovementTutorialUI>();
    }

    private void HidePanel<T>() where T : MonoBehaviour
    {
        foreach (T panel in GetComponentsInChildren<T>(true))
        {
            if (panel != null)
                panel.gameObject.SetActive(false);
        }
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
