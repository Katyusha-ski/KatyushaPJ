using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    public GameObject continueBtn;

    [Header("UI Panels")]
    public GameObject aboutMePanel; // Panel shows "About Me" information

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
        if (continueBtn == null) return;

        bool hasSave = GameManager.Instance != null && GameManager.Instance.HasSaveFile();
        continueBtn.SetActive(hasSave);
    }

    public void OnPlayButtonClick()
    {
        // Start new game instead of loading scene directly
        if (GameManager.Instance != null)
        {
            GameManager.Instance.NewGame();
        }
        else
        {
            // Fallback if GameManager not found
            if (GameSceneController.Instance != null)
            {
                GameSceneController.Instance.LoadGameScene("GrassScene");
            }
        }
    }

    public void OnQuitButtonClick()
    {
        if (GameSceneController.Instance != null)
        {
            GameSceneController.Instance.QuitGame();
        }
        else
        {
            Application.Quit();
        }
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
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SaveGame();
            UpdateContinueButton();
        }
        else
        {
            Debug.LogWarning("GameManager not found! Cannot save game.");
        }
    }

    public void OnNewGameButtonClick()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.NewGame();
        }
        else
        {
            Debug.LogWarning("GameManager not found! Cannot start new game.");
        }
    }

    public void OnContinueButtonClick()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadGame();
        }
        else
        {
            Debug.LogWarning("GameManager not found! Cannot load game.");
        }
    }
}
