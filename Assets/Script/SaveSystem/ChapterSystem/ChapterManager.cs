using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChapterManager : Singleton<ChapterManager>
{
    [SerializeField] private List<ChapterDataSO> chapters;
    [SerializeField] private int currentChapterIndex;
    private ChapterDataSO CurrentChapter => chapters[currentChapterIndex];
    public int CurrentChapterNumber => currentChapterIndex + 1;

    /// <summary>
    /// Advances to the next chapter without loading a scene.
    /// The caller can then perform its own transition (for example, with a fade).
    /// </summary>
    public bool TryAdvanceToNextChapter(out string mainSceneName)
    {
        mainSceneName = null;

        if (chapters == null || chapters.Count == 0)
        {
            Debug.LogError("[ChapterManager] chapters list is null or empty!");
            return false;
        }

        if (currentChapterIndex < 0 || currentChapterIndex >= chapters.Count)
        {
            Debug.LogError($"[ChapterManager] currentChapterIndex is invalid: {currentChapterIndex}");
            return false;
        }

        int nextChapterIndex = currentChapterIndex + 1;
        if (nextChapterIndex >= chapters.Count)
        {
            Debug.LogWarning("[ChapterManager] Đã ở chapter cuối, không có chapter tiếp theo.");
            return false;
        }

        ChapterDataSO nextChapter = chapters[nextChapterIndex];
        if (nextChapter == null || string.IsNullOrWhiteSpace(nextChapter.mainSceneName))
        {
            Debug.LogError($"[ChapterManager] Chapter {nextChapterIndex + 1} chưa có dữ liệu main scene.");
            return false;
        }

        currentChapterIndex = nextChapterIndex;
        mainSceneName = nextChapter.mainSceneName;

        if (GameManager.Instance != null)
            GameManager.Instance.SaveGame();

        return true;
    }

    public void SetChapter(int ChapterNumber)
    {
        if (chapters == null || chapters.Count == 0)
        {
            Debug.LogError("[ChapterManager] chapters list is null or empty!");
            return;
        }
        currentChapterIndex = Mathf.Clamp(ChapterNumber - 1, 0, chapters.Count - 1);
    }

    public void CompleteChapter()
    {
        if (chapters == null || chapters.Count == 0 || currentChapterIndex >= chapters.Count)
        {
            Debug.LogError("[ChapterManager] chapters list is invalid!");
            return;
        }

        if (!string.IsNullOrEmpty(CurrentChapter.bossSceneName))
        {
            SceneManager.LoadScene(CurrentChapter.bossSceneName);
            return;
        }

        currentChapterIndex++;
        if (currentChapterIndex >= chapters.Count)
        {
            SceneManager.LoadScene("MainMenu"); 
            return;
        }
        GoToMainScene();
        if (GameManager.Instance != null)
            GameManager.Instance.SaveGame();
    }

    public void CompleteBossChapter()
    {
        if (chapters == null || chapters.Count == 0)
        {
            Debug.LogError("[ChapterManager] chapters list is invalid!");
            return;
        }
        currentChapterIndex++;
        if (currentChapterIndex >= chapters.Count)
        {
            SceneManager.LoadScene("MainMenu");
            return;
        }
        GoToMainScene();
        if (GameManager.Instance != null)
            GameManager.Instance.SaveGame();
    }

    public void GoToMainScene()
    {
        if (chapters == null || chapters.Count == 0 || currentChapterIndex >= chapters.Count)
        {
            Debug.LogError("[ChapterManager] chapters list is invalid!");
            return;
        }
        SceneManager.LoadScene(CurrentChapter.mainSceneName);
    }
}
