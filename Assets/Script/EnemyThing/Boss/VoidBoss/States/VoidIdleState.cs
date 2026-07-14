using UnityEngine;

public class VoidIdleState : IEnemyState
{
    private const float DECISION_INTERVAL = 0.5f;
    private float decisionTimer;

    public void OnEnter(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        decisionTimer = DECISION_INTERVAL;

        if (ctx is VoidBossController boss)
            boss.UnlockFacing();
    }

    public void OnUpdate(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        decisionTimer -= Time.deltaTime;
        if (decisionTimer > 0f) return;
        decisionTimer = DECISION_INTERVAL;

        if (ctx is VoidBossController boss)
        {
            float distance = movement.GetDistanceToPlayer();

            if (distance <= boss.MeleeRange)
            {
                boss.PickMeleeAttack();
                return;
            }

            if (boss.IsBloodMoonReady())
            {
                boss.UseBloodMoon();
                ctx.SwitchTo("BloodMoon");
                return;
            }

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

            ctx.SwitchTo("Pursuit");
        }
    }

    public void OnExit(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx) { }
}
