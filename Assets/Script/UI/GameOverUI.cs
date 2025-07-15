using UnityEngine;

public class GameOverUI: MonoBehaviour
{
    public static GameOverUI Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
       
    }
    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void ShowGameOverUI()
    {
        gameObject.SetActive(true);
        GameManager.Instance.PauseGame();
    }

    public void HideGameOverUI()
    {
        gameObject.SetActive(false);
        GameManager.Instance.ResumeGame();
    }
}
