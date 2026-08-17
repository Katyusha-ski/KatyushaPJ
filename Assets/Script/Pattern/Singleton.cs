using UnityEngine;

/// <summary>
/// Base class cho mọi singleton trong game.
/// - Tự dedup: khi đã có Instance thì bản xuất hiện sau bị Destroy.
/// - Mặc định DontDestroyOnLoad; override PersistAcrossScenes = false nếu chỉ sống trong scene.
/// - Ghi đè OnSingletonAwake() để khởi tạo khi bản này trở thành Instance duy nhất.
/// </summary>
public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    public static T Instance { get; private set; }

    /// <summary>Có giữ xuyên scene (DontDestroyOnLoad) hay không.</summary>
    protected virtual bool PersistAcrossScenes => true;

    protected virtual void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = (T)this;

        if (PersistAcrossScenes && gameObject.scene.name != "DontDestroyOnLoad")
            DontDestroyOnLoad(gameObject);

        OnSingletonAwake();
    }

    /// <summary>Chỉ gọi khi bản này trở thành Instance duy nhất.</summary>
    protected virtual void OnSingletonAwake() { }

    protected virtual void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}