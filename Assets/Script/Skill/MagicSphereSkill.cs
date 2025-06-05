using UnityEngine;

[CreateAssetMenu(menuName = "Skill/MagicSphereSkill")]
public class MagicSphereSkill : SkillBase
{
    public GameObject magicSpherePrefab;

    public override void Activate(GameObject user, int direction)
    {
        if (!CanActivate)
            return;
        Vector3 spawnPos = user.transform.position + new Vector3(direction, 0, 0);
        GameObject sphere = Instantiate(magicSpherePrefab, spawnPos, Quaternion.identity);
        IMagicProjectile magicSphere = sphere.GetComponent<IMagicProjectile>();
        if (magicSphere != null)
        {
            magicSphere.SetDirection(direction);
        }
        cooldownTimer = cooldown;
    }
}
