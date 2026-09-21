# Save/Load + Ending — Known Issues

> Tổng hợp từ đợt rà soát save/load (khi làm nội dung Chapter 7).
> Quy ước: [FIXED] đã sửa, [TODO] chưa làm, [BACKLOG] để sau cùng, [BY-DESIGN] chấp nhận.

## 1. Ending (mức crash game)

- [FIXED] `ChapterManager.CompleteChapter()` và `CompleteBossChapter()` gọi
  `SceneManager.LoadScene("MainMenu")` trong khi scene thật tên `MainMenuScene`
  (`Assets/Script/SaveSystem/ChapterSystem/ChapterManager.cs`, đã sửa 2 chỗ
  sang `"MainMenuScene"`). Path này trước đây chưa bao giờ chạy tới vì game
  chưa có màn cuối; sau Chapter 7 (Kanus + `BossEndTrigger`) sẽ chạy tới.

## 2. Điểm save thực tế (đã quét toàn bộ `Assets/Script`)

- Save thủ công: nút Save ở MainMenu (`MainMenuUI.OnSaveButtonClick`).
- Autosave theo sự kiện chapter (`ChapterManager`): `TryAdvanceToNextChapter`,
  `CompleteChapter`, `CompleteBossChapter`.
- `SavePoint.cs` đầy đủ code (vùng `BoxCollider2D`, option `singleUse`) nhưng
  [TODO] **0/16 scene có đặt object này** — chết lâm sàng. Chapter 7 (Hyvoria
  dài, Kanus boss) chưa có điểm save giữa chừng nào.
- [BY-DESIGN?] Không save định kỳ, không save khi thoát/minimize game
  (`OnApplicationQuit/Pause/Focus` đều không hook). Tắt game đột ngột giữa
  chapter = mất tiến trình từ mốc save chapter gần nhất.

## 3. Trạng thái scene không được lưu [BACKLOG — xử lý sau cùng, đã chốt]

- Flag tiến trình trong scene không vào save file: cổng đã mở, Tezzy đã hide,
  trigger cutscene đã chạy, trạng thái boss/pillar/hazard, vị trí enemy.
- Load game = load lại scene tươi + restore inventory/vị trí/máu. Trigger
  one-shot sẽ chạy lại từ đầu (hiện tự chữa được vụ Hachi unlock).
- Nhỏ kèm theo: `RestorePlayerState` dùng `SetHealth` (không reset `isDead`).

## 4. Ghi chú kiểm thử cho Chapter 7

- Save/load tại `HyvoriaScene` và `Chapter7KanusBossScene` chưa được test
  trong Play Mode (2 scene mới).
- `WaveBossController` load-safe theo thiết kế (pool inactive, arena trigger
  chạy lại `StartBossFight`, buff apply fresh) — nhưng cũng chưa test thực tế.
- Flow `GrassScene` vs `OutskirtsScene` vẫn chưa thống nhất (vấn đề cũ, không
  thuộc đợt này).
