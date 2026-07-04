using UnityEngine;

public class BatHoverState : IEnemyState
{
    private float timer;

    public void OnEnter(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        timer = 2f;
    }

    public void OnUpdate(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        if (ctx is BatBossController boss)
        {
            boss.UpdateHover(Time.deltaTime);

            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                boss.PickNextAttack();
            }
        }
    }

    public void OnExit(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx) { }
}
