using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject aboutMePanel; // Panel show "About Me" information

    private void Start()
    {
        if (aboutMePanel != null)
        {
            aboutMePanel.SetActive(false);
        }
    }

    public void OnPlayButtonClick()
    {
        GameSceneController.Instance.LoadGameScene("GrassScene");
    }

    public void OnQuitButtonClick()
    {
        GameSceneController.Instance.QuitGame();
    }

    public void OnAboutMeButtonClick()
    {
        if (aboutMePanel != null)
        {
            aboutMePanel.SetActive(true);
        }
    }

    public void OnCloseAboutMeButtonClick()
    {
        if (aboutMePanel != null)
        {
            aboutMePanel.SetActive(false);
        }
    }

    public void OnSaveButtonClick()
    {
        GameManager.Instance.SaveGame();
    }

    public void OnNewGameButtonClick()
    {
        GameManager.Instance.NewGame();
    }

    public void OnContinueButtonClick()
    {
        GameManager.Instance.LoadGame();
    }
}
