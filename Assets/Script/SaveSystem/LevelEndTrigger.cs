using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Level End Trigger: Auto save and load next scene
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class LevelEndTrigger : MonoBehaviour
{
    [Header("Level Transition Settings")]
    [Tooltip("Load next scene in build index")]
    public bool loadNextScene = true;

    [Tooltip("Or specify scene name")]
    public string targetSceneName;

    [Header("Visual Settings")]
    public GameObject transitionEffectPrefab;
    public float transitionDelay = 1f;

    private bool isTriggered = false;

    private void Awake()
    {
        Collider2D col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isTriggered)
        {
            isTriggered = true;
            CompleteLevelAndTransition();
        }
    }

    private void CompleteLevelAndTransition()
    {
        Debug.Log($"Level completed! Transitioning...");

        // Always save when completing level - no toggle needed
        if (GameManager.Instance != null)
        {
            GameManager.Instance.currentLevel++;
            GameManager.Instance.SaveGame();
            Debug.Log($"? Progress saved! Current Level: {GameManager.Instance.currentLevel}");
        }
        else
        {
            Debug.LogError("GameManager not found! Cannot save progress.");
        }

        // Play transition effect if assigned
        if (transitionEffectPrefab != null)
        {
            Instantiate(transitionEffectPrefab, transform.position, Quaternion.identity);
        }

        // Load next level after delay
        Invoke(nameof(LoadNextLevel), transitionDelay);
    }

    private void LoadNextLevel()
    {
        // Priority 1: Use specified target scene name
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            if (GameSceneController.Instance != null)
            {
                GameSceneController.Instance.LoadGameScene(targetSceneName);
            }
            else
            {
                SceneManager.LoadScene(targetSceneName);
            }
            return;
        }

        // Priority 2: Load next scene by build index
        if (loadNextScene)
        {
            if (GameSceneController.Instance != null)
            {
                GameSceneController.Instance.LoadNextScene();
            }
            else
            {
                // Fallback: load next by build index
                Scene current = SceneManager.GetActiveScene();
                int nextIndex = current.buildIndex + 1;

                if (nextIndex < SceneManager.sceneCountInBuildSettings)
                {
                    SceneManager.LoadScene(nextIndex);
                }
                else
                {
                    Debug.LogWarning("No more scenes to load! Reached end of build settings.");
                }
            }
        }
        else
        {
            Debug.LogWarning("No target scene specified and loadNextScene is disabled!");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, Vector3.one);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(transform.position, Vector3.one * 1.5f);
    }
}