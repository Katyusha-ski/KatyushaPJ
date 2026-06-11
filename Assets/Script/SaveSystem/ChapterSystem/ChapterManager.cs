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
        currentChapterIndex = Mathf.Clamp(ChapterNumber - 1, 0, chapters.Count - 1);
    }

    public void CompleteChapter()
    {
        currentChapterIndex++;
        if (currentChapterIndex >= chapters.Count)
        {
            // Credit but i will do it later
            SceneManager.LoadScene("MainMenu"); 
            return;
        }
        GameManager.Instance.SaveGame();
        SceneManager.LoadScene("Village");
    }

    public void GoToMainScene()
    {
        SceneManager.LoadScene(CurrentChapter.mainSceneName);
    }
}
