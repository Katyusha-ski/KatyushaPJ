using UnityEngine;
using UnityEngine.UI;

public class SkillSystemUI : MonoBehaviour
{
    [System.Serializable]
    public class SkillCell
    {
        public Image background;
        public Image icon;
        public Sprite lockedBackground;
        public Sprite unlockedBackground;
        public Sprite skillIcon;
        public bool isUnlocked;
    }

    private const int Rows = 4;
    private const int Cols = 5;

    public SkillCell[] cells = new SkillCell[Rows * Cols];

    private static int ToIndex(int row, int col) => row * Cols + col;

    private void OnEnable()
    {
        if (Inventory.Instance != null)
            Inventory.Instance.OnSkillMatrixChanged += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        if (Inventory.Instance != null)
            Inventory.Instance.OnSkillMatrixChanged -= Refresh;
    }

    public void Refresh()
    {
        if (Inventory.Instance == null) return;
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Cols; c++)
            {
                bool unlocked = Inventory.Instance.IsSkillUnlocked(r, c);
                SetUnlocked(r, c, unlocked);
            }
        }
    }

    public void SetUnlocked(int row, int col, bool unlocked)
    {
        if (row < 0 || row >= Rows || col < 0 || col >= Cols) return;
        int index = ToIndex(row, col);
        if (cells[index] == null) return;
        cells[index].isUnlocked = unlocked;
        ApplyCellState(index);
    }

    private void ApplyCellState(int index)
    {
        var cell = cells[index];
        if (cell.background != null)
        {
            cell.background.sprite = cell.isUnlocked ? cell.unlockedBackground : cell.lockedBackground;
        }
        if (cell.icon != null)
        {
            cell.icon.sprite = cell.skillIcon;
            cell.icon.enabled = cell.isUnlocked;
            cell.icon.gameObject.SetActive(cell.isUnlocked);
        }
    }
}
