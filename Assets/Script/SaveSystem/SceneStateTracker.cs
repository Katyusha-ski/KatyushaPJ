using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// NHẬT KÝ LỖI (đọc trước khi sửa Refresh/ImportStates):
// - Triệu chứng: trigger một lần (VD BossSceneTrigger) phát lại sau load;
//   quái đã giết hồi sinh khi backtrack (farm coin vô hạn).
// - Cơ chế: Refresh() chạy mọi lần load scene, gọi ImportStates mà bản cũ
//   Clear() RAM trước rồi mới nạp FILE. Mọi tiến trình xảy ra sau lần save
//   gần nhất (fire trigger, giết quái) đều bị xóa khỏi RAM ngay khi sang
//   scene mới. Tệ hơn: arrival-save chạy 2 frame SAU Refresh nên nó trung
//   thành ghi đè bản RAM đã trống vào file -> mất vĩnh viễn, không chỉ 1 phiên.
// - Fix: ImportStates hợp nhất thay vì thay thế. firedTriggers/deadEnemies
//   union (tiến trình đơn điệu không bao giờ mất); objectStates lấy theo file
//   (xung đột true/false thì file thắng). NewGame/Credits gọi ClearAllStates
//   trực tiếp nên reset vẫn đúng, không ảnh hưởng.

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
    private readonly HashSet<Health> trackedEnemies = new HashSet<Health>();

    // ------------------------------------------------------------------
    // Static API: query
    // ------------------------------------------------------------------
    public static bool WasTriggerFired(string scene, string triggerName)
    {
        if (string.IsNullOrEmpty(scene) || string.IsNullOrEmpty(triggerName)) return false;
        var tracker = EnsureExists();
        return tracker.states.TryGetValue(scene, out var record) && record != null
            && record.firedTriggers != null && record.firedTriggers.Contains(triggerName);
    }

    // ------------------------------------------------------------------
    // Static API: record
    // ------------------------------------------------------------------
    public static void RecordTriggerFired(string scene, string triggerName)
    {
        if (string.IsNullOrEmpty(scene) || string.IsNullOrEmpty(triggerName)) return;
        EnsureExists().RecordTrigger(scene, triggerName);
    }

    public static void RecordObjectState(string scene, string objectName, bool active)
    {
        if (string.IsNullOrEmpty(scene) || string.IsNullOrEmpty(objectName)) return;
        EnsureExists().RecordObject(scene, objectName, active);
    }

    public static void RecordEnemyDead(string scene, string enemyName)
    {
        if (string.IsNullOrEmpty(scene) || string.IsNullOrEmpty(enemyName)) return;
        var record = EnsureExists().GetOrCreate(scene);
        if (!record.deadEnemies.Contains(enemyName))
            record.deadEnemies.Add(enemyName);
    }

    // ------------------------------------------------------------------
    // Static API: persist (RAM <-> file)
    // ------------------------------------------------------------------
    public static List<SceneStateRecord> ExportStates()
    {
        return new List<SceneStateRecord>(EnsureExists().states.Values);
    }

    public static void ImportStates(List<SceneStateRecord> list)
    {
        var tracker = EnsureExists();
        if (list == null) return;
        foreach (var record in list)
        {
            if (record == null || string.IsNullOrEmpty(record.sceneName)) continue;
            if (!tracker.states.TryGetValue(record.sceneName, out var current) || current == null)
            {
                tracker.states[record.sceneName] = record;
                continue;
            }
            foreach (var deadName in record.deadEnemies)
            {
                if (!string.IsNullOrEmpty(deadName) && !current.deadEnemies.Contains(deadName))
                    current.deadEnemies.Add(deadName);
            }
            foreach (var trigger in record.firedTriggers)
            {
                if (!string.IsNullOrEmpty(trigger) && !current.firedTriggers.Contains(trigger))
                    current.firedTriggers.Add(trigger);
            }
            foreach (var objState in record.objectStates)
            {
                if (objState == null || string.IsNullOrEmpty(objState.name)) continue;
                var existing = current.objectStates.Find(candidate => candidate != null && candidate.name == objState.name);
                if (existing != null) existing.active = objState.active;
                else current.objectStates.Add(objState);
            }
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
        // Hợp nhất file vào RAM (union): tiến trình live không bao giờ mất
        // vì load; thiếu file thì giữ nguyên RAM hiện tại.
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

    // Apply trước khi arena/trigger chạy (handler này chạy trước Start):
    // object ẩn/hiện lại, quái đã chết thì xóa ngay.
    private void ApplyState(Scene scene)
    {
        if (!states.TryGetValue(scene.name, out var record) || record == null) return;

        foreach (var objState in record.objectStates)
        {
            if (objState == null || string.IsNullOrEmpty(objState.name)) continue;
            GameObject go = FindInScene(scene, objState.name);
            if (go != null && go.activeSelf != objState.active)
                go.SetActive(objState.active);
        }

        foreach (string deadName in record.deadEnemies)
        {
            if (string.IsNullOrEmpty(deadName)) continue;
            GameObject go = FindInScene(scene, deadName);
            if (go != null)
                Object.Destroy(go);
        }
    }

    // Tự subscribe mọi Health tag Enemy trong scene — thêm/bớt quái sau này
    // không cần sửa code.
    private void TrackEnemies(Scene scene)
    {
        foreach (Health enemyHealth in trackedEnemies)
        {
            if (enemyHealth != null)
                enemyHealth.OnDied -= OnEnemyDied;
        }
        trackedEnemies.Clear();

        foreach (Health enemyHealth in FindObjectsByType<Health>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (enemyHealth == null || enemyHealth.gameObject.scene != scene) continue;
            if (!enemyHealth.CompareTag("Enemy")) continue;
            if (trackedEnemies.Add(enemyHealth))
                enemyHealth.OnDied += OnEnemyDied;
        }
    }

    private void OnEnemyDied(Health enemyHealth)
    {
        if (enemyHealth == null) return;
        enemyHealth.OnDied -= OnEnemyDied;
        trackedEnemies.Remove(enemyHealth);
        RecordEnemyDead(enemyHealth.gameObject.scene.name, enemyHealth.gameObject.name);
    }

    private SceneStateRecord GetOrCreate(string scene)
    {
        if (!states.TryGetValue(scene, out var record) || record == null)
        {
            record = new SceneStateRecord { sceneName = scene };
            states[scene] = record;
        }
        return record;
    }

    private void RecordTrigger(string scene, string triggerName)
    {
        var record = GetOrCreate(scene);
        if (!record.firedTriggers.Contains(triggerName))
            record.firedTriggers.Add(triggerName);
    }

    private void RecordObject(string scene, string objectName, bool active)
    {
        var record = GetOrCreate(scene);
        foreach (var objState in record.objectStates)
        {
            if (objState != null && objState.name == objectName)
            {
                objState.active = active;
                return;
            }
        }
        record.objectStates.Add(new ObjectState { name = objectName, active = active });
    }

    private static GameObject FindInScene(Scene scene, string targetName)
    {
        if (!scene.IsValid() || string.IsNullOrEmpty(targetName)) return null;
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            Transform found = FindRecursive(root.transform, targetName);
            if (found != null) return found.gameObject;
        }
        return null;
    }

    private static Transform FindRecursive(Transform current, string targetName)
    {
        if (current.name == targetName) return current;
        for (int i = 0; i < current.childCount; i++)
        {
            Transform found = FindRecursive(current.GetChild(i), targetName);
            if (found != null) return found;
        }
        return null;
    }
}
