using UnityEngine;

public class GolemFinalDeathState : IEnemyState
{
    private const float DEATH_DURATION = 2f;
    private float elapsed;

    public void OnEnter(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        elapsed = 0f;
        // Controller của golem chỉ có trigger "Die" (không có "FinalDeath"
        // riêng) nên dùng "Die" để animation chết thực sự phát.
        combat.PlayAnimTrigger("Die");
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
