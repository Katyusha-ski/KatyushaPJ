using UnityEngine;

public class GolemFinalDeathState : IEnemyState
{
    private const float DEATH_DURATION = 2f;
    private float elapsed;

    public void OnEnter(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        elapsed = 0f;
        combat.PlayAnimTrigger("FinalDeath");
    }

    public void OnUpdate(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        elapsed += Time.deltaTime;
        if (elapsed >= DEATH_DURATION)
        {
            ((MonoBehaviour)ctx).GetComponent<GolemController>()?.HandleEnemyDeath();
            Object.Destroy(((MonoBehaviour)ctx).gameObject);
        }
    }

    public void OnExit(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
    }
}
