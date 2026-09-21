using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Boss "vô hình" — không có Health/hitbox/tấn công riêng.
/// Quản lý một danh sách enemy có sẵn trong scene (đang inactive), active lần lượt
/// tối đa <see cref="maxActiveEnemies"/> con cùng lúc, buff stat theo target để
/// cân bằng enemy yếu/mạnh, và coi "đánh bại hết danh sách" = hạ boss.
/// </summary>
public class WaveBossController : MonoBehaviour
{
    [Header("Enemy Pool (đặt sẵn trong scene, để inactive)")]
    [Tooltip("Danh sách enemy object đã có sẵn trong scene, SetActive = false từ đầu.")]
    [SerializeField] private List<GameObject> enemyPool = new List<GameObject>();

    [Tooltip("Số enemy tối đa xuất hiện cùng lúc.")]
    [SerializeField] private int maxActiveEnemies = 4;

    [Header("Balanced Buff (san bằng theo target stat)")]
    [Tooltip("Mọi enemy có MaxHP gốc thấp hơn giá trị này sẽ được cộng thêm cho bằng đúng target. Enemy vốn đã cao hơn thì giữ nguyên.")]
    [SerializeField] private float targetHP = 100f;

    [Tooltip("Tương tự targetHP nhưng áp cho Atk. LƯU Ý: chỉ có tác dụng với enemy dùng DealNormalAttackDamage() (melee thường). Enemy dùng skill (vd Necromancer, mọi ranged qua SkillManager) KHÔNG đọc Atk, dùng targetSkillAmp bên dưới thay thế.")]
    [SerializeField] private float targetAtk = 15f;

    [Tooltip("Buff SkillAmp (%) cho enemy tấn công bằng skill (SpawnDamageSkillBase/DirectDmgSkillBase đọc CharacterStats.SkillAmp, không đọc Atk). Vô hại với enemy không dùng skill.")]
    [SerializeField] private float targetSkillAmp = 30f;

    [Header("Boss Health Bar (tái dùng UI có sẵn)")]
    [Tooltip("Instance của Assets/Resources/Prefab/UI/BossHealthBarUI.prefab đặt trong scene (Canvas hoặc child của object này) — KHÔNG có sẵn trong GameUIRoot, phải tự instantiate prefab riêng, giống pattern Bat/VoidBoss/Golem_Orange/Golem_Blue đang dùng.")]
    [SerializeField] private BossHealthBarUI bossHealthBar;

    /// <summary>Bắn khi toàn bộ enemy trong pool đã bị đánh bại. Dùng để hook vào Sequencer/cutscene.</summary>
    public event System.Action OnBossDefeated;

    private Queue<GameObject> pendingEnemies;
    private readonly List<Health> activeHealths = new List<Health>();
    private int totalEnemyCount;
    private int remainingEnemyCount;
    private bool bossStarted;

    /// <summary>Gọi hàm này từ Arena trigger / Sequencer action để bắt đầu trận boss.</summary>
    public void StartBossFight()
    {
        if (bossStarted)
        {
            Debug.LogWarning($"{name}: StartBossFight() đã được gọi trước đó, bỏ qua.");
            return;
        }

        if (enemyPool.Count == 0)
        {
            Debug.LogError($"{name}: enemyPool rỗng, không thể bắt đầu wave boss.");
            return;
        }

        bossStarted = true;
        pendingEnemies = new Queue<GameObject>(enemyPool);
        totalEnemyCount = enemyPool.Count;
        remainingEnemyCount = totalEnemyCount;

        // Subscribe OnDied cho TOÀN BỘ enemy trong pool ngay từ đầu, kể cả những
        // con chưa được active. An toàn vì enemy inactive không thể bị damage nên
        // OnDied chỉ thực sự bắn sau khi con đó được SpawnNext() active lên và chết.
        foreach (var enemyGO in enemyPool)
        {
            if (enemyGO == null) continue;
            var health = enemyGO.GetComponent<Health>();
            if (health == null)
            {
                Debug.LogWarning($"{enemyGO.name}: thiếu Health, sẽ không tính vào tiến trình wave boss.");
                continue;
            }
            health.OnDied += HandleEnemyDied;
            activeHealths.Add(health);
        }

        if (bossHealthBar != null)
        {
            bossHealthBar.gameObject.SetActive(true);
            bossHealthBar.SetMaxHealth(totalEnemyCount);
            bossHealthBar.SetHealth(remainingEnemyCount, totalEnemyCount);
        }

        int initialSpawnCount = Mathf.Min(maxActiveEnemies, pendingEnemies.Count);
        for (int i = 0; i < initialSpawnCount; i++)
        {
            SpawnNext();
        }
    }

    private void SpawnNext()
    {
        if (pendingEnemies.Count == 0) return;

        GameObject go = pendingEnemies.Dequeue();
        if (go == null)
        {
            // Slot hỏng trong list, bỏ qua và kéo con tiếp theo lên thay chỗ.
            SpawnNext();
            return;
        }

        var stats = go.GetComponent<CharacterStats>();

        // Buff PHẢI áp trước SetActive: Health.Awake() đọc characterStats.MaxHP
        // ngay khi Awake chạy (lần đầu object được active), nên buff phải có mặt trước đó.
        if (stats != null)
        {
            ApplyBalancedBuff(stats);
        }
        else
        {
            Debug.LogWarning($"{go.name}: thiếu CharacterStats, spawn không buff.");
        }

        go.SetActive(true);
        // Không subscribe OnDied ở đây nữa — đã subscribe cho toàn bộ pool
        // trong StartBossFight(), tránh subscribe trùng gây gọi HandleEnemyDied 2 lần.
    }

    private void ApplyBalancedBuff(CharacterStats stats)
    {
        float hpDiff = targetHP - stats.MaxHP;
        if (hpDiff > 0f)
        {
            stats.AddStatModifier(StatType.MaxHP, new StatsModifier(hpDiff, ModifierType.Additive, "WaveBoss"));
        }

        float atkDiff = targetAtk - stats.Atk;
        if (atkDiff > 0f)
        {
            stats.AddStatModifier(StatType.Atk, new StatsModifier(atkDiff, ModifierType.Additive, "WaveBoss"));
        }

        // Enemy dùng skill (Necromancer, mọi ranged qua SkillManager) đọc SkillAmp
        // chứ không đọc Atk — buff riêng để wave vẫn cân bằng với nhóm này.
        float skillAmpDiff = targetSkillAmp - stats.SkillAmp;
        if (skillAmpDiff > 0f)
        {
            stats.AddStatModifier(StatType.SkillAmp, new StatsModifier(skillAmpDiff, ModifierType.Additive, "WaveBoss"));
        }
    }

    private void HandleEnemyDied(Health deadHealth)
    {
        deadHealth.OnDied -= HandleEnemyDied;
        activeHealths.Remove(deadHealth);

        remainingEnemyCount--;
        bossHealthBar?.SetHealth(remainingEnemyCount, totalEnemyCount);

        if (pendingEnemies.Count > 0)
        {
            SpawnNext();
        }
        else if (remainingEnemyCount <= 0)
        {
            HandleBossDefeated();
        }
    }

    private void HandleBossDefeated()
    {
        bossHealthBar?.Hide();
        OnBossDefeated?.Invoke();
    }

    private void OnDestroy()
    {
        // Phòng trường hợp WaveBossController bị destroy giữa chừng (đổi scene, v.v.)
        foreach (var health in activeHealths)
        {
            if (health != null)
            {
                health.OnDied -= HandleEnemyDied;
            }
        }
    }
}
