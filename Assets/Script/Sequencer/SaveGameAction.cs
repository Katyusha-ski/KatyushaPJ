using System.Collections;
using UnityEngine;

/// <summary>
/// Lưu game ngay trong cutscene (thường đặt làm action ĐẦU TIÊN).
/// Đảm bảo save luôn chạy trước mọi action khác (dialogue/teleport), nên
/// load lại sẽ spawn đúng vị trí trước cutscene và trigger phát lại đầy đủ.
/// </summary>
[System.Serializable]
public class SaveGameAction : SequenceAction
{
    public override IEnumerator Execute()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.SaveGame();
        else
            Debug.LogWarning("[SaveGameAction] GameManager missing — bỏ qua save.");

        yield return null;
    }
}
