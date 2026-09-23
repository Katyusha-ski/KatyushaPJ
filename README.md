# KatyushaPJ

> Một game 2D side-scrolling Action RPG làm bằng Unity URP. Người chơi điều khiển nhân vật chính và companion Hachiware chiến đấu, khám phá chapter, thu thập vật phẩm và nâng cấp kỹ năng.

## Trạng thái hiện tại

- Unity `6000.3.11f1` với URP.
- Gameplay chính gồm di chuyển, tấn công thường, skill, enemy state machine, health/stat modifier, inventory, equipment, shop, dialogue, cutscene sequencer và save/load.
- `GameUIRoot` hiện có Inventory UI, Skill UI, Quest UI, Shop UI và các panel gameplay cơ bản.
- Quest UI gồm danh sách quest item trong ScrollView, slot spawn runtime từ `QuestSlotUI.prefab` và panel detail dùng `QuestListUI`, `QuestSlotUI` và `QuestDetailUI`.
- Shop của Usagi dùng `UsagiShopTrigger`: player vào vùng `BoxCollider2D` thì hiện nút shop qua `UIManager.SetShopButtonActive`, ra khỏi vùng thì ẩn nút. Trigger dùng `HashSet<Collider2D>` để chịu multi-collider, check `Player` qua tag/`attachedRigidbody`/`root`, có tham chiếu `SequencePlayer`.
- Health của enemy được đồng bộ với `CharacterStats.baseMaxHP` trong các prefab đã cấu hình.
- `CoreSystem` persist xuyên scene nhờ `PersistentRoot` (`Assets/Script/Pattern/PersistentRoot.cs`) gắn ở root prefab: dedup + `DontDestroyOnLoad` cả cụm. Bắt buộc vì `DontDestroyOnLoad` chỉ có tác dụng trên root GameObject — singleton con gọi trực tiếp sẽ warn + chết theo scene cũ (từng gây crash `GameManager.OnNewGameSceneLoaded` ở `Invoke` + mất inventory/shop/chapter state mỗi lần qua màn).

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
- Loot enemy (coin-only, trừ boss): xem `### Loot enemy` bên dưới. Boss bị loại trừ là `Bat` (`BatBossController`, layer `Boss`), `Golem_Orange` (`GolemA`), `Golem_Blue` (`GolemB`), `VoidBoss` (project không dùng Unity Tag `Boss`, boss được nhận diện bằng layer `Boss` (=9) + `BossHealthBarUI` + controller boss).

### Loot enemy

- Công thức: `coin = round(HP * 0.5)`, trần `max 15`, `dropChance: 100`, chỉ rớt `Coin` (`ItemsSO/Material/Coin.asset`). File nằm trong `Assets/Resources/Loot/`.
- Bảng hiện tại (10 enemy thường):

| Enemy (HP `baseMaxHP`) | LootTable | Coin |
|---|---|---|
| Skull_Enemy (5) | `SkullLT.asset` (giữ nguyên) | 3 |
| GreenSlime_Enemy (10) | `GreenSlimeLT.asset` (mới, giữ đúng giá trị gốc 5) | 5 |
| Golem_Enemy (20) | `Golem_EnemyLT.asset` (mới, coin-only, giữ đúng phần coin gốc 10) | 10 |
| BlueSlime_Enemy (25) | `BlueSlimeLT.asset` (update 5 → 12) | 12 |
| NightBorne (25) | `NightBorneLT.asset` (mới) | 12 |
| Catto (30) | `CattoLT.asset` (mới) | 15 |
| GolemV5 (30) | `GolemV5LT.asset` (mới) | 15 |
| Mad Ghost (35) | `MadGhostLT.asset` (mới, hạ 18 → 15 theo trần) | 15 |
| Abomination (40) | `AbominationLT.asset` (mới, hạ 20 → 15 theo trần) | 15 |
| Necromancer (60) | `NecromancerLT.asset` (mới, hạ 30 → 15 theo trần) | 15 |

- `GolemLT.asset` (10 coin + 1 Coal) giữ nguyên cho 2 Golem boss, không dùng cho enemy thường nữa.
- Fix kèm theo: nối lại `Health.lootManager` cho `Catto`/`GolemV5`/`Mad Ghost` (đang `{fileID: 0}`), fill `itemFloatPref` (ItemFloating) + `dropRad/dropForce: 0 → 1` cho các prefab thiếu, tách `GreenSlime` khỏi `BlueSlimeLT` dùng chung, thêm mới `LootManager` cho `Necromancer` (vốn chưa có component).

### Icon item/skill (kết quả rà soát)

