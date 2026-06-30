using UnityEngine;

public class NightBornE : EnemyController
{
    [Header("Death Explosion")]
    [SerializeField] private GameObject deathExplosionPrefab;

    public override void HandleEnemyDeath()
    {
        base.HandleEnemyDeath();

        if (deathExplosionPrefab != null)
        {
            Instantiate(deathExplosionPrefab, transform.position, Quaternion.identity);
        }
    }
}
