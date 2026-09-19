# VoidBoss — Gap Analysis vs GDD & Completion Plan

> Trạng thái 2026-09-17 (PM): core chiến đấu ổn định qua test Play Mode — wake, pursuit,
> 4 đòn spawn cuối anim, trap dash xuyên, mưa Blood Moon 18×2. Còn lại tuning số + dựng scene.

> Ngày phân tích: 2026-09-17. Nguồn đối chiếu: GDD trong `KatyushaPJ_Boss_System_Summary.md` (mục 2 + 4),
> kiến trúc chốt trong `Katyusha_VoidBoss_Context.md`, mẫu chuẩn `Boss_Implementation_Guidelines.md` (BatBoss),
> code `Assets/Script/EnemyThing/Boss/VoidBoss/`, prefab `Assets/Resources/Prefab/Enemy/VoidBoss.prefab`,
> scene `Assets/Scenes/Chapter6VoidBossScene.unity`. Mọi gap dưới đây đã verify bằng đọc code/prefab.

## 1. Hiện trạng

Đủ khung: `VoidBossController` + 3 state custom (`VoidIdleState` não AI, `VoidPursuitState` interrupt,
`BloodMoonState` multi-wave) + NA dùng lại `GenericAttackState`. Đủ 5 prefab effect, animation events
đúng timing GDD (Stomp 0.4167, Spike 0.25, Sphere 0.1667, Ambush 0.3333 + `OnAttackAnimEnd` cuối mỗi clip).
Scene `Chapter6VoidBossScene` rỗng hoàn toàn (chỉ có Main Camera).

## 2. Cơ chế chung của 3 boss

State mỗi con viết riêng, nhưng khung lifecycle giống nhau:
ngủ → reveal camera → wake → combat → die → `OnBossDefeated` → cleanup.

| Cơ chế | BatBoss (mẫu) | DuoGolem | VoidBoss |
|---|---|---|---|
| Wake | `BossArenaController` reveal | `BeginEncounter` | `BeginEncounter` (đã xóa `VoidAggroTrigger`/AggroZone thừa 2026-09-17) |
| Máu/bar | `Health` + `SetHealthBar` | `Health` + bar | `Health` + `SetHealthBar` (chưa ai gọi) |
| Defeat | `OnBossDefeated` → cutscene/chapter | tương tự | event có, không ai nghe (scene rỗng) |
| Super armor | miễn flinch ranged | không có Hurt | `GetHurtState` trả state hiện tại (`VoidBossController.cs:158`) |
| Khóa hướng khi đánh | có | có | `LockFacing`/`UnlockFacing` |
| Hitbox | layer `EnemyAttack` riêng | `OverlapBox` + offset mirror | `OverlapCircle`/`OverlapBox` trực tiếp trong Controller |
| Fallback hết anim | `Animation_OnAttackEnd` | tương tự | `OnAttackAnimEnd` (`VoidBossController.cs:304`) |
| Cleanup khi chết | hủy pillar | cleanup hazard | hủy sphere/trap/telegraph (`HandleEnemyDeath`, `VoidBossController.cs:342`) |

Void thiếu duy nhất hàng **arena**: không lock map, không bar, không trigger defeat.

## 3. Gap so với GDD

### Critical — boss hiện không đánh được ai (✅ đã fix 2026-09-17, còn chờ test Play Mode)
- **C1. Không bao giờ thức tỉnh — ĐÃ XÓA CƠ CHẾ (thay bằng pattern arena chung).**
  AggroZone riêng trong prefab là thừa: arena (`BossArenaController`) đã cover reveal +
  zoom + bật boss cho Bat/DuoGolem. Đã xóa AggroZone khỏi prefab, xóa
  `VoidAggroTrigger.cs`, boss tự wake trong `BeginEncounter()` override. Khi dựng scene chỉ cần đặt `BossArenaController` trỏ boss.
- **C2. Stomp/SpikePierce quét nhầm layer.** `playerLayer` của boss là `m_Bits: 512`
  (`VoidBoss.prefab:222`) = layer 9 (Boss), phải là layer 3 (Player, bit 8).
  ✅ Fix: đã sửa `m_Bits: 8`.
- **C3. Void Sphere không bao giờ nổ trúng player.** `VoidSphere.prefab` thiếu dòng
  `playerLayer` → mask = 0 → check `(0 & (1 << layer)) == 0` luôn return sớm
  (`VoidSphereProjectile.cs:64`). Hết 5s tự nổ chay, Skill 1 = 0 damage.
  ✅ Fix: đã thêm mask Player (`m_Bits: 8`).
- **C4. Prefab refs null.** `voidSpherePrefab`, `ambushTrapPrefab`,
  `bloodMoonTelegraphPrefab` (`VoidBoss.prefab:213-215`), `bossSprite` (`:233`)
  đều `{fileID: 0}` → skill gọi là return im lặng, hurt flash tắt.
  ✅ Fix: đã gán 3 prefab + `bossSprite` trỏ SpriteRenderer của boss.
  Còn lại `deathVFX` vẫn null (chưa có asset VFX chết — code đã null-check).

### High
- **H1. Blood Moon friendly fire.** `BloodMoonTelegraphController.DealAoEDamage`
  (`BloodMoonTelegraphController.cs:26`) quét không mask → nổ trúng cả boss
  (boss có `Health`). ✅ Fix 2026-09-17: đã thêm field `playerLayer`, truyền vào
  `OverlapCircleAll`, prefab set mask Player (`m_Bits: 8`).
