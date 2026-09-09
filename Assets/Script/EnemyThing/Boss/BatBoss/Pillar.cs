using UnityEngine;

public class Pillar : MonoBehaviour
{
    [SerializeField] private AudioClip destroySFX;
    [SerializeField] private GameObject destroyVFX;

    private BatBossController boss;
    private Health bossHealth;
    private Health health;

    private void Awake()
    {
        health = GetComponent<Health>();
        if (health != null)
            health.OnDied += HandleDeath;
        else
            Debug.LogError("Pillar requires a Health component.", this);
    }

    public void Init(BatBossController bossRef)
    {
        boss = bossRef;
        bossHealth = bossRef.GetComponent<Health>();

    }

    private void HandleDeath(Health deadHealth)
    {
        DestroyPillar();
    }

    private void DestroyPillar()
    {
        if (destroySFX != null && AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(destroySFX);

        if (destroyVFX != null)
            Instantiate(destroyVFX, transform.position, Quaternion.identity);

        if (bossHealth != null && boss != null)
        {
            DamageSource src = gameObject.AddComponent<DamageSource>();
            src.sourceType = DamageSourceType.Pillar;
            bossHealth.TakeDamage(boss.PillarBurstDamage, gameObject);
        }

        Destroy(gameObject);
    }

    public void DestroyOnBossDefeated()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (health != null)
            health.OnDied -= HandleDeath;
    }
}
