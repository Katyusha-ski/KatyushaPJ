using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Giữ cả cụm manager (vd. CoreSystem) sống xuyên scene.
///
/// VÌ SAO CẦN: DontDestroyOnLoad chỉ có tác dụng trên root GameObject.
/// Các Singleton con (GameManager, Inventory...) nằm lồng dưới root nên gọi
/// DDOL thất bại (warn + no-op) và chết theo scene cũ khi unload.
/// Root này dedup + DDOL cả cụm: load đầu tiên được bê sang scene
/// DontDestroyOnLoad, các scene sau root trùng tự destroy, singleton con
/// tự dedup như cũ (kết quả giống nhau bất kể thứ tự Awake cha/con vì
/// Destroy() là deferred).
/// </summary>
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

        // Đã có cụm này sống (từ scene trước) -> cụm mới tự hủy nguyên cụm.
        // So sánh null kiểu Unity (object destroyed == null) để chịu được
        // trường hợp tắt/mở Play Mode không reload domain trong Editor.
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
        // Cụm trùng bị hủy cuối frame KHÔNG được xóa id (bản gốc vẫn giữ).
        string key = Key;
        if (_aliveRoots.TryGetValue(key, out var existing) && existing == this)
            _aliveRoots.Remove(key);
    }
}
