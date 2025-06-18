using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject aboutMePanel; // Panel hiển thị thông tin About Me
    
    private void Start()
    {
        // Ẩn panel About Me khi bắt đầu
        if (aboutMePanel != null)
        {
            aboutMePanel.SetActive(false);
        }
    }

    public void OnPlayButtonClick()
    {
        // Chuyển đến scene game đầu tiên (GrassScene)
        GameSceneController.Instance.LoadGameScene("GrassScene");
    }

    public void OnQuitButtonClick()
    {
        GameSceneController.Instance.QuitGame();
    }

    public void OnAboutMeButtonClick()
    {
        // Hiển thị panel About Me
        if (aboutMePanel != null)
        {
            aboutMePanel.SetActive(true);
        }
    }

    public void OnCloseAboutMeButtonClick()
    {
        // Ẩn panel About Me
        if (aboutMePanel != null)
        {
            aboutMePanel.SetActive(false);
        }
    }
}
