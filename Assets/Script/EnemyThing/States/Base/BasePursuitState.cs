using UnityEngine;
public class BasePursuitState : IEnemyState
{
    private float loseTargetTimer;
    public virtual void OnEnter(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        combat.PlayAnimBool("Run", true);
        loseTargetTimer = 0f;
    }

    public virtual void OnUpdate(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        EnemyController enemy = (EnemyController)ctx;

        if (enemy.DistanceFromHomeX > enemy.MaxChaseDistance)
        {
            ctx.SwitchTo("ReturnToPost");
            return;
        }

        float distanceToPlayer = movement.GetDistanceToPlayer();

        if (distanceToPlayer > movement.GetVisionRange())
        {
            loseTargetTimer += Time.deltaTime;
            if (loseTargetTimer > enemy.LoseTargetDelay)
            {
                ctx.SwitchTo("ReturnToPost");
                return;
            }
        }
        else
        {
            loseTargetTimer = 0f;
        }

        if (distanceToPlayer <= combat.GetAttackRange())
        {
            ctx.SwitchTo("Attack");
            return;
        }

        if (movement.IsAtPlatformEdge())
        {
            ctx.SwitchTo("ReturnToPost");
            return;
        }

        movement.Pursue();
    }

    public virtual void OnExit(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        combat.PlayAnimBool("Run", false);
    }
}
