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

    public SkillCell[,] cells = new SkillCell[4, 5];

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
        var matrix = Inventory.Instance.skillMatrix;
        for (int r = 0; r < 4; r++)
        {
            for (int c = 0; c < 5; c++)
            {
                bool unlocked = matrix[r, c] != null && matrix[r, c].item != null;
                SetUnlocked(r, c, unlocked);
            }
        }
    }

    public void SetUnlocked(int row, int col, bool unlocked)
    {
        if (row < 0 || row >= 4 || col < 0 || col >= 5 || cells[row, col] == null) return;
        cells[row, col].isUnlocked = unlocked;
        ApplyCellState(row, col);
    }

    private void ApplyCellState(int row, int col)
    {
        var cell = cells[row, col];
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
