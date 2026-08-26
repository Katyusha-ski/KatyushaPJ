using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Chuyển sang scene khác: fade đen -> LoadScene -> fade sáng lại ở scene mới.
///
/// Điểm mấu chốt: coroutine của action sẽ bị HỦY cùng old scene ngay khi LoadScene
/// chạy, nên việc fade-sáng-lại được giao cho một coroutine cư trú trên FadeUI
/// (Singleton DontDestroyOnLoad) — thứ duy nhất sống xuyên qua lần đổi scene.
///
/// Lưu ý: sceneName phải trùng tên scene trong Build Settings, nếu chưa tạo scene
/// thì nhớ thêm vào Build Settings trước khi action này chạy tới.
/// </summary>
[System.Serializable]
public class SceneTransitionAction : SequenceAction
{
    [Tooltip("Tên scene đúng như đăng ký trong Build Settings. VD: RohokScene")]
    public string sceneName;

    [Tooltip("Nếu > 0 sẽ gọi ChapterManager.SetChapter(số này) trước khi chuyển scene để flow chapter không lệch. 0 = bỏ qua.")]
    public int setChapterNumber = 0;

    [Tooltip("Thời gian fade đen và fade sáng (giây).")]
    public float fadeDuration = 0.5f;

    public override IEnumerator Execute()
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogWarning("[SceneTransitionAction] sceneName đang trống. Action skipped.");
            yield break;
        }
        if (FadeUI.Instance == null || TeleportManager.Instance == null)
        {
            Debug.LogError("[SceneTransitionAction] Thiếu FadeUI hoặc TeleportManager trong scene.");
            yield break;
        }

        if (setChapterNumber > 0)
        {
            if (ChapterManager.Instance != null)
                ChapterManager.Instance.SetChapter(setChapterNumber);
            else
                Debug.LogWarning("[SceneTransitionAction] ChapterManager missing — bỏ qua SetChapter.");
        }

        yield return TeleportManager.Instance.FadeToBlack(fadeDuration);

        // Ủy quyền fade-in cho FadeUI — nó sống xuyên scene nên coroutine không bị hủy
        FadeUI.Instance.StartCoroutine(FadeInAfterSceneLoad(fadeDuration));

        SceneManager.LoadScene(sceneName);
    }

    private static IEnumerator FadeInAfterSceneLoad(float duration)
    {
        string oldScene = SceneManager.GetActiveScene().name;
        while (SceneManager.GetActiveScene().name == oldScene)
            yield return null;

        yield return null; // nhường thêm 1 frame cho Awake/Start của scene mới chạy xong

        if (TeleportManager.Instance != null)
            yield return TeleportManager.Instance.FadeFromBlack(duration);
    }
}
