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
        owner?.HideStateEffect();
    }
}