- **H2. Scene rỗng.** Chưa đặt boss, arena, health bar + `SetHealthBar`,
  `ObjectPool` (entry `BloodMoonTelegraph` size 25 đã có sẵn prefab),
  `BossDefeatCutsceneTrigger`/`BossEndTrigger`.
- **H3. Số lệch code vs prefab.** Stomp 15/20, Spike 25/15, Sphere 20/15, HP 300
  (doc ghi 500 tạm). Chốt 1 bộ số với design.

### Medium
- **M1. ~~`SpawnAmbushTrap` check gốc thế giới~~ ✅ Fix 2026-09-17.** Offset giờ theo phía
  player đứng tương đối với boss (`sign(player.x - boss.x)`), không còn `player.x < 0`.
- **M2. ~~Thứ tự ưu tiên lệch nhau~~ ✅ Thiết kế lại (2026-09-17).** Dormant dùng chung
  `BossDormantState` (mới, `States/Common/`): không patrol/vision/leash, wake duy nhất
  bằng arena. Void wake vào thẳng `Pursuit`, xóa `VoidIdleState.cs`; Golem `GetIdleState`
  trả dormant, hồi sinh xong về `Pursuit` (kể cả `ReviveWithHP`), vòng combat khép kín
  Pursuit ↔ Attack, leash/patrol không còn đường chạm. Não duy nhất ở Pursuit.
- **M3. Trap dash 4m** (`AmbushTrapController.cs:45-54`) khác chữ GDD "trồi lên đánh 1 nhát" —
  xác nhận thiết kế (giữ dash hay đứng yên).
- **M4. Fallback `Instantiate` khi pool null** (`VoidBossController.cs:290-294`) trái lệnh
  GDD cấm — thay bằng bảo đảm pool tồn tại trong scene.
- **M5. Stomp/Sphere dùng `GetComponent<Health>`** — hiện đúng vì collider + `Health`
  cùng nằm ở root Player, nhưng giòn; nên `GetComponentInParent` như Golem.

### Câu hỏi design còn mở (từ GDD 4.7, chưa ai chốt)
1. Không có Enrage theo %HP — giữ nguyên hay thêm?
2. Ultimate trùng lúc NA đang chạy dở — code hiện tại: melee trước (Idle), không ngắt
   `GenericAttackState` đang chạy — chốt thành luật chính thức?
3. Chết giữa Ultimate: code hủy wave ngay (`HandleEnemyDeath`) — xác nhận "thắng là sống".
4. Chốt số: `MeleeRange` 6, `a/b` 2.5, `minSpacing` 1.5, attempts = perWave × 5.
5. Art Skill 2 + Die (đang placeholder), bounds spawn Blood Moon trên map thật Ch.6.
6. Xác boss có destroy sau chết như Bat không (Void hiện để xác lại).

## 4. Kế hoạch test + hoàn thiện

- **Phase 0 — mở khóa (✅ xong 2026-09-17, sửa trực tiếp file):** C1 xóa AggroZone,
  boss tự wake qua `BeginEncounter`; C2/C3 mask Player; C4 gán 3 prefab + `bossSprite`
  (`deathVFX` vẫn null vì chưa có asset); H1 mask telegraph. Mở Unity Reimport rồi test.
- **Phase 1 — fix code:** M1, M2 xong. Còn chốt số H3. (H1 đã xong.)
- **Phase 2 — dựng scene (0.5 ngày):** boss + `BossArenaController` (trỏ boss, lo reveal +
  `BeginEncounter`) + `ObjectPool` + health bar + `SetHealthBar` +
  defeat triggers; quyết arena có lock map không.
- **Phase 3 — test lifecycle (0.5 ngày, theo Guidelines §12):** ngủ → reveal → wake →
  từng đòn → homing/timeout → silence → 5 wave + pool exhaust → super armor →
  chết giữa Ultimate → event đúng 1 lần → bar ẩn → loot.
- **Phase 4 — design chốt:** 6 câu hỏi mục 3.

## 5. Tuân thủ GDD đã đạt (không cần sửa)

- NA không dùng `Vector2.Distance` (grep `VoidBoss/` = 0 hit); damage bằng physics —
  đúng tinh thần "tuyệt đối không check cứng", dù không spawn prefab riêng như chữ GDD
  (đổi hướng đã ghi trong context doc SỬA 4).
- AI đúng thứ tự Skill2 > Skill1 > Pursuit, Pursuit interrupt, roll melee 50/50.
- Blood Moon theo timer 45s, Y random đối xứng, multi-wave mưa rào 18 wave × 2 điểm,
  interval 0.2s, vùng width ±7 / height ±4, giữ anti-overlap minSpacing 1.5, pool size 40.
- Super Armor, Facing Lock, Ghost Collision (matrix, không code riêng), Cleanup on Death.
- Animation chuyển sang spawn cuối clip (`GenericAttackState` nhận callback `onEnd`),
  không dùng Animation Event giữa clip. Duration đã sync độ dài clip mới
  (Stomp 0.75, Spike 0.92, Sphere 1.25, Ambush 0.6). Clip rebuild từ sheet 65 frame.
