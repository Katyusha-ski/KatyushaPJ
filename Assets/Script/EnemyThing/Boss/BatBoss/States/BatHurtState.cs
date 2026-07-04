using UnityEngine;

public class BatHurtState : IEnemyState
{
    private float duration = 0.3f;
    private float elapsed;

    public void OnEnter(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        elapsed = 0f;

        if (ctx is BatBossController boss)
        {
            boss.FlashHurt();
        }
    }

    public void OnUpdate(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        elapsed += Time.deltaTime;
        if (elapsed >= duration)
        {
            ctx.SwitchTo("Hover");
        }
    }

    public void OnExit(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx) { }
}
