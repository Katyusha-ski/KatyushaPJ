using System.Collections.Generic;
using UnityEngine;

public class SequenceEnemiesClearedTrigger : MonoBehaviour
{
    [SerializeField] private SequencePlayer sequencePlayer;

    [SerializeField, Tooltip("Để trống = theo dõi TẤT CẢ enemy tag 'Enemy' trong scene. Kéo Health vào = chỉ chơi khi các enemy được đánh dấu này chết hết.")]
    private List<Health> markedEnemies = new List<Health>();

    private readonly HashSet<Health> aliveEnemies = new HashSet<Health>();
    private bool hasFired;

    private void Awake()
    {
        if (sequencePlayer == null)
            sequencePlayer = GetComponent<SequencePlayer>();
    }

    private void Start()
    {
        if (sequencePlayer == null)
        {
            Debug.LogWarning("[SequenceEnemiesClearedTrigger] Missing SequencePlayer!");
            enabled = false;
            return;
        }

        CollectTrackedEnemies();
        foreach (Health health in aliveEnemies)
            health.OnDied += HandleEnemyDied;

        CheckAllDefeated();
    }

    private void OnDestroy()
    {
        foreach (Health health in aliveEnemies)
        {
            if (health != null)
                health.OnDied -= HandleEnemyDied;
        }
    }

    private void CollectTrackedEnemies()
    {
        aliveEnemies.Clear();

        if (markedEnemies.Count > 0)
        {
            foreach (Health health in markedEnemies)
            {
                if (health != null && health.CurrentHealth > 0)
                    aliveEnemies.Add(health);
            }
        }
        else
        {
            foreach (Health health in FindObjectsByType<Health>(FindObjectsSortMode.None))
            {
                if (health.gameObject.CompareTag("Enemy") && health.CurrentHealth > 0)
                    aliveEnemies.Add(health);
            }
        }

        if (aliveEnemies.Count == 0)
            Debug.LogWarning("[SequenceEnemiesClearedTrigger] Không track được enemy nào - cutscene sẽ chạy ngay khi Start!");
    }

    private void HandleEnemyDied(Health diedEnemy)
    {
        diedEnemy.OnDied -= HandleEnemyDied;
        aliveEnemies.Remove(diedEnemy);
        CheckAllDefeated();
    }

    private void CheckAllDefeated()
    {
        if (hasFired || aliveEnemies.Count > 0) return;

        hasFired = true;
        sequencePlayer.Play();
    }
}
