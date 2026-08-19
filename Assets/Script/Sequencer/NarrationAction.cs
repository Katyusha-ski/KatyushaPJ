using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

[System.Serializable]
public class NarrationAction : SequenceAction
{
    [TextArea(2, 5)] public string text;
    [Tooltip("Độ tối của lớp phủ (0..1). 1 = đen hẳn (giống Teleport).")]
    public float darkAlpha = 1f;
    [Tooltip("Thời gian fade tối/sáng màn hình (giây).")]
    public float fadeDuration = 0.5f;
    [Tooltip("Thời gian giữ text khi waitForClick = false.")]
    public float autoHoldSeconds = 2f;

    public override bool HandlesClickInternally => true;

    public override IEnumerator Execute()
    {
        if (FadeUI.Instance == null)
        {
            Debug.LogWarning("[NarrationAction] FadeUI missing (cần đặt trong GameUIRoot).");
            yield break;
        }

        Image panel = FadeUI.Instance.FadePanel;
        TextMeshProUGUI textUI = FadeUI.Instance.LoadingText;

        SetPlayerCanMove(false);

        DOTween.Kill(panel);
        panel.color = new Color(0f, 0f, 0f, 0f);
        panel.raycastTarget = true;
        yield return panel.DOFade(darkAlpha, fadeDuration).SetEase(Ease.InQuad).WaitForCompletion();

        textUI.text = text;
        DOTween.Kill(textUI);
        yield return textUI.DOFade(1f, fadeDuration).WaitForCompletion();

        if (waitForClick)
            yield return WaitForClick();
        else
            yield return new WaitForSeconds(autoHoldSeconds);

        DOTween.Kill(textUI);
        yield return textUI.DOFade(0f, fadeDuration).WaitForCompletion();

        DOTween.Kill(panel);
        yield return panel.DOFade(0f, fadeDuration).SetEase(Ease.OutQuad).WaitForCompletion();
        panel.raycastTarget = false;

        SetPlayerCanMove(true);
    }

    private void SetPlayerCanMove(bool canMove)
    {
        if (PlayerManager.Instance == null) return;
        var controller = PlayerManager.Instance.PlayerController;
        if (controller == null) return;
        var movement = controller.GetComponent<PlayerMovementController>();
        if (movement == null) return;
        movement.CanMove = canMove;
    }
}