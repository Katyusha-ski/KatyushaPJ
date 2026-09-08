using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class BossArenaController : MonoBehaviour
{
    [SerializeField] private BatBossController boss;
    [SerializeField] private CameraFollow bossCamera;
    [SerializeField] private bool activateBossOnEnter = true;
    [SerializeField] private UnityEvent onRevealComplete;

    private bool hasRevealed;
    private Collider2D revealTrigger;

    private void Awake()
    {
        if (!TryGetComponent(out revealTrigger))
        {
            Debug.LogError($"{nameof(BossArenaController)} on {name} requires a Collider2D.", this);
            enabled = false;
            return;
        }

        revealTrigger.isTrigger = true;

        if (bossCamera == null)
            bossCamera = FindFirstObjectByType<CameraFollow>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasRevealed) return;
        if (!other.CompareTag("Player")) return;

        hasRevealed = true;

        if (activateBossOnEnter && boss != null && !boss.gameObject.activeSelf)
            boss.gameObject.SetActive(true);

        boss?.BeginEncounter();

        if (bossCamera != null && boss != null)
            bossCamera.ZoomToBossReveal(boss.transform, () => onRevealComplete?.Invoke());
        else
            onRevealComplete?.Invoke();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.15f);
        Collider2D col = revealTrigger != null ? revealTrigger : GetComponent<Collider2D>();
        if (col != null)
            Gizmos.DrawCube(col.bounds.center, col.bounds.size);
    }
}
