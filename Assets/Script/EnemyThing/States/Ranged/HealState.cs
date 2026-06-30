using UnityEngine;

public class HealState : IEnemyState
{
    private IEnemyState previousState;

    public HealState()
    {
    }

    public void SetPreviousState(IEnemyState state)
    {
        previousState = state;
    }

    public void OnEnter(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        combat.PlayAnimBool("Run", false);
    }

    public void OnUpdate(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        var health = ((MonoBehaviour)ctx).GetComponent<Health>();
        if (health != null)
        {
            if (health.CurrentHealth < health.MaxHealth * 0.5f)
            {
                combat.PlayAnimTrigger("Heal");
                return;
            }

            ctx.SwitchTo("Pursuit");
        }
    }

    public void OnExit(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
    }
}
