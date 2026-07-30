using UnityEngine;

public class GolemAttackState : IEnemyState
{
    private int damage;
    private float animDuration;
    private float elapsed;

    public GolemAttackState(int damage, float animDuration)
    {
        this.damage = damage;
        this.animDuration = animDuration;
    }

    public void OnEnter(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        elapsed = 0f;
        combat.PlayAnimBool("Run", false);
        movement.LookAtPlayer();
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

    public void OnExit(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx) { }
}
