using UnityEngine;

// Trạng thái dormant dùng chung cho boss trước khi arena kích hoạt
// (Bat ngủ, Golem/Void chờ). Không patrol, không vision-check, không leash:
// wake hoàn toàn do arena gọi BeginEncounter rồi chuyển thẳng sang state chiến đấu.
public class BossDormantState : IEnemyState
{
    public void OnEnter(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        movement.Stop();
        combat.PlayAnimBool("Run", false);
    }

    public void OnUpdate(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx) { }

    public void OnExit(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx) { }
}
