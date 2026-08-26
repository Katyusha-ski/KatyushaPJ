using UnityEngine;

public class MenuUI : MonoBehaviour
{
    public GameObject UI;

    public void ShowMenuAndPause()
    {
        UI.SetActive(true);
        GameManager.Instance.PauseGame();
    }

    public void HideMenuAndResume()
    {
        UI.SetActive(false);
        GameManager.Instance.ResumeGame();
    }
}


