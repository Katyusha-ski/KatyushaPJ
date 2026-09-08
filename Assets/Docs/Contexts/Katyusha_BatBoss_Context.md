# Katyusha BatBoss Context — Chapter 4

Đây là context implementation hiện tại của BatBoss. Các quyết định thiết kế chi tiết
và checklist test nằm ở:

- `Assets/Script/EnemyThing/Boss/BatBoss/README.md`
- `Assets/Docs/Contexts/BatBoss_Test_TODO.md`
- `Assets/Docs/Contexts/Boss_Implementation_Guidelines.md`

## 1. Kiến trúc

```text
EnemyController
    └── BatBossController
          ├── BatCombatState
          ├── GenericAttackState (Atk1 / Atk2)
          ├── HurtState
          └── DieState

Health
    └── BatHealth (damage source filter)
```

BatBoss không còn dùng `BatHoverState` sine. `Combat` là state chính: boss đánh trong
`attackRange`, nếu Player ở ngoài thì truy đuổi theo trục X. Y của boss được giữ cố
định bởi `useInitialPosition` hoặc `fixedHoverPosition`.

## 2. Encounter và arena

`BossArenaController` có một `Collider2D` trigger và làm đúng bốn việc:

1. nhận Player đi vào vùng reveal;
2. bật boss nếu `activateBossOnEnter`;
3. gọi `BatBossController.BeginEncounter()`;
4. gọi `CameraFollow.ZoomToBossReveal()` và `onRevealComplete`.

Arena không quản lý gate vật lý, player spawn, health bar, music hay chapter completion.
Player spawn do `SceneTransitionAction`/`NextChapterAction` phụ trách. Boss defeat
được nối sang cutscene bằng `BossDefeatCutsceneTrigger`.

## 3. State và event

| State | Implementation | Điều kiện chuyển |
|---|---|---|
| Chưa encounter | Animator `Bat_Sleep` | Arena gọi `BeginEncounter()` |
| WakeUp | Animator `Bat_WakeUp` | Event `OnWakeUpComplete()` |
| Combat | `BatCombatState` | Attack ready hoặc truy đuổi |
| Atk1 | `GenericAttackState("Atk1", 1.2s, "Combat")` | Event `DropSphere()` |
| Atk2 | `GenericAttackState("Atk2", 1.2s, "Combat")` | Event `SpawnAoECircle()` |
| Hurt | `HurtState("Combat", false)` | Chỉ từ Pillar explosion |
| Die | `DieState(2s, callback)` | HP về 0 |

`BatBossController.Update()` luôn gọi base update khi boss chưa chết để `DieState`
không bị chặn bởi cờ `isAwake`. `HandleEnemyDeath()` phát `OnBossDefeated`, spawn
loot, ẩn boss health bar và destroy boss.

## 4. Damage

`BatHealth.TakeDamage(int, GameObject)` lấy `DamageSource` từ object gây damage:

| Source | Hành vi |
|---|---|
| `Ranged` | Nhận damage × 1.5 |
| `Pillar` | Nhận damage, gọi `ForceHurtState()` |
| `System` | Nhận damage bình thường |
| `Melee`, `Stand`, `EnemySkill`, null/thiếu component | Deflect |

Damage pipeline gốc của `Health` vẫn xử lý armor/shield và gọi death. Projectile,
melee và Stand không có nhánh xử lý Pillar riêng; chúng tìm `Health` bằng
`GetComponentInParent<Health>()`.

## 5. Pillar

Pillar là enemy thu nhỏ:

- Root có tag/layer Enemy.
- Root có `CharacterStats`, `Health`, `Pillar`, Collider2D.
- HP lấy từ `CharacterStats.baseMaxHP`; prefab hiện tại dùng 20.
- UI dùng lại `EnemyHPBar`.
- `Pillar` subscribe `Health.OnDied`, phát VFX/SFX, gây `25%` MaxHP boss bằng
  `DamageSourceType.Pillar`, rồi destroy.

Không dùng các API cũ `hp`, `currentHP`, `TakeHit()` hoặc health bar riêng.

## 6. Attack objects

### BatSphere

- Spawn tại `Player.x`, `Boss.y + dropHeight`.
- Rơi xuống bằng tốc độ cấu hình.
- Chạm ground: burst AoE một lần, tạo `HazardZone` nếu có prefab.
- Destroy sau `explosionAnimDuration`.
- Có gizmo `explosionRadius`.

### Bat-Hole

- Spawn tại `Player.x`, `holeSpawnY` (mặc định `-7`).
- Trigger cache Player đang ở trong vùng.
- Event `DealDamageNow()` gây damage một lần nếu Player còn trong vùng.
- Event `DestroyAfterAnimation()` destroy object sau animation one-shot.

## 7. UI

`BossHealthBarUI` implement `IHealthBar` để tương thích với `Health`. Script chỉ cập
nhật Slider value và visibility. Script không tự đổi màu. Màu mặc định lấy từ `Fill`
Image trong prefab; boss bar dùng Screen Space Canvas.

## 8. Serialized setup bắt buộc

### BatBoss

- `CharacterStats`, `BatHealth`, `BatBossController`, `Animator`, `Rigidbody2D`.
- Tag `Enemy`.
- Rigidbody Kinematic, Gravity Scale 0.
- Gán `batSpherePrefab`, `pillarPrefab`, `holePrefab`, `pillarSpawnPoints`.
- Gán `bossSprite` nếu muốn hurt flash.

### BossArenaController

- `Collider2D` trên cùng GameObject, `Is Trigger` sẽ được ép true trong Awake.
- Gán `boss` và `bossCamera`.
- Chỉ gán `onRevealComplete` nếu cần phát reveal cutscene/event.

### Defeat cutscene

- Gán cùng BatBoss vào `BossDefeatCutsceneTrigger`.
- Gán `SequencePlayer` hoặc để script tự tìm trên cùng GameObject.

## 9. Known limitations

- Chưa có navigation/pathfinding; truy đuổi chỉ theo X.
- Attack timer (`1.2s`) phải được kiểm tra lại nếu đổi độ dài animation.
- Hazard đã spawn có lifecycle riêng; cần quyết định rõ chúng hủy ngay hay chạy tiếp khi boss chết.
- Die animation chính thức và tuning damage/cooldown vẫn là phần cần kiểm thử trong Unity.
