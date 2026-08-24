using UnityEngine;

public class HurtState : IEnemyState
{
    private float hurtDuration = 0.5f;
    private float elapsedTime = 0f;
    private IEnemyState previousState;
    private string returnState;
    private bool playHurtTrigger;

    public HurtState(IEnemyState previousState)
    {
        this.previousState = previousState;
        this.playHurtTrigger = true;
    }

    public HurtState(string returnState = "Idle", bool playHurtTrigger = true)
    {
        this.returnState = returnState;
        this.playHurtTrigger = playHurtTrigger;
    }

    public void OnEnter(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        elapsedTime = 0f;
        if (playHurtTrigger)
            combat.PlayAnimTrigger("Hurt");
        combat.PlayAnimBool("Run", false);
    }

    public void OnUpdate(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime < hurtDuration) return;

        EnemyController enemy = (EnemyController)ctx;
        ctx.SwitchTo(enemy.ShouldReengage() ? "Pursuit" : "ReturnToPost");
    }

    public void OnExit(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
    }
}
