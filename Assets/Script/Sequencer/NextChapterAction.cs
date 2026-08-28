using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Advances the current chapter and loads the next chapter's main scene.
/// Uses the same persistent FadeUI/TeleportManager transition as SceneTransitionAction.
/// </summary>
[System.Serializable]
public class NextChapterAction : SequenceAction
{
    [Tooltip("Thời gian fade đen và fade sáng (giây).")]
    public float fadeDuration = 0.5f;

    public override IEnumerator Execute()
    {
        if (ChapterManager.Instance == null)
        {
            Debug.LogError("[NextChapterAction] Không tìm thấy ChapterManager.");
            yield break;
        }

        if (FadeUI.Instance == null || TeleportManager.Instance == null)
        {
            Debug.LogError("[NextChapterAction] Thiếu FadeUI hoặc TeleportManager trong scene.");
            yield break;
        }

        if (!ChapterManager.Instance.TryAdvanceToNextChapter(out string nextSceneName))
            yield break;

        yield return TeleportManager.Instance.FadeToBlack(fadeDuration);

        FadeUI.Instance.StartCoroutine(FadeInAfterSceneLoad(fadeDuration));
        SceneManager.LoadScene(nextSceneName);
    }

    private static IEnumerator FadeInAfterSceneLoad(float duration)
    {
        string oldScene = SceneManager.GetActiveScene().name;
        while (SceneManager.GetActiveScene().name == oldScene)
            yield return null;

        yield return null;

        if (TeleportManager.Instance != null)
            yield return TeleportManager.Instance.FadeFromBlack(duration);
    }
}
