using UnityEngine;

[CreateAssetMenu(menuName = "Skill/ProjectileSkill")]
public class ProjectileSkill : SkillBase
{
    public GameObject projectilePrefab;
    public AudioClip projectileSFX;
    public int isItNeedToFlip = 1; // Flip if the SpriteRenderer's is facing left (negative scale)
    public Vector3 spawnOffset = Vector3.zero; // Offset to adjust spawn position

    public override void Activate(GameObject user, int direction)
    {
        if (!CanActivate)
            return;

        if (projectilePrefab == null)
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
        GameObject sphere = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

        IMagicProjectile magicSphere = sphere.GetComponent<IMagicProjectile>();
        if (magicSphere != null)
        {
            magicSphere.SetDirection(direction, isItNeedToFlip);
        }
        cooldownTimer = cooldown;
    }
}
