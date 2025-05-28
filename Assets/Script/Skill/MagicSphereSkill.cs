using UnityEngine;

public class MagicSphereSkill : SkillBase
{
    public GameObject magicSpherePrefab;
    public Transform playerTransform;

    public override void Activate(int direction)
    {
        if (!CanActivate)
            return;

        Vector3 spawmPos = playerTransform.position + new Vector3(direction, 0, 0);
        GameObject sphere = Instantiate(magicSpherePrefab, spawmPos, Quaternion.identity);
        MagicSphere magicSphere = sphere.GetComponent<MagicSphere>();
        if (magicSphere != null)
        {
            magicSphere.SetDirection(direction);
        }
        cooldownTimer = cooldown;
    }
}
