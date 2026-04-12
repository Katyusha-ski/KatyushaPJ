
public interface ISpawnPref
{
    void SetDamage(float damage);
}


public interface IProjectilePref : ISpawnPref
{
    void SetDirection(int direction, int isItNeedToFlip = 1);
    
}
