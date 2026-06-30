public class BasePursuitState : IEnemyState
{
    public virtual void OnEnter(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        combat.PlayAnimBool("Run", true);
    }

    public virtual void OnUpdate(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        float distanceToPlayer = movement.GetDistanceToPlayer();

        if (distanceToPlayer > movement.GetVisionRange())
        {
            ctx.SwitchTo("Idle");
            return;
        }

        if (distanceToPlayer <= combat.GetAttackRange())
        {
            ctx.SwitchTo("Attack");
            return;
        }

        movement.Pursue();
    }

    public virtual void OnExit(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        combat.PlayAnimBool("Run", false);
    }
}
