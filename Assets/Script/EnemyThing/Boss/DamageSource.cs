using UnityEngine;

public enum DamageSourceType
{
    Melee,
    Ranged,
    Stand,
    EnemySkill,
    Pillar,
    System,
}

public class DamageSource : MonoBehaviour
{
    public DamageSourceType sourceType;

    private static GameObject _systemSource;
    public static GameObject SystemSource
    {
        get
        {
            if (_systemSource == null)
            {
                _systemSource = new GameObject("SystemDamageSource");
                _systemSource.AddComponent<DamageSource>().sourceType = DamageSourceType.System;
                Object.DontDestroyOnLoad(_systemSource);
            }
            return _systemSource;
        }
    }
}
