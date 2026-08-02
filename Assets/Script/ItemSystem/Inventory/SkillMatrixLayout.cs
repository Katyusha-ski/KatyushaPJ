using UnityEngine;

[CreateAssetMenu(fileName = "SkillMatrixLayout", menuName = "Katyusha/Skill Matrix Layout")]
public class SkillMatrixLayout : ScriptableObject
{
    private const int Rows = 4;
    private const int Cols = 5;

    // 20 ô, thứ tự phẳng: index = row * 5 + col.
    // row: 0=Range, 1=Dash, 2=Defend, 3=Melee (khớp SkillType trong ItemData.cs).
    // col: 0=Lv1 ... 4=Lv5.
    [Tooltip("Kéo đúng 20 ItemData (loại Skill) vào đây theo thứ tự row*5+col")]
    public ItemData[] skillItems = new ItemData[Rows * Cols];

    public ItemData GetItemAt(int row, int col)
    {
        int index = row * Cols + col;
        if (index < 0 || index >= skillItems.Length) return null;
        return skillItems[index];
    }
}
