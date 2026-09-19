using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Biến thể của DialogueAction: chạy thoại bình thường, tới dòng cuối thì tắt nút Next
/// và spawn các nút lựa chọn (từ prefab, xếp dọc tại vị trí Next) thay chỗ. Chọn xong:
/// add item của option đó, show icon, rồi complete. Dùng cho các đoạn rẽ nhánh phần
/// thưởng (VD Usagi chọn 1-3).
/// </summary>
[System.Serializable]
public class OptionEntry
{
    public string label;
    public ItemData item;
}

[System.Serializable]
public class OptionAction : SequenceAction
{
    public DialogueData dialogue;
    public List<OptionEntry> options = new();

    public override bool HandlesClickInternally => true;

    public override IEnumerator Execute()
    {
        DialogueUI ui = DialogueUI.Instance;
        if (ui == null)
        {
            Debug.LogWarning("[OptionAction] DialogueUI missing. Action skipped.");
            yield break;
        }
        if (options == null || options.Count == 0)
        {
            Debug.LogWarning("[OptionAction] No options assigned. Action skipped.");
            yield break;
        }

        List<DialogueLine> lines = dialogue != null && dialogue.lines != null
            ? dialogue.lines
            : new List<DialogueLine>();

        int picked = -1;
        bool done = false;

        if (lines.Count > 0)
        {
            ui.Show(lines[0]);
            int index = 0;
            while (!done)
            {
                if (index >= lines.Count - 1)
                {
                    ui.ShowOptions(options.ConvertAll(o => o != null ? o.label : string.Empty),
                        i => { picked = i; done = true; });
                    while (!done)
                        yield return null;
                }
                else
                {
                    bool next = false;
                    System.Action handler = () => next = true;
                    ui.OnNextClicked += handler;
                    while (!next && !done)
                        yield return null;
                    ui.OnNextClicked -= handler;
                    if (done)
                        break;
                    index++;
                    ui.UpdateLine(lines[index]);
                }
            }
            ui.ClearOptions();
        }
        else
        {
            List<string> labels = options.ConvertAll(o => o != null ? o.label : string.Empty);
            ui.ShowOptions(labels, i => { picked = i; done = true; });
            while (!done)
                yield return null;
            ui.ClearOptions();
        }

        if (picked >= 0 && picked < options.Count)
        {
            OptionEntry entry = options[picked];
            if (entry != null && entry.item != null)
            {
                if (Inventory.Instance != null)
                    Inventory.Instance.AddItem(entry.item, 1);
                else
                    Debug.LogWarning("[OptionAction] Inventory instance is null. Item not added.");

                if (entry.item.itemIcon != null)
                    yield return ShowImageAction.ShowSprite(entry.item.itemIcon, 0.9f, 0.5f, 2f, false);
            }
            else
            {
                Debug.LogWarning("[OptionAction] Item của option chưa tạo. Bỏ qua phần thưởng.");
            }
        }

        ui.Hide();
    }
}
