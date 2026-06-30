using UnityEngine;

[CreateAssetMenu(menuName = "Skill/SpawnPrefabSkill")]
public class SpawnPrefabSkill : SpawnDamageSkillBase
{
    public AudioClip spawnSFX;
    public Vector3 offset;

    public override void Activate(GameObject user, int direction)
    {
        if (!CanActivate)
            return;

        // Null check for AudioManager
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(spawnSFX);
        }

        Vector3 spawnPos = PlayerManager.Instance.transform.position + offset;
        GameObject spawn = Instantiate(spawnPrefab, spawnPos, Quaternion.identity);

        // Try to set damage if prefab implements ISpawnPref (includes IProjectilePref)
        ISpawnPref spawnPref = spawn.GetComponent<ISpawnPref>();
        if (spawnPref != null)
        {
            spawnPref.SetDamage(CalculateFinalDamage());
        }

        cooldownTimer = cooldown;
    }
}
