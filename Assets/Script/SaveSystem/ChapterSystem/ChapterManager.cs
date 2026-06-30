using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChapterManager : MonoBehaviour
{
    public static ChapterManager Instance { get; private set; }

    [SerializeField] private List<ChapterDataSO> chapters;
    [SerializeField] private int currentChapterIndex;
    private ChapterDataSO CurrentChapter => chapters[currentChapterIndex];
    public int CurrentChapterNumber => currentChapterIndex + 1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
        }
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
        SceneManager.sceneLoaded += OnVillageLoaded;
        SceneManager.LoadScene("Village");
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
        SceneManager.sceneLoaded += OnVillageLoaded;
        SceneManager.LoadScene("Village");
    }

    private void OnVillageLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "Village") return;
        SceneManager.sceneLoaded -= OnVillageLoaded;
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
