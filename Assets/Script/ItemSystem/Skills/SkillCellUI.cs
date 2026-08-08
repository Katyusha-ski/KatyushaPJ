using UnityEngine;
using UnityEngine.UI;

public class SkillCellUI : MonoBehaviour
{
    [Header("Vị trí trong ma trận 4x5")]
    public int row;
    public int col;

    [Header("Tham chiếu UI")]
    public Image background;
    public Image icon;
    public Sprite lockedBackground;
    public Sprite unlockedBackground;

    private void Awake()
    {
        var skillUI = GetComponentInParent<SkillSystemUI>();
        if (skillUI == null)
        {
            Debug.LogWarning($"[SkillCellUI] Không tìm thấy SkillSystemUI ở GameObject cha của ô ({row},{col}).");
            return;
        }

        int index = row * 5 + col;
        if (index < 0 || index >= skillUI.cells.Length)
        {
            Debug.LogWarning($"[SkillCellUI] row/col ({row},{col}) vượt ngoài phạm vi 4x5.");
            return;
        }

        skillUI.cells[index] = new SkillSystemUI.SkillCell
        {
            background = background,
            icon = icon,
            lockedBackground = lockedBackground,
            unlockedBackground = unlockedBackground
        };
    }
}
