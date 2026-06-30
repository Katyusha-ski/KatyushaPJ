public class RangedAttackState : BaseAttackState
{
    public override void OnUpdate(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        float distanceToPlayer = movement.GetDistanceToPlayer();

        if (distanceToPlayer > combat.GetAttackRange())
        {
            ctx.SwitchTo("Pursuit");
            return;
        }

        if (movement is IEnemyRanged ranged && distanceToPlayer < ranged.GetCloseDistance())
        {
            ctx.SwitchTo("Kitting");
            return;
        }

        base.OnUpdate(movement, combat, ctx);
    }
}
