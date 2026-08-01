#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// Tool debug: lưu/load snapshot inventory ra 1 file JSON riêng (không phải file save game),
/// và TỰ ĐỘNG load lại snapshot đó mỗi khi bấm Play trong Editor.
///
/// Cách dùng:
/// 1. Bấm Play, tự tay add item vào inventory cho đúng ý muốn test.
/// 2. Menu Tools > Debug > Inventory > Save Snapshot  -> lưu state hiện tại ra JSON.
/// 3. Những lần Play sau, script này tự động load lại JSON đó vào Inventory.Instance,
///    không cần add tay nữa.
/// 4. Muốn đổi bộ item test khác: chỉnh inventory trong lúc Play rồi Save Snapshot lại,
///    hoặc Delete Snapshot để quay về trạng thái mặc định (item rỗng).
///
/// File nằm ngoài thư mục Assets (project/DebugData/inventory_debug.json) nên Unity
/// không import nó thành asset, và toàn bộ code này bị loại khỏi build thật (UNITY_EDITOR).
/// </summary>
public static class InventoryDebugTool
{
    private static readonly string SnapshotPath =
        Path.Combine(Application.dataPath, "../DebugData/inventory_debug.json");

    // ------------------------------------------------------------------
    // Auto-load mỗi khi bấm Play trong Editor
    // ------------------------------------------------------------------
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AutoLoadOnPlay()
    {
        if (Inventory.Instance == null)
        {
            Debug.LogWarning("[InventoryDebugTool] Không tìm thấy Inventory.Instance, bỏ qua auto-load.");
            return;
        }

        LoadSnapshot();
    }

    // ------------------------------------------------------------------
    // Menu items
    // ------------------------------------------------------------------
    [MenuItem("Tools/Debug/Inventory/Save Snapshot")]
    private static void SaveSnapshotMenuItem()
    {
        if (!EditorApplication.isPlaying)
        {
            Debug.LogWarning("[InventoryDebugTool] Phải ở Play Mode mới lưu được snapshot (cần Inventory.Instance).");
            return;
        }
        SaveSnapshot();
    }

    [MenuItem("Tools/Debug/Inventory/Load Snapshot")]
    private static void LoadSnapshotMenuItem()
    {
        if (!EditorApplication.isPlaying)
        {
            Debug.LogWarning("[InventoryDebugTool] Phải ở Play Mode mới load được snapshot (cần Inventory.Instance).");
            return;
        }
        LoadSnapshot();
    }

    [MenuItem("Tools/Debug/Inventory/Delete Snapshot")]
    private static void DeleteSnapshotMenuItem()
    {
        if (File.Exists(SnapshotPath))
        {
            File.Delete(SnapshotPath);
            Debug.Log($"[InventoryDebugTool] Đã xoá snapshot: {SnapshotPath}");
        }
        else
        {
            Debug.LogWarning("[InventoryDebugTool] Không có snapshot nào để xoá.");
        }
    }

    [MenuItem("Tools/Debug/Inventory/Open Snapshot Folder")]
    private static void OpenSnapshotFolderMenuItem()
    {
        string folder = Path.GetDirectoryName(SnapshotPath);
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);
        EditorUtility.RevealInFinder(folder);
    }

    // ------------------------------------------------------------------
    // Core logic
    // ------------------------------------------------------------------
    public static void SaveSnapshot()
    {
        Inventory inventory = Inventory.Instance;
        if (inventory == null)
        {
            Debug.LogWarning("[InventoryDebugTool] Không tìm thấy Inventory.Instance.");
            return;
        }

        InventoryDebugSnapshot snapshot = new InventoryDebugSnapshot
        {
            inventoryItem = inventory.GetSerializableInventory(),
            equipmentItem = inventory.GetSerializableEquipment(),
            questItems = inventory.GetSerializableQuestItems()
        };

        // Flatten skillMatrix 2D -> 1D vì JsonUtility không hỗ trợ List<List<T>>
        List<List<SerializableItemStack>> skillMatrix2D = inventory.GetSerializableSkillMatrix();
        snapshot.skillRows = skillMatrix2D.Count;
        snapshot.skillCols = skillMatrix2D.Count > 0 ? skillMatrix2D[0].Count : 0;
        snapshot.skillMatrixFlat = new List<SerializableItemStack>();
        foreach (List<SerializableItemStack> row in skillMatrix2D)
        {
            foreach (SerializableItemStack cell in row)
                snapshot.skillMatrixFlat.Add(cell);
        }

        string json = JsonUtility.ToJson(snapshot, true);

        string folder = Path.GetDirectoryName(SnapshotPath);
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        try
        {
            File.WriteAllText(SnapshotPath, json);
            Debug.Log($"[InventoryDebugTool] Đã lưu inventory debug snapshot: {SnapshotPath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[InventoryDebugTool] Lưu snapshot thất bại: {e.Message}");
        }
    }

    public static void LoadSnapshot()
    {
        if (!File.Exists(SnapshotPath))
        {
            Debug.LogWarning($"[InventoryDebugTool] Chưa có file snapshot ({SnapshotPath}). Save Snapshot trước đã.");
            return;
        }

        Inventory inventory = Inventory.Instance;
        if (inventory == null)
        {
            Debug.LogWarning("[InventoryDebugTool] Không tìm thấy Inventory.Instance.");
            return;
        }

        string json;
        try
        {
            json = File.ReadAllText(SnapshotPath);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[InventoryDebugTool] Đọc file snapshot thất bại: {e.Message}");
            return;
        }

        if (string.IsNullOrWhiteSpace(json))
        {
            Debug.LogWarning("[InventoryDebugTool] File snapshot rỗng.");
            return;
        }

        InventoryDebugSnapshot snapshot;
        try
        {
            snapshot = JsonUtility.FromJson<InventoryDebugSnapshot>(json);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[InventoryDebugTool] Parse JSON thất bại: {e.Message}");
            return;
        }

        inventory.LoadSerializableInventory(snapshot.inventoryItem);
        inventory.LoadSerializableEquipment(snapshot.equipmentItem);
        inventory.LoadSerializableQuestItems(snapshot.questItems);

        // Dựng lại skillMatrix 2D từ flat list
        if (snapshot.skillMatrixFlat != null && snapshot.skillRows > 0 && snapshot.skillCols > 0)
        {
            List<List<SerializableItemStack>> matrix2D = new List<List<SerializableItemStack>>();
            for (int r = 0; r < snapshot.skillRows; r++)
            {
                List<SerializableItemStack> row = new List<SerializableItemStack>();
                for (int c = 0; c < snapshot.skillCols; c++)
                {
                    int flatIndex = r * snapshot.skillCols + c;
                    row.Add(flatIndex < snapshot.skillMatrixFlat.Count ? snapshot.skillMatrixFlat[flatIndex] : null);
                }
                matrix2D.Add(row);
            }
            inventory.LoadSerializableSkillMatrix(matrix2D);
        }

        Debug.Log($"[InventoryDebugTool] Đã load inventory debug snapshot: {SnapshotPath}");
    }
}
#endif
