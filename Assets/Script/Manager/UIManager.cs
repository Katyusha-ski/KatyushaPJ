using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    public void PlayBtnSfx()
    {
        ButtonSFX.Instance.PlayBtnSFX();
    }

    public void LoadNextScene() 
    {
        GameSceneController.Instance.LoadNextScene();
    }
    public void RestartCurrentScene()
    {
        GameSceneController.Instance.RestartCurrentScene();
    }

    public void LoadMainMenu()
    {
        GameSceneController.Instance.LoadMainMenu();
    }

}
