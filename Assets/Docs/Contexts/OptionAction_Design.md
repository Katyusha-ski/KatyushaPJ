# OptionAction — Thiết kế (ĐÃ CHỐT 2026-09-17, đã implement)

> 1.A option là đuôi dialogue — 2.A thưởng gộp trong action — 3.B nút prefab
> (`DialogueOptionButton.prefab`, clone NextBtn, xếp dọc tại vị trí Next).

> Mục tiêu: đoạn Usagi Ch.6 cho player chọn 1 trong 3, nhận vũ khí tương ứng + show icon.
> Hiện trạng: đã implement (`OptionAction.cs`, prefab nút, API `DialogueUI`),
> CS4 chạy được. Còn chờ: vũ khí + label thật + test Play Mode.

## 1. Vị trí option trong flow

- Phương án A (đề xuất): option là đuôi của dialogue. Action ôm `DialogueData`, chạy thoại
  bình thường bằng nút Next, tới dòng cuối ("...chọn 1 đến 3 đi...") thì tắt Next,
  spawn 3 nút thay chỗ. Không node rời, không ngắt mạch đọc.
- Phương án B: node choice đứng riêng sau `DialogueAction`. Đơn giản hơn ~20 dòng code
  nhưng thoại prompt và nút chọn tách làm 2 node, sau này đổi câu prompt phải sửa 2 chỗ.

## 2. Thưởng (add item + show icon)

- Phương án A (đề xuất): mỗi option = `{label, item}`; chọn xong add luôn trong action,
  show icon món trúng rồi complete. Không truyền kết quả giữa các action.
- Phương án B: giữ node `AddItem`/`ShowImage` rời + blackboard truyền index đã chọn
  (static hoặc field trên `SequencePlayer`). Thêm coupling, thêm chỗ gãy — không đáng
  với 1 mối dùng duy nhất (CS4).

## 3. Nút bấm

- Ngắn hạn: dựng runtime dưới panel (`Button` + `Image` nền mờ + `TMP_Text`), dọc,
  canh dưới. Chạy được ngay, xấu.
- Dài hạn: UI artist làm prefab + style, thay hàm dựng, không đổi logic action.
- `DialogueUI` thêm: event `OnNextClicked`, `SetNextVisible(bool)`,
  `ShowOptions(labels, onPick)`, `ClearOptions()`. Listener `AdvanceLine` cũ giữ nguyên
  (tự no-op khi `DialogueManager` không active nên không xung đột nút Next mượn tạm).

## 4. Icon sau chọn

- Tách lõi show ảnh của `ShowImageAction` thành `ShowSprite(...)` static để gọi lại.
  Behavior node cũ giữ nguyên (chỉ đổi chỗ code, không đổi flow).
- Không copy-paste блок fade sang action mới.

## 5. Data

```csharp
OptionEntry { string label; ItemData item; }
OptionAction : SequenceAction {
  DialogueData dialogue;          // thoại trước, dòng cuối là câu prompt
  List<OptionEntry> options;      // 3 entry; item null thì skip có warning
  HandlesClickInternally = true;  // SequencePlayer không chờ click thừa
}
```

- CS4 sau khi xong: `[OptionAction(Before + 3 option)] → [DialogueAction(After)]`.
  Node `AddItem`/`ShowImage` rời hiện tại xóa.
- Label tạm "1/2/3", đổi tên thật khi có item. `amount` luôn 1 (vũ khí).

## 6. Trường hợp biên (không được soft-lock)

| Tình huống | Xử lý |
|---|---|
| `dialogue` null | Bỏ qua thoại, show option luôn |
| `options` rỗng | warn + complete ngay |
| `DialogueUI` null | warn + complete ngay |
| item null (vũ khí chưa tạo) | warn, bỏ qua thưởng, vẫn complete |
| `Inventory` null | warn, vẫn show icon nếu có rồi complete |
| Đổi scene giữa choice | coroutine chết theo scene như mọi action khác (chấp nhận) |

## 7. Test khi xong

1. Play CS4: thoại chạy đủ 9 dòng, dòng cuối tắt Next hiện 3 nút.
2. Chọn từng nút: đúng item vào túi, icon đúng món, panel dọn sạch, Next trả lại.
3. Item null: warn, không kẹt, qua dialogue sau bình thường.
4. Regression `ShowImageAction` cũ (BatBoss cutscene còn dùng): behavior y nguyên.
