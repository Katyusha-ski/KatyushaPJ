using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Màn hình tri ân sau khi phá đảo: chữ cuộn lên, bấm phím/chuột để bỏ qua.
/// Wipe save + reset tiến trình NGAY KHI VÀO (point of no return), rồi về menu.
/// </summary>
public class CreditsController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMPro.TMP_Text creditsText;

    [Header("Chữ chạy")]
    [SerializeField] private float scrollSpeed = 60f;
    [SerializeField] private float startDelay = 1f;
    [SerializeField] private float endHoldSeconds = 2f;

    [Header("Nội dung (sửa trong Inspector nếu đổi)")]
    [TextArea(3, 10)] [SerializeField] private string titleBlock =
        "CẢM ƠN ĐÃ CHƠI KATYUSHA";
    [TextArea(3, 10)] [SerializeField] private string donorBlock =
        "ỦNG HỘ / DONATE\nTezzy tìm nhà\nIfeelsoskibidi\nNe Ne\nTrần Hải Bằng\nKiotakhai";
    [TextArea(3, 10)] [SerializeField] private string devBlock =
        "PHÁT TRIỂN\nToàn bộ mọi thứ đều được Katyusha a.k.a Nguyễn Minh Châu làm ra hoặc ăn trộm từ đâu đó.";

    private RectTransform textRect;
    private float totalHeight;
    private float startTimer;
    private float endTimer;
    private bool finished;

    private void Start()
    {
        WipeProgress();

        if (creditsText != null)
        {
            creditsText.text = titleBlock + "\n\n\n" + donorBlock + "\n\n\n" + devBlock;
            creditsText.ForceMeshUpdate();
            textRect = creditsText.rectTransform;
            totalHeight = Mathf.Max(creditsText.preferredHeight, Screen.height);
            Vector2 pos = textRect.anchoredPosition;
            pos.y = -Screen.height * 0.5f - totalHeight * 0.5f;
            textRect.anchoredPosition = pos;
        }

        startTimer = startDelay;
    }

    private void Update()
    {
        if (finished) return;

        // Bỏ qua bất cứ lúc nào.
        if (Input.anyKeyDown || Input.GetMouseButtonDown(0))
        {
            Finish();
            return;
        }

        if (startTimer > 0f)
        {
            startTimer -= Time.unscaledDeltaTime;
            return;
        }

        if (textRect != null)
        {
            Vector2 pos = textRect.anchoredPosition;
            pos.y += scrollSpeed * Time.unscaledDeltaTime;
            textRect.anchoredPosition = pos;

            if (pos.y >= Screen.height * 0.5f + totalHeight * 0.5f)
            {
                endTimer += Time.unscaledDeltaTime;
                if (endTimer >= endHoldSeconds)
                    Finish();
            }
        }
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
