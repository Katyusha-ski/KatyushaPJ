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

    public void JustShowMenu()
    {
        UI.SetActive(true);
    }

    public void JustHideMenu()
    {
        if (UI != null) UI.SetActive(false);
    }
}


