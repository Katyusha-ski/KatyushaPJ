using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/DefendSkill")]
public class DefendSkill : SkillBase
{
    [Header("Defend Stats")]
    public int shieldAmount = 10;
    public float defendDuration = 2f;

    [Header("Level-Up Upgrades")]
    public bool canMove = false;
    public bool reflectDamage = false;
    public bool unstoppable = false;
    public float moveSpeedMultiplier = 0.5f;
    public float reflectTime = 0.5f;
    public float reflectRange = 3f;
    public float reflectDamageAmount = 5;

    public AudioClip defendSFX;
    private const string SPEED_MOD_SOURCE = "DefendSkill";

    public override void Activate(GameObject user, int direction)
    {
        if (!CanActivate)
            return;

        cooldownTimer = cooldown;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(defendSFX);

        user.GetComponent<MonoBehaviour>().StartCoroutine(DefendRoutine(user));
    }

    private IEnumerator DefendRoutine(GameObject user)
    {
        var health = user.GetComponent<Health>();
        var stats = user.GetComponent<CharacterStats>();

        // Apply shield
        if (health != null)
            health.AddShield(shieldAmount);

        // Root or slow movement via modifier
        StatsModifier speedMod = null;
        if (stats != null)
        {
            float modValue = canMove ? -(1f - moveSpeedMultiplier) : -1f;
            speedMod = new StatsModifier(modValue, ModifierType.Multiplicative, SPEED_MOD_SOURCE);
            stats.AddMovementSpeedModifier(speedMod);
        }

        // Subscribe reflect during the initial window
        System.Action<int> reflectHandler = null;
        if (reflectDamage && health != null)
        {
            float reflectDmgMultiplier = 1 + Mathf.Clamp01(characterStats.SkillAmp / 100f);
            reflectHandler = (int _) =>
            {
                Collider2D[] hits = Physics2D.OverlapCircleAll(user.transform.position, reflectRange, LayerMask.GetMask("Enemy"));
                foreach (var hit in hits)
                {
                    var enemyHealth = hit.GetComponent<Health>();
                    if (enemyHealth != null)
                        enemyHealth.TakeDamage((int)(reflectDamageAmount * reflectDmgMultiplier));
                }
            };
            health.OnDamaged += reflectHandler;
        }

        // Wait for reflect window then unsubscribe
        if (reflectDamage && health != null && reflectHandler != null)
        {
            yield return new WaitForSeconds(reflectTime);
            health.OnDamaged -= reflectHandler;
        }

        // Wait remaining duration
        if (reflectDamage && health != null && reflectHandler != null)
            yield return new WaitForSeconds(defendDuration - reflectTime);
        else
            yield return new WaitForSeconds(defendDuration);

        // Cleanup
        if (health != null)
            health.SetShield(0);

        if (stats != null && speedMod != null)
            stats.RemoveMovementSpeedModifier(speedMod);
    }
}
