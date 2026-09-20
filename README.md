# KatyushaPJ

> Một game 2D side-scrolling Action RPG làm bằng Unity URP. Người chơi điều khiển nhân vật chính và companion Hachiware chiến đấu, khám phá chapter, thu thập vật phẩm và nâng cấp kỹ năng.

## Trạng thái hiện tại

- Unity `6000.3.11f1` với URP.
- Gameplay chính gồm di chuyển, tấn công thường, skill, enemy state machine, health/stat modifier, inventory, equipment, shop, dialogue, cutscene sequencer và save/load.
- `GameUIRoot` hiện có Inventory UI, Skill UI, Quest UI, Shop UI và các panel gameplay cơ bản.
- Quest UI gồm danh sách quest item trong ScrollView, slot spawn runtime từ `QuestSlotUI.prefab` và panel detail dùng `QuestListUI`, `QuestSlotUI` và `QuestDetailUI`.
- Shop của Usagi dùng `UsagiShopTrigger`: player vào vùng `BoxCollider2D` thì hiện nút shop qua `UIManager.SetShopButtonActive`, ra khỏi vùng thì ẩn nút. Trigger dùng `HashSet<Collider2D>` để chịu multi-collider, check `Player` qua tag/`attachedRigidbody`/`root`, có tham chiếu `SequencePlayer`.
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

Trừ `Test/ShaderTest`, các scene còn lại đều đang được bật trong `ProjectSettings/EditorBuildSettings.asset`. Lưu ý: `MainMenuUI` hiện load scene theo tên `GrassScene`, còn dữ liệu chapter 1 trỏ tới `OutskirtsScene`; đây là điểm cần thống nhất khi hoàn thiện flow chapter.

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
| `UI/` | Main menu, pause (`MenuUI`), option, inventory, skill (`SkillUI`/`SkillPanelUI`), stats, tab control, tutorial, game over và victory UI |
| `Pattern/` | `Singleton`, `ObjectPool`/`PoolMember` dùng chung |
| `Scene/` | `GameSceneController` load scene gameplay |
| `Teleport/` | `TeleportManager`, `FadeUI` chuyển cảnh |
| `Effect/` | Camera (`CameraController`, `CameraFollowParallax`), `ButtonSFX`, `AutoDestroy` |
| `Editor/` | `CutsceneDataDrawer` custom inspector cho cutscene |
| `HuongDan/` | Docs hướng dẫn (`ItemInfo.md`, roadmap đồ họa/Firebase) |

## Các hệ thống chính

### Player và combat

- `PlayerController` điều phối movement, skill input, animation và health.
- `Stand` đại diện cho Hachiware companion và xử lý normal attack.
- `CharacterStats` hỗ trợ stat cơ bản cùng modifier additive/multiplicative.
- `Health` nhận damage, xử lý armor, damage reduction, shield, regen và đồng bộ Max HP từ `CharacterStats`.

### Enemy và boss

- Enemy dùng state machine đã refactor theo `REFACTORING_PLAN.md`: `EnemyController` điều phối qua `IEnemyMovement`/`IEnemyCombat`/`IEnemyRanged`/`IEnemyStateContext`, logic di chuyển ở `MovementManager`, animation ở `AnimationController`, tạo state qua `EnemyStateFactory`. Các state gồm idle, alert, patrol, pursuit, return-to-post, attack/generic-attack, hurt, heal, kiting, die, boss-dormant, cộng state riêng của boss (`GolemAttack/Pursuit`, `RevivalChanneling`, `Paralyzed`, `VoidPursuit`, `BloodMoon`...).
- Damage thường của enemy tính từ tâm `GetAttackCenter()` (pivot + `attackOffset.x * hướng`) với bán kính `GetAttackDamageRadius()`; `attackRange` chỉ còn dùng để quyết định vào state Attack. Enemy thường phải đặt `attackDamageRadius = attackRange` (mặc định `-1` = dùng `attackRange`).
- `GolemController` đã gom về dùng chung `GetAttackCenter()` của base, không còn logic `flipX` riêng.
- Abomination đã sửa pivot toàn bộ 29 slice sheet về `(0.225, 0.5)`, prefab đang đặt `attackOffset (2.12, 0)` và `attackDamageRadius 3.1`. Clip `Abomination_attack.anim` đã có Animation Event `DealNormalAttackDamage`.
- Enemy hiện có các prefab như Green/Blue Slime, Skull, NightBorne, Catto, Bat, Mad Ghost, Golem/Golem_Orange/Golem_Blue/GolemV5, Necromancer, Abomination và VoidBoss.
- Boss code hiện có `BatBoss`, `VoidBoss` và `DuoGolem` (`GolemController` + `GolemA`/`GolemB`). BatBoss là implementation tham chiếu cho arena reveal, damage source, weak point dùng chung `Health`, boss health bar và boss defeat cutscene. `Chapter7KanusBossScene` đã có trong build nhưng chưa thấy controller riêng cho Kanus trong `Assets/Script`.
- `DuoGolem` đã có code hazard (`SnapTrap`/`RollingStone` của `GolemA`, `StoneSpike`/`Hailstorm` của `GolemB`, `ArenaHazardController`); phần còn lại là kiểm tra gán prefab và tuning thông số gameplay.

