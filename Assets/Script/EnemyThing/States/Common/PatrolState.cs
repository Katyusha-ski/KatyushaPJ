using UnityEngine;

public class PatrolState : IEnemyState
{
    public void OnEnter(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        combat.PlayAnimBool("Run", true);
    }

    public void OnUpdate(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        if (((EnemyController)ctx).ShouldReturnHome())
        {
            ctx.SwitchTo("ReturnToPost");
            return;
        }

        if (movement.GetDistanceToPlayer() <= movement.GetVisionRange())
        {
            ctx.SwitchTo("Pursuit");
            return;
        }

        movement.Patrol();
    }

    public void OnExit(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        combat.PlayAnimBool("Run", false);
    }
}
