using UnityEngine;

public class BatDieState : IEnemyState
{
    private float elapsed;
    private bool hasDied;

    public void OnEnter(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        elapsed = 0f;
        hasDied = false;
    }

    public void OnUpdate(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        elapsed += Time.deltaTime;

        if (!hasDied && ctx is BatBossController boss)
        {
            hasDied = true;
            boss.HandleEnemyDeath();
        }

        if (elapsed >= 2f)
        {
            Object.Destroy(((MonoBehaviour)ctx).gameObject);
        }
    }

    public void OnExit(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx) { }
}
