using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Bản ghi trạng thái của 1 scene (đi vào SaveData.sceneStates).
/// Key là tên object (quy ước unique trong scene).
/// </summary>
[System.Serializable]
public class ObjectState
{
    public string name;
    public bool active;
}

/// <summary>
/// Trạng thái đã đổi của 1 scene so với file .unity gốc.
/// Chỉ level scene được persist — boss scene (nhận biết qua
/// WaveBossController) luôn load tươi, không track.
/// </summary>
[System.Serializable]
public class SceneStateRecord
{
    public string sceneName;
    public List<string> deadEnemies = new List<string>();
    public List<ObjectState> objectStates = new List<ObjectState>();
    public List<string> firedTriggers = new List<string>();
}

/// <summary>
/// Giữ trạng thái scene Hyvoria ⇄ boss: quái chết, đồ ẩn/hiện, trigger 1-lần.
/// Tự tạo khi hook đầu tiên gọi tới (không cần đặt trong scene).
/// Nguồn chân lý khi chạy; SaveData.sceneStates là bản persist của nó.
/// </summary>
public class SceneStateTracker : Singleton<SceneStateTracker>
{
    private readonly Dictionary<string, SceneStateRecord> states = new Dictionary<string, SceneStateRecord>();
    private readonly HashSet<Health> tracked = new HashSet<Health>();

    // ------------------------------------------------------------------
    // Static facade (hook gọi, tự EnsureExists)
    // ------------------------------------------------------------------
    public static void RecordTriggerFired(string scene, string triggerName)
    {
        if (string.IsNullOrEmpty(scene) || string.IsNullOrEmpty(triggerName)) return;
        EnsureExists().RecordTrigger(scene, triggerName);
    }

    public static bool WasTriggerFired(string scene, string triggerName)
    {
        if (string.IsNullOrEmpty(scene) || string.IsNullOrEmpty(triggerName)) return false;
        var t = EnsureExists();
        return t.states.TryGetValue(scene, out var rec) && rec != null
            && rec.firedTriggers != null && rec.firedTriggers.Contains(triggerName);
    }

    public static void RecordObjectState(string scene, string objectName, bool active)
    {
        if (string.IsNullOrEmpty(scene) || string.IsNullOrEmpty(objectName)) return;
        EnsureExists().RecordObject(scene, objectName, active);
    }

    public static void RecordEnemyDead(string scene, string enemyName)
    {
        if (string.IsNullOrEmpty(scene) || string.IsNullOrEmpty(enemyName)) return;
        var rec = EnsureExists().GetOrCreate(scene);
        if (!rec.deadEnemies.Contains(enemyName))
            rec.deadEnemies.Add(enemyName);
    }

    public static List<SceneStateRecord> ExportStates()
    {
        return new List<SceneStateRecord>(EnsureExists().states.Values);
    }

    public static void ImportStates(List<SceneStateRecord> list)
    {
        var t = EnsureExists();
        t.states.Clear();
        if (list == null) return;
        foreach (var r in list)
        {
            if (r != null && !string.IsNullOrEmpty(r.sceneName))
                t.states[r.sceneName] = r;
        }
    }

    public static void ClearAllStates()
    {
        EnsureExists().states.Clear();
    }

    public static void RefreshForScene(Scene scene)
    {
        EnsureExists().Refresh(scene);
    }

    private static SceneStateTracker EnsureExists()
    {
        if (Instance != null) return Instance;
        var go = new GameObject("SceneStateTracker");
        var tracker = go.AddComponent<SceneStateTracker>();
        // Vừa tạo giữa scene đang chơi (VD bấm Play thẳng trong Hyvoria):
        // apply + track ngay cho scene hiện tại.
        tracker.Refresh(SceneManager.GetActiveScene());
        return tracker;
    }

