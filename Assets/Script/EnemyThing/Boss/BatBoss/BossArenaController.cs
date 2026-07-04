using UnityEngine;

public class BossArenaController : MonoBehaviour
{
    [SerializeField] private BatBossController boss;
    [SerializeField] private BossHealthBarUI bossHealthBar;
    [SerializeField] private GameObject arenaGate;
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private AudioClip battleMusic;

    private bool isActive;

    private void Start()
    {
        if (boss != null)
        {
            boss.gameObject.SetActive(false);
            boss.SetHealthBar(bossHealthBar);
            boss.OnBossDefeated += OnBossDefeated;
        }
        if (bossHealthBar != null)
            bossHealthBar.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isActive) return;
        if (!other.CompareTag("Player")) return;

        isActive = true;
        arenaGate?.SetActive(true);

        if (boss != null)
            boss.gameObject.SetActive(true);

        if (bossHealthBar != null && boss != null)
            bossHealthBar.SetBoss(boss.GetComponent<Health>());
    }

    private void OnBossDefeated()
    {
        arenaGate?.SetActive(false);

        if (bossHealthBar != null)
            bossHealthBar.Hide();

        Invoke(nameof(EndBossChapter), 1.5f);
    }

    private void EndBossChapter()
    {
        if (ChapterManager.Instance != null)
            ChapterManager.Instance.CompleteBossChapter();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.15f);
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            Gizmos.DrawCube(transform.position, col.bounds.size);
    }
}
