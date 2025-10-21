using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    public GameObject continueBtn;

    [Header("UI Panels")]
    public GameObject aboutMePanel; // Panel show "About Me" information



    private void Start()
    {
        if (aboutMePanel != null)
        {
            aboutMePanel.SetActive(false);
        }
        
        UpdateContinueButton();
    }

    private void UpdateContinueButton()
    {
        bool hasSave = GameManager.Instance != null && GameManager.Instance.HasSaveFile();
        continueBtn.SetActive(hasSave);
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
        UpdateContinueButton();
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