    // ------------------------------------------------------------------
    // Lifecycle
    // ------------------------------------------------------------------
    protected override void OnSingletonAwake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    protected override void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
        base.OnDestroy();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Refresh(scene);
    }

    // ------------------------------------------------------------------
    // Core
    // ------------------------------------------------------------------
    public void Refresh(Scene scene)
    {
        if (!scene.IsValid()) return;
        // File là chân lý duy nhất: nạp trước rồi mới apply.
        // RAM live mà chưa kịp save thì coi như chưa từng xảy ra.
        // Chưa từng save (không có file) thì giữ RAM hiện tại.
        if (SaveManager.HasSaveFile())
        {
            SaveData data = SaveManager.LoadGame();
            if (data != null && data.sceneStates != null)
                ImportStates(data.sceneStates);
        }
        if (IsBossScene(scene)) return;
        ApplyState(scene);
        TrackEnemies(scene);
    }

    // Boss scene = chứa WaveBossController: luôn tươi, không track gì cả.
    // Check trong đúng scene đích (không check global — lúc chuyển scene,
    // object scene cũ có thể chưa unload xong).
    private static bool IsBossScene(Scene scene)
    {
        if (!scene.IsValid()) return false;
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            if (root.GetComponentInChildren<WaveBossController>(true) != null)
                return true;
        }
        return false;
    }

    private SceneStateRecord GetOrCreate(string scene)
    {
        if (!states.TryGetValue(scene, out var rec) || rec == null)
        {
            rec = new SceneStateRecord { sceneName = scene };
            states[scene] = rec;
        }
        return rec;
    }

    private void RecordTrigger(string scene, string triggerName)
    {
        var rec = GetOrCreate(scene);
        if (!rec.firedTriggers.Contains(triggerName))
            rec.firedTriggers.Add(triggerName);
    }

    private void RecordObject(string scene, string objectName, bool active)
    {
        var rec = GetOrCreate(scene);
        foreach (var o in rec.objectStates)
        {
            if (o != null && o.name == objectName)
            {
                o.active = active;
                return;
            }
        }
        rec.objectStates.Add(new ObjectState { name = objectName, active = active });
    }

    // Apply trước khi arena/trigger chạy (handler này chạy trước Start):
    // object ẩn/hiện lại, quái đã chết thì xóa ngay.
    private void ApplyState(Scene scene)
    {
        if (!states.TryGetValue(scene.name, out var rec) || rec == null) return;

        foreach (var o in rec.objectStates)
        {
            if (o == null || string.IsNullOrEmpty(o.name)) continue;
            GameObject go = FindInScene(scene, o.name);
            if (go != null && go.activeSelf != o.active)
                go.SetActive(o.active);
        }

        foreach (string name in rec.deadEnemies)
        {
            if (string.IsNullOrEmpty(name)) continue;
            GameObject go = FindInScene(scene, name);
            if (go != null)
                Object.Destroy(go);
        }
    }

    // Tự subscribe mọi Health tag Enemy trong scene — thêm/bớt quái sau này
    // không cần sửa code.
    private void TrackEnemies(Scene scene)
    {
        foreach (Health h in tracked)
        {
            if (h != null)
                h.OnDied -= OnEnemyDied;
        }
        tracked.Clear();

        foreach (Health h in FindObjectsByType<Health>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (h == null || h.gameObject.scene != scene) continue;
            if (!h.CompareTag("Enemy")) continue;
            if (tracked.Add(h))
                h.OnDied += OnEnemyDied;
        }
    }

    private void OnEnemyDied(Health h)
    {
        if (h == null) return;
        h.OnDied -= OnEnemyDied;
        tracked.Remove(h);
        RecordEnemyDead(h.gameObject.scene.name, h.gameObject.name);
    }

    private static GameObject FindInScene(Scene scene, string name)
    {
        if (!scene.IsValid() || string.IsNullOrEmpty(name)) return null;
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            Transform found = FindRecursive(root.transform, name);
            if (found != null) return found.gameObject;
        }
        return null;
    }

    private static Transform FindRecursive(Transform t, string name)
    {
        if (t.name == name) return t;
        for (int i = 0; i < t.childCount; i++)
        {
            Transform found = FindRecursive(t.GetChild(i), name);
            if (found != null) return found;
        }
        return null;
    }
}
