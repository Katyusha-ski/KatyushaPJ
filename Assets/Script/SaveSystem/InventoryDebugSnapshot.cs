using System.Collections.Generic;

/// <summary>
/// Snapshot JSON dùng riêng cho debug inventory trong Editor.
/// Không dùng chung với SaveData (save game thật) để tránh lẫn lộn 2 mục đích.
///
/// Lưu ý: skill unlock state là bool[20] phẳng (index = row*5+col) — lưu trực tiếp
/// thành List<bool> nên JsonUtility serialize được bình thường, không cần flatten
/// 2D như cấu trúc ItemStack[,] cũ.
/// </summary>
[System.Serializable]
public class InventoryDebugSnapshot
{
    public List<SerializableItemStack> inventoryItem = new List<SerializableItemStack>();
    public List<SerializableItemStack> equipmentItem = new List<SerializableItemStack>();

    public List<bool> skillUnlocked = new List<bool>(new bool[20]);

    public List<string> questItems = new List<string>();
}
