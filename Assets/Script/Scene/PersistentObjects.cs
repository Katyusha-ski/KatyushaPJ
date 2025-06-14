using UnityEngine;

public class PersistentObjects : MonoBehaviour
{
    public static PersistentObjects Instance { get; private set; }

    [Header("UI Elements")]
    public GameObject mainUI;        // UI chính
    public GameObject loadingScreen; // Loading screen

    [Header("Managers")]
    public AudioManager audioManager;
    public GameSceneController sceneController;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Đảm bảo các object con cũng không bị destroy
            foreach (Transform child in transform)
            {
                DontDestroyOnLoad(child.gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}