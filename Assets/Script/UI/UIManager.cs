using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

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
