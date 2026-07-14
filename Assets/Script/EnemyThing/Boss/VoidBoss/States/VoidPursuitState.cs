using UnityEngine;

public class VoidPursuitState : IEnemyState
{
    public void OnEnter(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        combat.PlayAnimBool("Run", true);
    }

    public void OnUpdate(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        if (ctx is VoidBossController boss)
        {
            float distance = movement.GetDistanceToPlayer();

            // --- Interrupt: skill hồi khi đang chạy → dừng xả chiêu ---
            if (distance > boss.MeleeRange)
            {
                if (boss.IsSkill2Ready())
                {
                    boss.UseSkill2();
                    ctx.SwitchTo("AmbushSummon");
                    return;
                }

                if (boss.IsSkill1Ready())
                {
                    boss.UseSkill1();
                    ctx.SwitchTo("VoidSphere");
                    return;
                }
            }

            // --- Vào tầm cận chiến → NA ---
            if (distance <= boss.MeleeRange)
            {
                boss.PickMeleeAttack();
                return;
            }

            // --- Mất tầm nhìn → về VoidIdle chờ ---
            if (distance > movement.GetVisionRange())
            {
                ctx.SwitchTo("VoidIdle");
                return;
            }

            // --- Tiếp tục truy đuổi (Facing Lock guard) ---
            if (!boss.IsFacingLocked)
            {
                movement.LookAtPlayer();
                movement.MoveTowardPlayer();
            }
        }
    }

    public void OnExit(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        combat.PlayAnimBool("Run", false);
    }
}
