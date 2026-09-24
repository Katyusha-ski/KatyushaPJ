using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Màn hình tri ân sau khi phá đảo: vào hiện lời cảm ơn, mỗi click hiện
/// thêm 1 khối (assets, rồi donation), click cuối về menu.
/// Wipe save + reset tiến trình NGAY KHI VÀO (point of no return), rồi về menu.
/// </summary>
public class CreditsController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMPro.TMP_Text creditsText;

    [Header("Nội dung (sửa trong Inspector nếu đổi)")]
    [TextArea(3, 10)] [SerializeField] private string titleBlock =
        "CẢM ƠN ĐÃ CHƠI KATYUSHA";
    [TextArea(3, 10)] [SerializeField] private string devBlock =
        "PHÁT TRIỂN\nToàn bộ mọi thứ đều được Katyusha a.k.a Nguyễn Minh Châu làm ra hoặc ăn trộm từ đâu đó.";
    [TextArea(3, 10)] [SerializeField] private string donorBlock =
        "ỦNG HỘ / DONATE\nTezzy tìm nhà\nIfeelsoskibidi\nNe Ne\nTrần Hải Bằng\nKiotakhai";

    [Header("Click")]
    [Tooltip("Chặn click lan từ cutscene/usagi trước đó (giây).")]
    [SerializeField] private float inputLockSeconds = 0.5f;

    private int stage;
    private float inputLock;
    private bool finished;

    private void Start()
    {
        WipeProgress();

        if (UIManager.Instance != null)
        {
            UIManager.Instance.SetShopButtonActive(false);
            UIManager.Instance.SetGameplayUIActive(false);
        }

        inputLock = inputLockSeconds;
        ShowStage(0);
    }

    private void Update()
    {
        if (finished) return;

        if (inputLock > 0f)
        {
            inputLock -= Time.unscaledDeltaTime;
            return;
        }

        if (Input.anyKeyDown || Input.GetMouseButtonDown(0))
            Advance();
    }

    private void Advance()
    {
        stage++;
        if (stage >= 3)
            Finish();
        else
            ShowStage(stage);
    }

    private void ShowStage(int index)
    {
        if (creditsText == null) return;

        creditsText.text = index == 0 ? titleBlock : index == 1 ? devBlock : donorBlock;
        creditsText.rectTransform.anchoredPosition = Vector2.zero;
        creditsText.ForceMeshUpdate();
    }

    private void Finish()
    {
        if (finished) return;
        finished = true;
        SceneManager.LoadScene("MainMenuScene");
    }

    private static void WipeProgress()
    {
        SaveManager.DeleteSave();
        if (ChapterManager.Instance != null)
            ChapterManager.Instance.SetChapter(1);
        SceneStateTracker.ClearAllStates();
        if (Inventory.Instance != null)
            Inventory.Instance.ClearInventory();
        if (GameManager.Instance != null && GameManager.Instance.UsagiShopChest != null)
            GameManager.Instance.UsagiShopChest.Clear();
    }
}
