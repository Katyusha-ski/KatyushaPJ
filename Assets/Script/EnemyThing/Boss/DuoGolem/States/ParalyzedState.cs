using UnityEngine;

public class ParalyzedState : IEnemyState
{
    private GolemController owner;

    public ParalyzedState(GolemController owner)
    {
        this.owner = owner;
    }

    public void OnEnter(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        // Keep the downed body at its current ground position. The collider is
        // intentionally disabled, so gravity must be suspended as well.
        Rigidbody2D rb = ((MonoBehaviour)ctx).GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.gravityScale = 0f;
        }

        // Disable ALL colliders (body + hurtbox) to prevent:
        //   - Player walking through (intended)
        //   - Hachiware/companion farming lifesteal on paralyzed body (crucial)
        //   - Any raycast/targeting system detecting this entity
        Collider2D[] colliders = ((MonoBehaviour)ctx).GetComponents<Collider2D>();
        foreach (Collider2D col in colliders)
        {
            if (col != null)
                col.enabled = false;
        }

        if (owner != null)
        {
            combat.PlayAnimTrigger("Knockdown");
            owner.MyHazards?.Cleanup();
            owner.ShowStateEffect(new Color(1f, 0.15f, 0.15f, 0.35f));
        }
    }

    public void OnUpdate(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        // no-op: fully disabled, waiting for revival
    }

    public void OnExit(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        Rigidbody2D rb = ((MonoBehaviour)ctx).GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.gravityScale = 1f;

        owner?.HideStateEffect();
    }
}
