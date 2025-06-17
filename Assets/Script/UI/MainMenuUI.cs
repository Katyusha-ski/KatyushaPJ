using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public void OnStartButtonClick()
    {
        GameSceneController.Instance.LoadNextScene();
    }

    public void OnQuitButtonClick()
    {
        GameSceneController.Instance.QuitGame();
    }
}
