using UnityEngine;

public class Pillar : MonoBehaviour
{
    [SerializeField] private int hp = 20;
    [SerializeField] private AudioClip destroySFX;
    [SerializeField] private GameObject destroyVFX;

    private BatBossController boss;
    private Health bossHealth;
    private SpriteRenderer sprite;
    private Color originalColor;
    private int currentHP;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        currentHP = hp;
    }

    public void Init(BatBossController bossRef)
    {
        boss = bossRef;
        bossHealth = bossRef.GetComponent<Health>();

        if (sprite != null)
            originalColor = sprite.color;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var playerDamage = collision.GetComponent<PlayerNA>();
            if (playerDamage != null)
            {
                TakeHit(1);
            }

            var stand = collision.GetComponentInChildren<Stand>();
            if (stand != null)
            {
                TakeHit(1);
            }
        }

        var projectile = collision.GetComponent<ProjectilePref>();
        if (projectile != null)
        {
            TakeHit(1);
        }
    }

    public void TakeHit(int amount)
    {
        currentHP -= amount;

        if (sprite != null)
            StartCoroutine(HitFlash());

        if (currentHP <= 0)
        {
            DestroyPillar();
        }
    }

    private System.Collections.IEnumerator HitFlash()
    {
        sprite.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        sprite.color = originalColor;
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
}
