using UnityEngine;

public class GolemAttackState : IEnemyState
{
    private float animDuration;
    private float elapsed;

    public GolemAttackState(float animDuration)
    {
        this.animDuration = animDuration;
    }

    public void OnEnter(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        elapsed = 0f;
        combat.PlayAnimBool("Run", false);
        movement.LookAtPlayer();
        if (ctx is GolemController golem)
            golem.SetAnimationSpeed(golem.AttackAnimationSpeed);
        combat.PlayAnimTrigger("Punch");
    }

    public void OnUpdate(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        elapsed += Time.deltaTime;
        if (elapsed >= animDuration)
        {
            ctx.SwitchTo("Pursuit");
        }
    }

    public void OnExit(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        if (ctx is GolemController golem)
            golem.SetAnimationSpeed(1f);
    }
}
