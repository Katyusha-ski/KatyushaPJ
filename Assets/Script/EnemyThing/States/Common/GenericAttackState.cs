using UnityEngine;
public class GenericAttackState : IEnemyState
{
    private string animTrigger;
    private float animDuration;
    private string returnState;
    private float elapsed;

    public GenericAttackState(string animTrigger, float animDuration, string returnState)
    {
        this.animTrigger = animTrigger;
        this.animDuration = animDuration;
        this.returnState = returnState;
    }

    public void OnEnter(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        elapsed = 0f;
        combat.PlayAnimTrigger(animTrigger);
    }

    public void OnUpdate(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        elapsed += Time.deltaTime;
        if (elapsed >= animDuration)
        {
            ctx.SwitchTo(returnState);
        }
    }

    public void OnExit(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx) { }
}
