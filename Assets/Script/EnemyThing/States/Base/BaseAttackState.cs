using UnityEngine;
public class BaseAttackState : IEnemyState
{
    private const float AttackCommitWindow = 0.5f;
    private float lastExecuteTime = -Mathf.Infinity;
    public virtual void OnEnter(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        lastExecuteTime = -Mathf.Infinity;
        combat.PlayAnimBool("Run", false);
        movement.LookAtPlayer();
    }

    public virtual void OnUpdate(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        EnemyController enemy = (EnemyController)ctx;
        bool leashExceeded = enemy.DistanceFromHomeX > enemy.MaxChaseDistance;
        if (movement.GetDistanceToPlayer() > combat.GetAttackRange())
        {
            ctx.SwitchTo(leashExceeded ? "ReturnToPost" : "Pursuit");
            return;
        }

        if (leashExceeded && Time.time - lastExecuteTime <= AttackCommitWindow) return;

        if (leashExceeded)
        {
            ctx.SwitchTo("ReturnToPost");
            return;
        }
        
        if (combat.IsAttackReady())
        {
            combat.ExecuteAttack();
            combat.RecordAttack();
            lastExecuteTime = Time.time;
        }
    }

    public virtual void OnExit(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
    }
}
