# BatBoss — Chapter 4 Boss

## Trạng thái implementation

BatBoss là boss bay đầu tiên của Chapter 4. Boss dùng lại pipeline enemy hiện có:

- `BatBossController : EnemyController` điều phối state machine và animation events.
- `BatHealth : Health` lọc damage theo `DamageSourceType`.
- Pillar dùng `CharacterStats + Health` giống một enemy thu nhỏ, không có hệ HP riêng.
- Boss arena chỉ reveal camera; không quản lý gate vật lý, player spawn hoặc chapter completion.
- Boss defeated được nối sang cutscene bằng `BossDefeatCutsceneTrigger`.

## Luồng encounter

1. Boss được đặt sẵn trong scene, thường ở trạng thái inactive hoặc ngủ.
2. Player đi vào trigger của `BossArenaController`.
3. Arena bật boss nếu `activateBossOnEnter`, gọi `BeginEncounter()` và chạy animation `Bat_WakeUp`.
4. Animation event `OnWakeUpComplete()` chuyển boss sang `Combat`.
5. Khi đã thức, boss kiểm tra khoảng cách với Player:
   - trong `attackRange`: chọn `Atk1` hoặc `Atk2` khi attack cooldown sẵn sàng;
   - ngoài `attackRange`: truy đuổi theo trục X nhưng giữ nguyên độ cao.
6. Khi HP về 0, `DieState` chạy animation chết trong 2 giây, gọi `HandleEnemyDeath()`, phát `OnBossDefeated`, spawn loot và destroy boss.

## State machine hiện tại

```text
Sleep / chưa encounter
        |
        | BossArenaController -> BeginEncounter()
        v
WakeUp -- OnWakeUpComplete() --> Combat
                                  |
             +--------------------+--------------------+
             |                                         |
      trong AttackRange                         ngoài AttackRange
             |                                         |
       Atk1 hoặc Atk2                              Chase X
             |                                         |
             +--------------------> Combat <----------+

Damage về 0 --> Die (2s) --> HandleEnemyDeath() --> Destroy
```

State cache trong `BatBossController`:

| Key | State | Vai trò |
|---|---|---|
| `Combat` | `BatCombatState` | Đánh thường hoặc truy đuổi Player |
| `DropSphere` | `GenericAttackState("Atk1", 1.2s, "Combat")` | Thả BatSphere |
| `SpawnDoT` | `GenericAttackState("Atk2", 1.2s, "Combat")` | Tạo Bat-Hole |
| `Hurt` | `HurtState("Combat", false)` | Chỉ được gọi khi Pillar nổ |
| `Die` | `DieState(2s, callback)` | Death animation và cleanup |

## Vị trí và physics

Boss không dùng gravity và không bay theo sine nữa. `useInitialPosition = true` giữ boss ở vị trí đặt trong scene; nếu tắt, dùng `fixedHoverPosition`. Rigidbody2D được cấu hình Kinematic, gravity bằng 0 và khóa vị trí/rotation phù hợp.

`DropSphere()` spawn BatSphere tại `x = Player.x`, `y = Boss.y + dropHeight`.

`SpawnAoECircle()` spawn Bat-Hole tại `Player.x` và `holeSpawnY` (mặc định `-7`).

## Damage rules

`BatHealth` chỉ nhận:

| Source | Kết quả |
|---|---|
| `Ranged` | Nhận damage nhân `rangedDamageMultiplier` (mặc định 1.5) |
| `Pillar` | Nhận damage và gọi `ForceHurtState()` |
| `System` | Nhận damage bình thường |
| `Melee`, `Stand`, `EnemySkill`, null hoặc thiếu `DamageSource` | Deflect, không trừ HP |

Projectile muốn gây damage cho BatBoss phải có `DamageSource` với `sourceType = Ranged`. Damage nội bộ dùng `DamageSource.SystemSource`.

## Pillar

Pillar là enemy thu nhỏ, không còn `hp`, `currentHP`, `TakeHit()` hay health bar riêng:

- Root có `CharacterStats` và `Health`.
- HP hiện tại được lấy từ `CharacterStats.baseMaxHP` (prefab hiện tại: 20).
- Root nằm trên layer/tag Enemy để normal attack, Stand và projectile đi qua pipeline damage chung.
- `EnemyHPBar` dùng lại prefab health bar enemy.
- Khi `Health.OnDied` được gọi, Pillar phát VFX/SFX, gây `PillarBurstDamage = 25% MaxHP` lên boss rồi destroy.

## Attack và animation events

| Clip | Event | Kết quả |
|---|---|---|
| `Bat_WakeUp` | `OnWakeUpComplete()` | Bắt đầu Combat |
| `Bat_Atk1` | `DropSphere()` | Spawn BatSphere |
| `Bat_Atk2` | `SpawnAoECircle()` | Spawn Bat-Hole |
| `Bat-Hole` | `DealDamageNow()` | Damage Player nếu đang trong vùng |
| `Bat-Hole` | `DestroyAfterAnimation()` | Destroy Bat-Hole sau một lần animation |

BatSphere khi chạm ground sẽ gây burst AoE một lần, tạo `HazardZone` nếu có prefab và tự destroy sau animation explosion.

## Arena setup

```text
[BossArenaController]
 ├── Collider2D (Is Trigger)
 ├── boss              -> BatBoss trong scene
 ├── bossCamera        -> CameraFollow
 ├── activateBossOnEnter
 └── onRevealComplete  -> optional UnityEvent, ví dụ SequencePlayer.Play()
```

Arena chỉ chịu trách nhiệm nhận Player vào vùng reveal, bật boss, gọi wake-up và zoom camera. Player spawn do `SceneTransitionAction`/`NextChapterAction` xử lý; boss defeated do `BossDefeatCutsceneTrigger` xử lý.

## Boss health bar

`BossHealthBarUI` vẫn implement `IHealthBar` để nhận cập nhật từ `Health`, nhưng không tự đổi màu. `Slider.value` chỉ phản ánh HP; màu nằm ở `Fill` trong prefab và do Inspector kiểm soát. Boss bar dùng Screen Space Canvas theo prefab hiện tại.

## Setup checklist

- Bat root có `CharacterStats`, `BatHealth`, `BatBossController`, `Animator`, `Rigidbody2D` và tag `Enemy`.
- Rigidbody2D của Bat là Kinematic, gravity 0.
- Gán `batSpherePrefab`, `pillarPrefab`, `holePrefab` và các `pillarSpawnPoints`.
- Bat có animation events đúng tên như bảng trên.
- BossArena có Collider2D và gán `boss`, `bossCamera`.
- Boss defeat trigger gán cùng BatBoss và một `SequencePlayer` nếu cần cutscene.
- Pillar prefab có `CharacterStats`, `Health`, `Pillar`, Collider2D và `EnemyHPBar`.

## Known limitations

- `BatCombatState` truy đuổi theo trục X, chưa có navigation/pathfinding.
- Attack timer vẫn là giá trị cấu hình trong state; nếu đổi độ dài clip cần kiểm tra lại timing event.
- Cleanup hiện tại destroy boss nhưng các hazard đã spawn trước đó có lifecycle riêng; cần test riêng case boss chết giữa attack.
