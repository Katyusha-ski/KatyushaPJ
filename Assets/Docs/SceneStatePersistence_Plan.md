# Plan B — Giữ trạng thái scene khi Hyvoria ⇄ KanusBoss (ĐÃ IMPLEMENT)

> Triển khai xong: `SceneStateTracker.cs` + `SaveData.sceneStates` +
> hook record ở `ActivateObjectsAction`, `SequenceCutsceneTrigger`,
> `SequenceEnemiesClearedTrigger`. Chưa test Play Mode.

> Quyết định: dùng snapshot + restore thay vì additive scene.
> Mục tiêu: đánh boss xong quay về level scene, toàn bộ object giữ trạng thái
> trước đó (enemy đã chết vẫn chết, Tezzy/Kanus vẫn ẩn, trigger 1-lần không
> phát lại). Persist cả xuyên session (lưu vào save file).
>
> Phạm vi (chốt): boss scene là one-shot, KHÔNG persist gì trong đó.
> Chết trong boss scene → reload tươi đánh lại từ đầu. Out game giữa boss
> scene → Continue rơi về save trước cửa vào (trong Hyvoria) → đi vào lại.
> Chỉ level scene (Hyvoria...) mới cần persist.

## 1. Dữ liệu (key = tên scene + tên object — hiện đã unique)

```csharp
[Serializable] public class ObjectState { public string name; public bool active; }
[Serializable] public class SceneStateRecord {
    public string sceneName;
    public List<string> deadEnemies = new();   // tên instance đã chết
    public List<ObjectState> objectStates = new(); // object đổi active (Tezzy, Kanus...)
    public List<string> firedTriggers = new(); // trigger/cutscene 1-lần đã chạy
}
// Không có defeatedBosses riêng: boss scene loại khỏi persist, cửa vào boss
// chặn bằng firedTriggers của trigger ToLab/Door.
```

- Runtime: `SceneStateTracker` (Singleton, DontDestroyOnLoad) giữ
  `Dictionary<string, SceneStateRecord>`.
- Save file: thêm `List<SceneStateRecord> sceneStates` vào `SaveData`;
  save ở các mốc hiện có (qua chapter, hạ boss, Save menu); load xong apply
  lại sau `OnSceneLoadedAfterLoadGame`.

## 2. Điểm hook (code hiện tại → việc cần thêm)

| # | Vị trí | Đã làm |
|---|---|---|
| 1 | Enemy chết | Tracker tự subscribe mọi `Health` tag `Enemy` lúc load scene (không cần đụng `WaveBossController`) → chết là ghi tên |
| 2 | Boss scene skip | `Refresh` bỏ qua scene chứa `WaveBossController` (check trong đúng scene đích) — vào là đánh mới |
| 3 | Flag boss riêng | Không cần — cửa vào boss chặn bằng `firedTriggers` của ToLab |
| 4 | `WaveBossArenaController` | Không sửa (boss scene không persist) |
| 5 | `SequenceEnemiesClearedTrigger` | Ghi `firedTriggers` khi bắn xong (dùng chung cho sau này) |
| 6 | `SequenceCutsceneTrigger` | Check `firedTriggers` trước khi nổ + ghi sau khi nổ (trigger không repeat). Door/ToLab đã fired thì im = chặn quay lại boss |
| 7 | `ActivateObjectsAction` | Ghi `objectStates` (tên + active) mỗi lần chạy; load scene apply lại |
| 8 | `SaveData` + `GameManager` | Field `sceneStates`; save thì export, Continue thì import + refresh lại scene vừa load, New Game thì xóa |

## 3. Thứ tự apply khi load scene (quan trọng)

1. Scene load xong → **import file trước** (nếu có file; chưa từng save thì
   giữ RAM) → apply `objectStates` (SetActive) + xóa enemy trong
   `deadEnemies` (Destroy ngay, trong handler `sceneLoaded` chạy trước `Start`
   của trigger/arena). RAM live chưa kịp save thì coi như chưa từng xảy ra.
2. Rồi mới để arena/trigger/cutscene chạy logic bình thường (trigger check
   `firedTriggers` và tự skip).

## 4. Edge case đã biết

- Player chết → retry reload scene: boss scene = tươi hoàn toàn (đánh lại từ
  wave 1); level scene = apply state đã persist (Tezzy vẫn ẩn, trigger cũ im),
  chỉ reset vị trí/máu player. Quái thường hồi sinh mỗi lần load (farm được).
- New Game: xóa toàn bộ records + save.
- Debug snapshot (`InventoryDebugTool`): không đụng scene state — test Play
  từ đầu vẫn thấy full enemy như cũ.
- Tên object phải unique trong scene (hiện tại 20 enemy + Tezzy/Kanus đều
  unique — giữ quy ước này khi đặt thêm object).
- `NightBorne` prefab đã sửa tag ở gốc (ảnh hưởng MiraScene) — không liên
  quan plan này nhưng ghi chú để khỏi quên.

## 5. Checklist khi implement xong

- [ ] Giết vài con ở Hyvoria (nếu có) → sang boss scene → về: con chết vẫn mất, Tezzy vẫn ẩn, intro không phát lại.
- [ ] Hạ Kanus → AfterCore → về Hyvoria (80,2): Tezzy vẫn ẩn, trigger cũ im.
  Cố tình đi ngược vào Door: ToLab im (đã fired), không vào lại boss được.
- [ ] Save giữa chừng → thoát game → Continue: trạng thái giữ nguyên.
- [ ] Player chết → retry: enemy/flow đúng như quyết định mục 4.
