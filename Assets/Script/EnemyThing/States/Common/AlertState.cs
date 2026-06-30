using UnityEngine;

public class AlertState : IEnemyState
{
    private float alertDuration = 0.5f;
    private float elapsedTime = 0f;

    public void OnEnter(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        elapsedTime = 0f;
        combat.PlayAnimBool("Run", false);
        combat.PlayAnimTrigger("Alert");
    }

    public void OnUpdate(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        if (movement.GetDistanceToPlayer() <= movement.GetVisionRange())
        {
            ctx.SwitchTo("Pursuit");
            return;
        }

        elapsedTime += Time.deltaTime;
        if (elapsedTime >= alertDuration)
        {
            ctx.SwitchTo("Idle");
        }
    }

    public void OnExit(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
    }
}
