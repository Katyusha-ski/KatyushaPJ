public class IdleState : IEnemyState
{
    public void OnEnter(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        combat.PlayAnimBool("Run", false);
    }

    public void OnUpdate(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        if (movement.GetDistanceToPlayer() <= movement.GetVisionRange())
        {
            ctx.SwitchTo("Pursuit");
            return;
        }
        movement.Patrol();
    }

    public void OnExit(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
    }
}
