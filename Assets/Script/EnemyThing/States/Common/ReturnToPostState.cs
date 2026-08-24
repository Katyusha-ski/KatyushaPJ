using UnityEngine;

public class ReturnToPostState : IEnemyState
{
    public float recoveryTimer;
    public void OnEnter(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        recoveryTimer = 0f;
        combat.PlayAnimBool("Run", true);
    }

    public void OnUpdate(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        EnemyController enemy = (EnemyController)ctx;

        recoveryTimer += Time.deltaTime;
        if (recoveryTimer >= enemy.RecoveryTimeout)
        {
            movement.SnapToPosition(enemy.HomePosition);
            ctx.SwitchTo("Patrol");
            return;
        }
        // 1. Được phép re-engage (ngoài dead zone + thấy Player) → Pursuit
        if (enemy.ShouldReengage())
        {
            ctx.SwitchTo("Pursuit");
            return;
        }

        // 2. Đã về tới patrol zone → giao lại cho Patrol
        float x = enemy.transform.position.x;
        bool xInZone = x >= enemy.PatrolMinX && x <= enemy.PatrolMaxX;
        if (xInZone && movement.IsGrounded())
        {
            movement.SnapToY(enemy.HomePosition.y);
            ctx.SwitchTo("Patrol");
            return;
        }
        if (xInZone)
        {
            movement.Stop();
            return;
        }

        // 3. Còn lại (dead zone hoặc chưa tới nơi) → đi tiếp về home
        movement.MoveTowardsX(enemy.HomePosition.x);
    }

    public void OnExit(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        combat.PlayAnimBool("Run", false);
    }
}
