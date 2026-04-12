using UnityEngine;

[CreateAssetMenu(menuName = "Skill/ProjectileSkill")]
public class ProjectileSkill : SpawnDamageSkillBase
{
    public AudioClip projectileSFX;
    public int isItNeedToFlip = 1; // Flip if the SpriteRenderer's is facing left (negative scale)
    public Vector3 spawnOffset = Vector3.zero; // Offset to adjust spawn position

    public override void Activate(GameObject user, int direction)
    {
        if (!CanActivate)
            return;

        if (spawnPrefab == null)
        {
            Debug.LogError("ProjectileSkill: projectilePrefab is not assigned in the inspector for " + name);
            return;
        }

        // Null check for AudioManager
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(projectileSFX);
        }

        Vector3 spawnPos = user.transform.position + new Vector3(direction, 0, 0) + spawnOffset;
        GameObject sphere = Instantiate(spawnPrefab, spawnPos, Quaternion.identity);

        IProjectilePref magicSphere = sphere.GetComponent<IProjectilePref>();
        if (magicSphere != null)
        {
            magicSphere.SetDirection(direction, isItNeedToFlip);
            magicSphere.SetDamage(CalculateFinalDamage());
        }
        cooldownTimer = cooldown;
    }
}
