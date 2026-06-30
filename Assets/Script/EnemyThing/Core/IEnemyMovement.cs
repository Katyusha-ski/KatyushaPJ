using UnityEngine;

public interface IEnemyMovement
{
    void Patrol();
    void LookAtPlayer();
    void MoveTowardPlayer();
    void Pursue();
    void RetreatFromPlayer();
    void SetDirection(int dir);
    int GetDirection();
    float GetDistanceToPlayer();
    float GetVisionRange();
}
