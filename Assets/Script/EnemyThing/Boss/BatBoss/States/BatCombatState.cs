using UnityEngine;

public class BatCombatState : IEnemyState
{
    public void OnEnter(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        combat.PlayAnimBool("Run", true);
    }

    public void OnUpdate(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        if (!(ctx is BatBossController boss))
            return;

        float distanceToPlayer = boss.GetDistanceToPlayer();

        if (distanceToPlayer <= combat.GetAttackRange())
        {
            boss.FacePlayer();

            if (combat.IsAttackReady())
            {
                boss.PickNextAttack();
                combat.RecordAttack();
            }

            return;
        }

        boss.ChasePlayer(Time.deltaTime);
    }

    public void OnExit(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        combat.PlayAnimBool("Run", false);
    }
}
