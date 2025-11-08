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

    [Header("Floating Effect")]
    [SerializeField] private float floatHeight = 0.4f;
    [SerializeField] private float floatSpeed = 2f;

    private Vector3 originalPos;
    private bool hasBeenUsed = false;

    private void Start()
    {
        originalPos = transform.localPosition;
    }

    private void Update()
    {
        float newY = originalPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.localPosition = new Vector3(originalPos.x, newY, originalPos.z);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null || !collision.CompareTag("Player"))
            return;

        // Only save if not used OR not single use
        if (!(singleUse && hasBeenUsed))
        {
            SaveGameAtPoint();
        }
    }

    public void SaveGameAtPoint()
    {
        // Check GameManager before saving
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager not found! Cannot save progress.");
            return;
        }
        
        GameManager.Instance.SaveGame();
        hasBeenUsed = true;     

        Debug.Log($"Game saved at {gameObject.name}");

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