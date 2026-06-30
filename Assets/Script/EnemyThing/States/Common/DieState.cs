using UnityEngine;

public class DieState : IEnemyState
{
    private float dieDuration = 1.0f;
    private float elapsedTime = 0f;

    public void OnEnter(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        combat.PlayAnimTrigger("Die");
    }

    public void OnUpdate(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= dieDuration)
        {
            Object.Destroy(((MonoBehaviour)ctx).gameObject);
        }
    }

    public void OnExit(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
    }
}
