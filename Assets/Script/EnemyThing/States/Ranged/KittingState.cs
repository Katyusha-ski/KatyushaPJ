public class KittingState : IEnemyState
{
    public void OnEnter(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        combat.PlayAnimBool("Run", false);
    }

    public void OnUpdate(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        if (movement is IEnemyRanged ranged)
        {
            float distanceToPlayer = movement.GetDistanceToPlayer();

            if (distanceToPlayer > combat.GetAttackRange())
            {
                ctx.SwitchTo("Pursuit");
                return;
            }

            if (distanceToPlayer >= ranged.GetCloseDistance() && distanceToPlayer <= ranged.GetPreferredDistance())
            {
                ctx.SwitchTo("Attack");
                return;
            }

            ranged.Kitting();
        }
    }

    public void OnExit(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
    }
}