- Skill của player đủ hết: 20x `ItemsSO/Skill` + 20x `SkillSO` player (`Dash`/`Defend`/`Melee`/`Range OnlyPlayer`) đều đã có icon. 5 skill riêng của enemy (`Stone Spike`, `NercoHole`, `NecroHeal`, `GolemMagic`, `NercoFire`) thiếu icon là đúng dự kiến.
- Item thiếu icon còn lại (8 quest item, hiện orphan — dò `guid` cho 0 reference trong shop/cutscene/scene/script): `Quest/Angel's Mirror`, `Key 1–5`, `Scroll`, `Witch's Hat`.
- `Consumable/Strength Potion` đã được gán icon `Sprites/ItemIcon/P_Red01.png` (dùng chung với Dragon's Blood/Resistance/Witch's Strength — cân nhắc đổi sang `P_Orange*`/`P_Yellow*` còn trống để dễ phân biệt). Item này có bán thật trong shop (`StrengthPotionEntrySO`, `unlockChapter: 1`, `stock: 8` trong `CoreSystem.prefab`).

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
- `DialogueUI.Show()` tự bật lại chính GameObject của mình, vì `UIManager.ResetForMainMenu()` (chỉ chạy ở MainMenu) tắt nguyên object để chống overlap — `Show` cũ chỉ bật panel con nên dialogue vô hình suốt session sau khi qua menu.
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
- Một số icon item/skill vẫn là placeholder hoặc còn thiếu; 8 quest item (`Angel's Mirror`, `Key 1–5`, `Scroll`, `Witch's Hat`) vẫn thiếu icon nhưng hiện orphan (0 reference trong shop/cutscene/scene/script), xem chi tiết ở `### Icon item/skill`.
- Abomination còn dở: prefab còn 2 component `CharacterStats` trùng nhau; cần kiểm tra flip 2 hướng, collider và cảm giác hitbox trong Play Mode.
- Quest UI đã có logic spawn slot và panel detail, nhưng wiring/visibility cần tiếp tục kiểm tra trong Unity Play Mode; object template trong `GameUIRoot` không phải slot runtime được spawn.
- `InventoryDebugTool` chỉ chạy trong Editor và tự load `DebugData/inventory_debug.json` khi Play Mode bắt đầu. Nếu sửa format snapshot thủ công, `questItems` phải là `List<string>` như ví dụ ở trên.
- Các thay đổi gameplay và layout UI nên được kiểm tra lại trong Unity Play Mode sau khi merge prefab/scene.

## Cân bằng kinh tế (coin/loot/shop)

- Loot enemy coin-only, dropChance 100%, lượng cố định theo HP (`round(HP*0.5)`, trần 15). Boss không tính thu nhập (Bat/VoidBoss không rớt; DuoGolem giữ `GolemLT` cũ ngoài chuẩn).
- Thu nhập coin/chapter (giết mỗi con 1 lần, không tính boss, không tính spawn runtime nếu có):

| Chapter (scene) | Income |
|---|---|
| 1 Outskirts | 28 |
| 2 Rohok | 102 |
| 3 KuriFarm | 227 |
| 4 Mira + BatBoss | 191 |
| 5 Kynarite + DuoGolem | 108 |
| 6 Mythara + VoidBoss | 267 |
| 7 Hyvoria + KanusBoss | 441 |

- Quy tắc giá shop theo đợt (`unlockChapter`): **mua 1 mỗi loại trong đợt ≤ 85% income chapter đó** (dư ~15%+). Lưu ý trung thực: mua FULL stock đợt 1 cần tối thiểu 57 coin trong khi income chỉ 28 nên bảo đảm full-stock là bất khả thi — chuẩn áp dụng là 1-mỗi-loại/đợt, stock thêm mua bằng tích lũy.
- 6 entry từng đòi material (Frog/Fox/Snail/Octopus — không nguồn rớt) đã chuyển coin-only (+5 coin/1 material, rồi scale theo đợt). Hiện không còn cost phi-coin nào.
- Kiểm chứng: Ch1 23 (dư 18%), Ch2 86 (16%), Ch3 120 giữ nguyên (dư 47%), Ch4 162 (15%), Ch5 90 (17%), Ch6 226 (15%), Ch7 50 giữ nguyên (dư 89%).

## TODO playtest (ghi chú)

- [ ] Item sprite/icon/balance: 8 quest item thiếu icon (`Angel's Mirror`, `Key 1–5`, `Scroll`, `Witch's Hat` — hiện orphan); `P_Red01` đang dùng chung 4 potion; `Coin.asset` vừa được Unity re-serialize (mở editor kiểm tra lại); cân lại effect consumable sau đợt chỉnh giá.
- [ ] Component/tag/layer: Abomination còn 2 `CharacterStats` trùng; rà layer `Boss`(9) vs `Enemy`(7) (VoidBoss đang ở layer 7); TODO tắt Player ở MainMenu đã note trong `PlayerManager.cs`.
- [ ] Địa hình: thống nhất flow `GrassScene` vs `OutskirtsScene` (chapter 1); cần pass tilemap/obstacle riêng (chưa liệt kê).
- [ ] Boss balance: Kanus chưa có controller riêng; tuning số DuoGolem/Bat/VoidBoss; WaveBoss — chờ số cụ thể.

## Tài liệu liên quan

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
