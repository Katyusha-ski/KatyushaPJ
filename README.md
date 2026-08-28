# KatyushaPJ

> Một game 2D side-scrolling Action RPG làm bằng Unity URP. Người chơi điều khiển nhân vật chính và companion Hachiware chiến đấu, khám phá chapter, thu thập vật phẩm và nâng cấp kỹ năng.

## Trạng thái hiện tại

- Unity `6000.3.11f1` với URP.
- Gameplay chính gồm di chuyển, tấn công thường, skill, enemy state machine, health/stat modifier, inventory, equipment, shop, dialogue, cutscene sequencer và save/load.
- `GameUIRoot` hiện có Inventory UI, Skill UI, Shop UI và các panel gameplay cơ bản.
- Shop của Usagi dùng `UsagiShopTrigger`: player vào vùng `BoxCollider2D` thì hiện nút shop, ra khỏi vùng thì ẩn nút.
- Health của enemy được đồng bộ với `CharacterStats.baseMaxHP` trong các prefab đã cấu hình.

## Scene hiện có

| Scene | Vai trò |
|---|---|
| `MainMenuScene` | Menu chính, Continue/New Game và các tùy chọn cơ bản |
| `Test/GrassScene` | Scene gameplay/test nền cỏ |
| `Test/SnowScene` | Scene gameplay/test nền tuyết |
| `Test/StoneScene` | Scene gameplay/test nền đá |
| `OutskirtsScene` | Scene chapter 1 hiện đang được khai báo trong `Chapter-1.asset` |
| `RohokScene` | Scene main chapter 2 |
| `KuriFarmScene` | Scene main chapter 3 |
| `MiraScene` | Scene main chapter 4 |
| `KynariteScene` | Scene main chapter 5 |
| `MytharaScene` | Scene main chapter 6 |
| `HyvoriaScene` | Scene main chapter 7 |
| `Chapter4BatBossScene` | Boss scene chapter 4 |
| `Chapter5DuoGolemBossScene` | Boss scene chapter 5 |
| `Chapter6VoidBossScene` | Boss scene chapter 6 |
| `Chapter7KanusBossScene` | Boss scene chapter 7 |
| `Test/ShaderTest` | Scene kiểm thử shader |

Các scene trên đều đang được bật trong `ProjectSettings/EditorBuildSettings.asset`. Lưu ý: `MainMenuUI` hiện load scene theo tên `GrassScene`, còn dữ liệu chapter 1 trỏ tới `OutskirtsScene`; đây là điểm cần thống nhất khi hoàn thiện flow chapter.

## Cấu trúc code

Các thư mục chính nằm trong `Assets/Script/`:

| Khu vực | Nội dung |
|---|---|
| `Manager/` | `GameManager`, `PlayerManager`, `UIManager`, `AudioManager` |
| `PlayerThing/` | Player controller, movement, animation, normal attack, companion và stats |
| `EnemyThing/` | Enemy controller, state machine, enemy types và boss |
| `Health/` | Damage, HP, shield, regen và health bar |
| `Skill/` | Skill ScriptableObject, projectile, melee, dash, defend và spawn skill |
| `ItemSystem/` | Inventory, equipment, consumable, loot và shop |
| `Dialogue/` | Dialogue data, character profile, manager, UI và trigger |
| `Sequencer/` | Data-driven cutscene với `SequencePlayer` và các `SequenceAction` |
| `SaveSystem/` | Save/load JSON, chapter progression và save point |
| `UI/` | Main menu, pause, option, inventory, skill, stats, game over và victory UI |

## Các hệ thống chính

### Player và combat

- `PlayerController` điều phối movement, skill input, animation và health.
- `Stand` đại diện cho Hachiware companion và xử lý normal attack.
- `CharacterStats` hỗ trợ stat cơ bản cùng modifier additive/multiplicative.
- `Health` nhận damage, xử lý armor, damage reduction, shield, regen và đồng bộ Max HP từ `CharacterStats`.

### Enemy và boss

- Enemy dùng state machine với các state idle, alert, pursuit, attack, hurt, heal, kiting và die.
- Enemy hiện có các prefab như Slime, Skull, NightBorne, Golem, Necromancer, Abomination và VoidBoss.
- Boss code hiện có `BatBoss`, `VoidBoss` và `DuoGolem`.
- `DuoGolem` vẫn còn nhiều thông số thiết kế và hazard prefab cần hoàn thiện.

### Item, inventory và shop

- Item data được lưu bằng ScriptableObject.
- Inventory gồm item slots, equipment, skill matrix và quest items.
- Equipment áp dụng `ItemStats` vào `CharacterStats`.
- Consumable tạo và áp dụng status effect thông qua `ConsumableManager`.
- Shop dùng `ShopManager`, `ShopEntrySO`, category filter, item list và item detail UI.

### Dialogue, sequencer và progression

- Dialogue dùng `DialogueData`, `CharacterProfile`, `DialogueManager` và `DialogueUI`.
- Sequencer hỗ trợ dialogue, narration, animation, background, image, teleport, add item, activate object và scene transition.
- Chapter data lưu scene chính, boss scene và tiến trình chapter.
- Save system lưu dữ liệu game bằng JSON trong `Application.persistentDataPath`.

## Cách mở project

1. Mở project bằng Unity `6000.3.11f1`.
2. Mở `MainMenuScene` để chạy flow menu.
3. Dùng các scene trong `Assets/Scenes/Test/` khi cần kiểm tra gameplay riêng lẻ.
4. Nếu chỉnh shop Usagi, kiểm tra đồng thời:
   - `Assets/Resources/Prefab/UI/GameUIRoot.prefab`
   - `Assets/Resources/Prefab/Props/UsagiShopTrigger.prefab`
   - instance `UsagiShopTrigger` trong `OutskirtsScene`

## Known issues / phần còn lại

- Flow scene giữa `GrassScene` và `OutskirtsScene` cần được thống nhất.
- `DuoGolem` còn các hazard skill chờ prefab và thông số gameplay chính thức.
- Save system vẫn tra item bằng `itemName`; chưa migrate hoàn toàn sang `itemId`.
- Một số icon item/skill vẫn là placeholder hoặc còn thiếu.
- Các thay đổi gameplay và layout UI nên được kiểm tra lại trong Unity Play Mode sau khi merge prefab/scene.

## Tài liệu liên quan

- [REFACTORING_PLAN.md](REFACTORING_PLAN.md) — kế hoạch refactor enemy system.
- [Assets/Docs/SequencerContext.md](Assets/Docs/SequencerContext.md) — hướng dẫn sequencer.
- [Assets/Docs/SKILL_SYSTEM_PLAN.md](Assets/Docs/SKILL_SYSTEM_PLAN.md) — thiết kế skill system.
- [Assets/Docs/Roadmap.md](Assets/Docs/Roadmap.md) — roadmap dự án.
- [Assets/Docs/DialogueScript.md](Assets/Docs/DialogueScript.md) — dialogue và cast.
- [Assets/Script/HuongDan/ItemInfo.md](Assets/Script/HuongDan/ItemInfo.md) — catalog item.
