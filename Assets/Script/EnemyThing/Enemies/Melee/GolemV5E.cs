using UnityEngine;

/// <summary>
/// Standard enemy controller for Golem V5.
/// Normal attacks rotate A1 -> A2 -> A3. GroundSlam has priority whenever
/// its internal cooldown is ready.
/// </summary>
public class GolemV5E : EnemyController
{
    [Header("GroundSlam")]
    [Tooltip("Cooldown between GroundSlam casts, in seconds.")]
    [SerializeField] private float groundSlamCooldown = 6f;

    [Tooltip("Bonus damage added to the normal attack damage by GroundSlam.")]
    [SerializeField] private int groundSlamBonusDamage = 5;

    private int nextNormalAttack;
    private bool groundSlamPending;
    private float nextGroundSlamTime = -Mathf.Infinity;

    protected override void Start()
    {
        base.Start();
    }

    public override void ExecuteAttack()
    {
        if (CanUseGroundSlam())
        {
            groundSlamPending = true;
            PlayAnimTrigger("GroundSlam");
            return;
        }

        groundSlamPending = false;
        string trigger = nextNormalAttack switch
        {
            0 => "A1",
            1 => "A2",
            _ => "A3"
        };

        nextNormalAttack = (nextNormalAttack + 1) % 3;
        PlayAnimTrigger(trigger);
    }

    /// <summary>Called by the GroundSlam animation event at impact.</summary>
    public void DealGroundSlamDamage()
    {
        if (!groundSlamPending)
            return;

        groundSlamPending = false;
        nextGroundSlamTime = Time.time + Mathf.Max(0f, groundSlamCooldown);

        Health selfHealth = GetComponent<Health>();
        if (selfHealth == null || selfHealth.CurrentHealth <= 0 || player == null)
            return;

        Health playerHealth = player.GetComponent<Health>();
        if (playerHealth == null || playerHealth.CurrentHealth <= 0)
            return;

        if (Vector2.Distance(transform.position, player.position) > attackRange)
            return;

        int damage = Mathf.Max(1, Mathf.CeilToInt(characterStats.Atk) + groundSlamBonusDamage);
        playerHealth.TakeDamage(damage);
    }

    private bool CanUseGroundSlam()
    {
        return Time.time >= nextGroundSlamTime;
    }
}
