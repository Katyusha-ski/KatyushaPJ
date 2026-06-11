using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class VillageExitTrigger : MonoBehaviour
{
    [Header("Visual Settings")]
    public GameObject transitionEffectPrefab;
    public float transitionDelay = 1f;

    private bool isTriggered = false;

    private void Awake()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null || !collision.CompareTag("Player") || isTriggered)
            return;

        isTriggered = true;

        if (transitionEffectPrefab != null)
        {
            Instantiate(transitionEffectPrefab, transform.position, Quaternion.identity);
        }

        if (transitionDelay > 0)
        {
            Invoke(nameof(GoToMainScene), transitionDelay);
        }
        else
        {
            GoToMainScene();
        }
    }

    private void GoToMainScene()
    {
        if (ChapterManager.Instance != null)
        {
            ChapterManager.Instance.GoToMainScene();
        }
        else
        {
            Debug.LogError("ChapterManager not found!");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, Vector3.one);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(transform.position, Vector3.one * 1.5f);
    }
}
