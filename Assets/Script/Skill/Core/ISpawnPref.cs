
public interface ISpawnPref
{
    void SetDamage(float damage);
}


public struct ProjectileConfig
{
    public float speed;
    public int pierceCount;
    public bool applyPoison;
    public int poisonDamagePerTick;
    public float poisonTickInterval;
    public float poisonDuration;
    public bool applySlow;
    public float slowAmount;
    public float slowDuration;
    public bool applyArmorDebuff;
    public float armorDebuffAmount;
    public float armorDebuffDuration;
}


public interface IProjectilePref : ISpawnPref
{
    void SetDirection(int direction, int isItNeedToFlip = 1);
    void SetProjectileConfig(ProjectileConfig config);
}
