using UnityEngine;

public class MenuUI : MonoBehaviour
{
    public GameObject UI;

    public void ShowMenu()
    {
        UI.SetActive(true);
        GameManager.Instance.PauseGame();
    }

    public void HideMenu()
    {
        UI.SetActive(false);
        GameManager.Instance.ResumeGame();
    }
}


