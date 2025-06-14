using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public void OnStartButtonClick()
    {
        GameSceneController.Instance.LoadGameScene("GameScene");
    }

    public void OnQuitButtonClick()
    {
        GameSceneController.Instance.QuitGame();
    }
}
