public class BaseAttackState : IEnemyState
{
    public virtual void OnEnter(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        combat.PlayAnimBool("Run", false);
        movement.LookAtPlayer();
    }

    public virtual void OnUpdate(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        if (movement.GetDistanceToPlayer() > combat.GetAttackRange())
        {
            ctx.SwitchTo("Pursuit");
            return;
        }

        if (combat.IsAttackReady())
        {
            combat.ExecuteAttack();
            combat.RecordAttack();
        }
    }

    public virtual void OnExit(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
    }
}
