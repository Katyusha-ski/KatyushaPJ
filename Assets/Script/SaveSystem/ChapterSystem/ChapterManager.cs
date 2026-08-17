using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChapterManager : Singleton<ChapterManager>
{
    [SerializeField] private List<ChapterDataSO> chapters;
    [SerializeField] private int currentChapterIndex;
    private ChapterDataSO CurrentChapter => chapters[currentChapterIndex];
    public int CurrentChapterNumber => currentChapterIndex + 1;

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
