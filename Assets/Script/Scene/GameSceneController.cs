using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneController : MonoBehaviour
{
    public static GameSceneController Instance { get; private set; }

    [Header("Scene Configuration")]
    public int mainMenuSceneIndex = 0; // Index của main menu

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

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneIndex);
    }

    public void LoadGameScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadNextScene()
    {
        // Lấy index của scene hiện tại
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        
        // Tính toán index của scene tiếp theo
        int nextIndex = currentSceneIndex + 1;
        
        // Kiểm tra xem có phải scene cuối cùng không
        if (nextIndex >= SceneManager.sceneCountInBuildSettings)
        {
            // Nếu là scene cuối, quay về main menu
            nextIndex = mainMenuSceneIndex;
        }
        
        // Load scene tiếp theo
        SceneManager.LoadScene(nextIndex);
    }

    public void RestartCurrentScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
