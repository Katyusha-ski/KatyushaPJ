using UnityEngine;

[CreateAssetMenu(menuName = "Skill/ProjectileSkill")]
public class ProjectileSkill : SkillBase
{
    public GameObject projectilePrefab;
    public AudioClip projectileSFX;
    
    public override void Activate(GameObject user, int direction)
    {
        if (!CanActivate)
            return;
            
        // Null check for AudioManager
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(projectileSFX);
        }
        
        Vector3 spawnPos = user.transform.position + new Vector3(direction, 0, 0);
        GameObject sphere = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
        IMagicProjectile magicSphere = sphere.GetComponent<IMagicProjectile>();
        if (magicSphere != null)
        {
            magicSphere.SetDirection(direction);
        }
        cooldownTimer = cooldown;
    }
}
