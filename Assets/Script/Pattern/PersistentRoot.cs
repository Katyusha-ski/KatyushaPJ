using System.Collections.Generic;
using UnityEngine;

/// <summary>Keeps a manager cluster (e.g. CoreSystem) alive across scenes.</summary>
public class PersistentRoot : MonoBehaviour
{
    private static readonly Dictionary<string, PersistentRoot> _aliveRoots =
        new Dictionary<string, PersistentRoot>();

    [SerializeField, Tooltip("Id duy nhất cho cụm này. Để trống = dùng tên GameObject.")]
    private string rootId;

    private string Key => string.IsNullOrEmpty(rootId) ? gameObject.name : rootId;

    private void Awake()
    {
        string key = Key;

        if (_aliveRoots.TryGetValue(key, out var existing) && existing != null && existing != this)
        {
            Destroy(gameObject);
            return;
        }

        _aliveRoots[key] = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        string key = Key;
        if (_aliveRoots.TryGetValue(key, out var existing) && existing == this)
            _aliveRoots.Remove(key);
    }
}
