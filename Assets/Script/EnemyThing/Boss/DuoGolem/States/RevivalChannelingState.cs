using UnityEngine;

public class RevivalChannelingState : IEnemyState
{
    private const float CHANNEL_DURATION = 8f;

    private GolemController owner;
    private float timer;
    private bool hasAppliedPhaseDown;

    public RevivalChannelingState(GolemController owner)
    {
        this.owner = owner;
    }

    public void OnEnter(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        timer = CHANNEL_DURATION;
        hasAppliedPhaseDown = false;

        combat.PlayAnimBool("Run", false);

        if (owner != null && owner.MyHazards != null)
        {
            GolemController.GolemPhase currentPhase = owner.CurrentPhase;
            if (currentPhase == GolemController.GolemPhase.Phase1)
            {
                // Edge case: already at lowest phase, keep Phase1 (don't disable entirely)
                owner.MyHazards.SetPhase(GolemController.GolemPhase.Phase1);
            }
            else
            {
                GolemController.GolemPhase reducedPhase = (GolemController.GolemPhase)((int)currentPhase - 1);
                owner.MyHazards.SetPhase(reducedPhase);
            }
            hasAppliedPhaseDown = true;
        }
    }

    public void OnUpdate(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        timer -= Time.deltaTime;

        Health health = ((MonoBehaviour)ctx).GetComponent<Health>();

        if (health != null && health.CurrentHealth <= 0)
        {
            if (owner != null && owner.PartnerGolem != null && owner.PartnerGolem.GetCurrentState() is ParalyzedState)
            {
                owner.HandleDuoBossDefeated();
            }
            return;
        }

        if (timer <= 0f && health != null && health.CurrentHealth > 0)
        {
            if (owner != null && owner.PartnerGolem != null)
            {
                owner.PartnerGolem.ReviveWithHP(health.CurrentHealth);
            }

            if (hasAppliedPhaseDown && owner != null && owner.MyHazards != null)
            {
                owner.MyHazards.SetPhase(owner.CurrentPhase);
            }

            ctx.SwitchTo("Idle");
        }
    }

    public void OnExit(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        timer = CHANNEL_DURATION;
        hasAppliedPhaseDown = false;
    }
}
