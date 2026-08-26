using System.Collections;
using UnityEngine;

[System.Serializable]
public abstract class SequenceAction
{
    /// <summary>GameObject đang chạy sequence (do SequencePlayer inject). Không serialize.</summary>
    [System.NonSerialized] public GameObject Runner;

    [Tooltip("Chờ click chuột (giống Narration) rồi mới chạy action kế tiếp. Bật true cho action không tự xử lý click (VD: Animation, ActivateObjects).")]
    public bool waitForClick;

    /// <summary>true nếu action tự xử lý click bên trong Execute (Dialogue/Narration/Teleport) — tránh chờ click 2 lần.</summary>
    public virtual bool HandlesClickInternally => false;

    public abstract IEnumerator Execute();

    /// <summary>Chờ một cú click chuột trái trước khi sang action kế tiếp.</summary>
    public IEnumerator WaitForClick()
    {
        while (!Input.GetMouseButtonDown(0))
            yield return null;
    }
}