using UnityEngine;

public class GameOverUI : Singleton<GameOverUI>
{
    protected override bool PersistAcrossScenes => false;

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
