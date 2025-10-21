using UnityEngine;


[RequireComponent(typeof(Collider2D))]
public class SavePoint : MonoBehaviour
{
    [Header("Save Point Settings")]
    [Tooltip("Save point can only be used once")]
    public bool singleUse = false;

    [Header("Visual Feedback")]
    public GameObject saveEffectPrefab;
    public AudioClip saveSound;

    private bool hasBeenUsed = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!(singleUse && hasBeenUsed))
            {
                SaveGameAtPoint();
            }
        }
    }

    public void SaveGameAtPoint()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("Game Manager not found!");
        }
        GameManager.Instance.SaveGame();
        hasBeenUsed = true;     

        Debug.Log($"? Game have been saved");

        if (saveEffectPrefab != null)
        {
            Instantiate(saveEffectPrefab, transform.position, Quaternion.identity);
        }

        if (saveSound != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(saveSound);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, Vector3.one * 2f);
    }
}