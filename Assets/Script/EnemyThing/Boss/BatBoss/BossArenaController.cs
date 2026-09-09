using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class BossArenaController : MonoBehaviour
{
    [SerializeField] private BatBossController boss;
    [SerializeField] private CameraFollow bossCamera;
    [SerializeField] private Transform arenaBackground;
    [SerializeField] private float backgroundRevealScale = 1.5f;
    [SerializeField] private bool activateBossOnEnter = true;
    [SerializeField] private UnityEvent onRevealComplete;

    private bool hasRevealed;
    private Collider2D revealTrigger;
    private Vector3 backgroundBaseScale;

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

        if (arenaBackground == null)
        {
            GameObject background = GameObject.Find("BG");
            if (background != null)
                arenaBackground = background.transform;
        }

        if (arenaBackground != null)
            backgroundBaseScale = arenaBackground.localScale;
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
            bossCamera.ZoomToBossReveal(
                boss.transform,
                () => onRevealComplete?.Invoke(),
                UpdateBackgroundRevealScale);
        else
            onRevealComplete?.Invoke();
    }

    private void UpdateBackgroundRevealScale(float progress)
    {
        if (arenaBackground == null)
            return;

        float scale = Mathf.Lerp(1f, backgroundRevealScale, progress);
        arenaBackground.localScale = backgroundBaseScale * scale;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.15f);
        Collider2D col = revealTrigger != null ? revealTrigger : GetComponent<Collider2D>();
        if (col != null)
            Gizmos.DrawCube(col.bounds.center, col.bounds.size);
    }
}
