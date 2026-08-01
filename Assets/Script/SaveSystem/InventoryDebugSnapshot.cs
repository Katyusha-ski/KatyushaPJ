using System.Collections.Generic;

/// <summary>
/// Snapshot JSON dùng riêng cho debug inventory trong Editor.
/// Không dùng chung với SaveData (save game thật) để tránh lẫn lộn 2 mục đích.
///
/// Lưu ý: skillMatrix gốc là ItemStack[4,5] -> Inventory.GetSerializableSkillMatrix()
/// trả về List<List<SerializableItemStack>>. JsonUtility KHÔNG serialize được
/// list-lồng-list (nested generic collection) nên ở đây flatten thành 1 list phẳng
/// (row * skillCols + col) rồi dựng lại 2D khi load.
/// (Đây cũng chính là nguyên nhân của TODO "SkillMatrix save serialization structure
/// needs review" trong SaveData/SaveManager hiện tại của bạn.)
/// </summary>
[System.Serializable]
public class InventoryDebugSnapshot
{
    public List<SerializableItemStack> inventoryItem = new List<SerializableItemStack>();
    public List<SerializableItemStack> equipmentItem = new List<SerializableItemStack>();

    public int skillRows;
    public int skillCols;
    public List<SerializableItemStack> skillMatrixFlat = new List<SerializableItemStack>();

    public List<string> questItems = new List<string>();
}
