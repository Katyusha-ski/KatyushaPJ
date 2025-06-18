using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen Instance { get; private set; }
    
    [Header("UI Elements")]
    public GameObject loadingPanel;
    public Slider progressBar;
    public Text progressText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Tự động tìm các UI elements nếu chưa được gán
            if (loadingPanel == null)
                loadingPanel = transform.Find("LoadingPanel")?.gameObject;
            if (progressBar == null)
                progressBar = transform.Find("LoadingPanel/ProgressBar")?.GetComponent<Slider>();
            if (progressText == null)
                progressText = transform.Find("LoadingPanel/ProgressText")?.GetComponent<Text>();
                
            // Ẩn loading panel khi bắt đầu
            if (loadingPanel != null)
                loadingPanel.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(string sceneName)
    {
        if (loadingPanel != null)
            loadingPanel.SetActive(true);
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            
            if (progressBar != null)
                progressBar.value = progress;
            if (progressText != null)
                progressText.text = (progress * 100f).ToString("F0") + "%";

            if (operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true;
            }

            yield return null;
        }

        if (loadingPanel != null)
            loadingPanel.SetActive(false);
    }
}