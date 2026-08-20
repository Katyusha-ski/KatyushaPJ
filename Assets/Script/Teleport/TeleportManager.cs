using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class TeleportManager : Singleton<TeleportManager>
{
    [Header("Settings")]
    [SerializeField] private float fadeDuration = 0.5f;

    private Image GetFadePanel()
    {
        return FadeUI.Instance != null ? FadeUI.Instance.FadePanel : null;
    }

    public IEnumerator FadeToBlack(float duration){
        Image fadePanel = GetFadePanel();
        if (fadePanel == null)
        {
            Debug.LogError("[TeleportManager] Không tìm thấy FadeUI (cần đặt trong GameUIRoot).");
            yield break;
        }
        DOTween.Kill(fadePanel);
        fadePanel.color = new Color(0f,0f,0f,0f);
        fadePanel.raycastTarget = true;
        yield return fadePanel.DOFade(1f, duration).SetEase(Ease.InQuad).WaitForCompletion();
    }

    public IEnumerator FadeFromBlack(float duration)
    {
        Image fadePanel = GetFadePanel();
        if (fadePanel == null)
        {
            Debug.LogError("[TeleportManager] Không tìm thấy FadeUI (cần đặt trong GameUIRoot).");
            yield break;
        }
        DOTween.Kill(fadePanel);
        fadePanel.color = new Color(0f, 0f, 0f, 1f);
        yield return fadePanel.DOFade(0f, duration).SetEase(Ease.OutQuad).WaitForCompletion();
        fadePanel.raycastTarget = false;
    }

    public IEnumerator Teleport(Rigidbody2D playerRB, Vector2 destination, string loadingMessage, IEnumerator workDuringBlack = null)
    {
        if (playerRB == null)
        {
            Debug.LogError("[TeleportManager] Player Rigidbody2D is null. Cannot teleport.");
            yield break;
        }
        if (FadeUI.Instance == null)
        {
            Debug.LogError("[TeleportManager] Không tìm thấy FadeUI (cần đặt trong GameUIRoot).");
            yield break;
        }

        yield return FadeToBlack(fadeDuration);

        TextMeshProUGUI loadingText = FadeUI.Instance.LoadingText;
        if (!string.IsNullOrEmpty(loadingMessage))
        {
            loadingText.text = loadingMessage;
            DOTween.Kill(loadingText);
            yield return loadingText.DOFade(1f, fadeDuration).WaitForCompletion();
        }

        // 3) Chờ người đọc câu chữ — click vào vùng trống (panel đen) để tiếp tục
        yield return WaitForClick();

        // 4) DỜI PLAYER — BẮT BUỘC QUA RIGIDBODY
        playerRB.position = destination;               // rb.position, KHÔNG phải transform.position!

        if (workDuringBlack != null)
            yield return workDuringBlack;

        if (!string.IsNullOrEmpty(loadingMessage)) 
        {
            yield return loadingText.DOFade(0f, fadeDuration).WaitForCompletion();
        }
        yield return FadeFromBlack(fadeDuration);
    }

    private IEnumerator WaitForClick()
    {
        while (!Input.GetMouseButtonDown(0))
            yield return null;
    }
}
