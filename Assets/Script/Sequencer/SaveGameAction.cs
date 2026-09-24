using System.Collections;
using UnityEngine;

/// <summary>
/// Lưu game ngay trong cutscene (thường đặt làm action CUỐI CÙNG, sau mọi
/// AddItem/grant). Save ghi đúng state lúc chạy; trigger một lần không phát
/// lại nên save phải đứng sau grant, nếu không retry sẽ về trạng thái
/// chưa-nhận-đồ mà trigger cũng câm (mất đồ vĩnh viễn).
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
