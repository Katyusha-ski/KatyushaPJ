using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// Hiện một ảnh cutscene trên màn hình + làm tối phần xung quanh bằng FadePanel
/// của FadeUI (alpha tối đa mặc định 0.9), rồi tự tháo ra khi kết thúc.
///
/// Flow: fade tối nền -> hiện ảnh (fade-in) -> giữ (chờ click hoặc auto-hold)
///       -> fade-out ảnh -> fade sáng lại nền.
/// Ảnh được tạo/tái dùng dưới cùng Canvas với FadePanel, preserveAspect nên
/// không bị méo dù kích thước ảnh khác tỉ lệ màn hình.
/// </summary>
[System.Serializable]
public class ShowImageAction : SequenceAction
{
    public Sprite image;

    [Tooltip("Độ tối lớp phủ xung quanh (0..1). Mặc định 0.9.")]
    public float darkAlpha = 0.9f;

    [Tooltip("Thời gian fade tối/sáng nền và fade ảnh (giây).")]
    public float fadeDuration = 0.5f;

    [Tooltip("Thời gian giữ ảnh khi waitForClick = false.")]
    public float autoHoldSeconds = 2f;

    private const string ImageName = "CutsceneImage";

    public override bool HandlesClickInternally => true;

    public override IEnumerator Execute()
    {
        if (image == null)
        {
            Debug.LogWarning("[ShowImageAction] Chưa gán Sprite ảnh. Action skipped.");
            yield break;
        }

        if (FadeUI.Instance == null)
        {
            Debug.LogWarning("[ShowImageAction] FadeUI missing (cần đặt trong GameUIRoot).");
            yield break;
        }

        Image panel = FadeUI.Instance.FadePanel;
        Image cutsceneImage = GetOrCreateImage(panel);

        // 1. Làm tối nền
        DOTween.Kill(panel);
        panel.color = new Color(0f, 0f, 0f, 0f);
        panel.raycastTarget = true;
        yield return panel.DOFade(darkAlpha, fadeDuration).SetEase(Ease.InQuad).WaitForCompletion();

        // 2. Fade-in ảnh
        cutsceneImage.sprite = image;
        cutsceneImage.gameObject.SetActive(true);
        cutsceneImage.transform.SetAsLastSibling();
        Color c = cutsceneImage.color; c.a = 0f; cutsceneImage.color = c;
        DOTween.Kill(cutsceneImage);
        yield return cutsceneImage.DOFade(1f, fadeDuration).WaitForCompletion();

        // 3. Giữ ảnh
        if (waitForClick)
            yield return WaitForClick();
        else
            yield return new WaitForSeconds(autoHoldSeconds);

        // 4. Fade-out ảnh rồi sáng lại nền
        DOTween.Kill(cutsceneImage);
        yield return cutsceneImage.DOFade(0f, fadeDuration).WaitForCompletion();
        cutsceneImage.gameObject.SetActive(false);

        DOTween.Kill(panel);
        yield return panel.DOFade(0f, fadeDuration).SetEase(Ease.OutQuad).WaitForCompletion();
        panel.raycastTarget = false;
    }

    /// <summary>Tìm image tái sử dụng dưới Canvas của FadePanel; chưa có thì tạo mới full-screen.</summary>
    private static Image GetOrCreateImage(Image panel)
    {
        Canvas canvas = panel.GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("[ShowImageAction] FadePanel không nằm dưới bất kỳ Canvas nào.");
            return null;
        }

        Transform found = canvas.transform.Find(ImageName);
        if (found != null)
            return found.GetComponent<Image>();

        GameObject go = new GameObject(ImageName, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(canvas.transform, false);

        var rt = (RectTransform)go.transform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        var img = go.GetComponent<Image>();
        img.preserveAspect = true;
        img.raycastTarget = false;
        go.SetActive(false);
        return img;
    }
}
