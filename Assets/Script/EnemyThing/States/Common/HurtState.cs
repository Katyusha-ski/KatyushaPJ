using UnityEngine;

public class HurtState : IEnemyState
{
    private float hurtDuration = 0.5f;
    private float elapsedTime = 0f;
    private IEnemyState previousState;

    public HurtState(IEnemyState previousState)
    {
        this.previousState = previousState;
    }

    public void OnEnter(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        elapsedTime = 0f;
        combat.PlayAnimTrigger("Hurt");
        combat.PlayAnimBool("Run", false);
    }

    public void OnUpdate(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= hurtDuration)
        {
            if (previousState != null)
            {
                ctx.SwitchTo("Pursuit");
            }
            else
            {
                ctx.SwitchTo("Idle");
            }
        }
    }

    public void OnExit(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
    }
}
