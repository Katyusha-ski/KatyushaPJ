using UnityEngine;

[CreateAssetMenu(menuName = "Skill/SpawnPrefabSkill")]
public class SpawnPrefabSkill : SkillBase
{
    public GameObject spawnPrefab;
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
        cooldownTimer = cooldown;
    }
}
