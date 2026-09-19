using UnityEngine;
public class GenericAttackState : IEnemyState
{
    private string animTrigger;
    private float animDuration;
    private string returnState;
    private float elapsed;
    private readonly System.Action onAttackEnd;

    public GenericAttackState(string animTrigger, float animDuration, string returnState, System.Action onEnd = null)
    {
        this.animTrigger = animTrigger;
        this.animDuration = animDuration;
        this.returnState = returnState;
        this.onAttackEnd = onEnd;
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
            onAttackEnd?.Invoke();
            ctx.SwitchTo(returnState);
        }
    }

    public void OnExit(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx) { }
}
