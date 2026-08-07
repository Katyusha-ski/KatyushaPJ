using UnityEngine;

[CreateAssetMenu(menuName = "Skill/ProjectileSkill")]
public class ProjectileSkill : SpawnDamageSkillBase
{
    public AudioClip projectileSFX;
    public int isItNeedToFlip = 1;
    public Vector3 spawnOffset = Vector3.zero;

    [Header("Level-Up Upgrades")]
    public float projectileSpeed = 8f;
    public int pierceCount = 0;
    public bool applyPoison = false;
    public int poisonDamagePerTick = 0;
    public float poisonTickInterval = 1f;
    public float poisonDuration = 3f;
    public bool applySlow = false;
    [Range(0f, 1f)] public float slowAmount = 0f;
    public float slowDuration = 0f;
    public bool applyArmorDebuff = false;
    [Range(0f, 1f)] public float armorDebuffAmount = 0f;
    public float armorDebuffDuration = 0f;

    public override void Activate(GameObject user, int direction)
    {
        if (!CanActivate)
            return;

        if (spawnPrefab == null)
        {
            Debug.LogError("ProjectileSkill: projectilePrefab is not assigned in the inspector for " + name);
            return;
        }

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(projectileSFX);

        Vector3 spawnPos = user.transform.position + new Vector3(direction, 0, 0) + spawnOffset;
        GameObject sphere = Instantiate(spawnPrefab, spawnPos, Quaternion.identity);

        IProjectilePref magicSphere = sphere.GetComponent<IProjectilePref>();
        if (magicSphere != null)
        {
            magicSphere.SetDirection(direction, isItNeedToFlip);
            magicSphere.SetDamage(CalculateFinalDamage());
            magicSphere.SetProjectileConfig(new ProjectileConfig
            {
                speed = projectileSpeed,
                pierceCount = pierceCount,
                applyPoison = applyPoison,
                poisonDamagePerTick = poisonDamagePerTick,
                poisonTickInterval = poisonTickInterval,
                poisonDuration = poisonDuration,
                applySlow = applySlow,
                slowAmount = slowAmount,
                slowDuration = slowDuration,
                applyArmorDebuff = applyArmorDebuff,
                armorDebuffAmount = armorDebuffAmount,
                armorDebuffDuration = armorDebuffDuration
            });
        }
        cooldownTimer = cooldown;
    }
}