### Item, inventory và shop

- Item data được lưu bằng ScriptableObject.
- Inventory gồm item slots, equipment, skill matrix và quest items. Quest item được lưu riêng trong `Inventory.questItems` và không chiếm inventory slot thường.
- Equipment áp dụng `ItemStats` vào `CharacterStats`.
- Consumable tạo và áp dụng status effect thông qua `ConsumableManager`.
- Shop dùng `ShopManager`, `ShopEntrySO`, category filter, item list và item detail UI.

### Quest UI

- `QuestListUI` đọc `Inventory.questItems`, tạo một `QuestSlotUI` cho mỗi item và spawn vào `ScrollView/Content`.
- `QuestSlotUI` hiển thị icon, tên item và xử lý chọn item.
- `QuestDetailUI` hiển thị description của item đang chọn.
- Prefab slot độc lập nằm tại `Assets/Resources/Prefab/UI/QuestSlotUI.prefab`.
- Quest item trong debug snapshot chỉ lưu bằng tên item, không lưu amount:

```json
"questItems": ["Map"]
```

### Dialogue, sequencer và progression

- Dialogue dùng `DialogueData`, `CharacterProfile`, `DialogueManager` và `DialogueUI`.
- Sequencer hỗ trợ dialogue, narration, animation, background, image, teleport, add/remove item, unlock Hachi, activate object, option/choice (`OptionAction` spawn nút từ prefab và share routine với `ShowImageAction`), next chapter, enemy-cleared/boss-defeat/cutscene trigger và scene transition.
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
- `DuoGolem` đã có hazard skill code (`SnapTrap`, `RollingStone`, `StoneSpike`, `Hailstorm`); còn lại là kiểm tra gán prefab trong scene và tuning thông số gameplay chính thức.
- Save system vẫn tra item bằng `itemName`; chưa migrate hoàn toàn sang `itemId`.
- Một số icon item/skill vẫn là placeholder hoặc còn thiếu; các quest item hiện có vẫn cần kiểm tra/gán icon riêng.
- Abomination còn dở: prefab còn 2 component `CharacterStats` trùng nhau; cần kiểm tra flip 2 hướng, collider và cảm giác hitbox trong Play Mode.
- Quest UI đã có logic spawn slot và panel detail, nhưng wiring/visibility cần tiếp tục kiểm tra trong Unity Play Mode; object template trong `GameUIRoot` không phải slot runtime được spawn.
- `InventoryDebugTool` chỉ chạy trong Editor và tự load `DebugData/inventory_debug.json` khi Play Mode bắt đầu. Nếu sửa format snapshot thủ công, `questItems` phải là `List<string>` như ví dụ ở trên.
- Các thay đổi gameplay và layout UI nên được kiểm tra lại trong Unity Play Mode sau khi merge prefab/scene.

## Tài liệu liên quan

- [REFACTORING_PLAN.md](REFACTORING_PLAN.md) — kế hoạch refactor enemy system (các interface `IEnemyMovement`/`IEnemyCombat`/`IEnemyRanged`/`IEnemyStateContext`, `MovementManager`, `AnimationController`, `EnemyStateFactory` hiện đã có trong code).
- [Assets/Docs/SequencerContext.md](Assets/Docs/SequencerContext.md) — hướng dẫn sequencer.
- [Assets/Docs/SKILL_SYSTEM_PLAN.md](Assets/Docs/SKILL_SYSTEM_PLAN.md) — thiết kế skill system.
- [Assets/Docs/Roadmap.md](Assets/Docs/Roadmap.md) — roadmap dự án.
- [Assets/Docs/Contexts/Boss_Implementation_Guidelines.md](Assets/Docs/Contexts/Boss_Implementation_Guidelines.md) — nguyên tắc và checklist rút ra từ BatBoss cho boss tiếp theo.
- [Assets/Docs/Contexts/Katyusha_BatBoss_Context.md](Assets/Docs/Contexts/Katyusha_BatBoss_Context.md) — context kiến trúc BatBoss.
- [Assets/Docs/Contexts/Katyusha_VoidBoss_Context.md](Assets/Docs/Contexts/Katyusha_VoidBoss_Context.md) — context kiến trúc/gap VoidBoss.
- [Assets/Docs/Contexts/KatyushaPJ_Boss_System_Summary.md](Assets/Docs/Contexts/KatyushaPJ_Boss_System_Summary.md) — tổng hợp hệ thống boss.
- [Assets/Docs/Contexts/OptionAction_Design.md](Assets/Docs/Contexts/OptionAction_Design.md) — thiết kế choice/option trong sequencer.
- [Assets/Docs/Contexts/BatBoss_Test_TODO.md](Assets/Docs/Contexts/BatBoss_Test_TODO.md) — checklist test core BatBoss.
- [Assets/Docs/DialogueScript.md](Assets/Docs/DialogueScript.md) — dialogue và cast.
- [Assets/Script/HuongDan/ItemInfo.md](Assets/Script/HuongDan/ItemInfo.md) — catalog item.
