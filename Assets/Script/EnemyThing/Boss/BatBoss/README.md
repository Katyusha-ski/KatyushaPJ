# BatBoss — Chapter 4 Boss

## Tổng quan

BatBoss là một con dơi khổng lồ bay trên cao. Đây là boss đầu tiên được implement theo kiến trúc boss chung (kế thừa `EnemyController` + `BatHealth`).

---

## Cơ chế đặc biệt

### 1. Ranged-only Immunity

BatBoss **chỉ nhận damage** từ các nguồn sau:
- **Ranged** — đòn đánh tầm xa (projectile từ player skills)
- **Pillar** — khi pillar bị phá huỷ (25% MaxHP)
- **System** — damage từ hệ thống (DoT, AoE từ sphere)

Tất cả damage khác (`null` source, `Melee`, `Stand`, `EnemySkill`) → **bị chặn + phát âm thanh deflect**.

**Cách implement:**
- `DamageSource` component gắn trên projectile (`sourceType = Ranged`)
- `BatHealth` extends `Health`, override `TakeDamage(int, GameObject)` để filter source
- `Health.cs` gốc được sửa: `TakeDamage(int)` → route đến `virtual TakeDamage(int, GameObject = null)`

### 2. Pillar — Điểm yếu

Pillar là vật thể xuất hiện trên mặt đất:
- `Collider2D` với `IsTrigger = true` — không chặn player di chuyển
- `HP = 3`, có thể bị phá bởi: đánh thường (Stand), skill (Projectile), normal attack (PlayerNA)
- Khi bị phá → gây **25% MaxHP của boss** dạng burst damage
- Damage được tag `DamageSourceType.Pillar` để BatHealth chấp nhận

### 3. AoE Explosion (BatSphere)

BatSphere khi rơi xuống:
1. **Burst AoE** (`Physics2D.OverlapCircle`) — sát thương tức thì trong `explosionRadius`
2. **Spawn HazardZone** — vùng độc apply `DoTEffect` lên player khi đứng trong vùng

---

## State Machine

```
                    ┌─────────────────────────────────────────┐
                    │                                         │
                    ▼                                         │
              ┌──────────┐      timer 2s      ┌───────────────┴──┐
              │  Hover   │ ──────────────────► │  PickNextAttack │
              │  (bay)   │                     │  (random chọn)  │
              └──────────┘                     └───────┬─────────┘
                    ▲                                  │
                    │                ┌─────────────────┼─────────────────┐
                    │                ▼                  ▼                 ▼
                    │        ┌────────────┐   ┌────────────┐   ┌──────────────┐
                    │        │ DropSphere │   │  SpawnDoT  │   │ SpawnPillar  │
                    │        │ Attack1    │   │  Attack2   │   │  Attack3     │
                    │        │ (1.2s)     │   │  (1.2s)    │   │  (1.5s)      │
                    │        └────────────┘   └────────────┘   └──────────────┘
                    │                │                  │               │
                    └────────────────┴──────────────────┴───────────────┘
                          (timer hết → tự động SwitchTo Hover)

    ┌──────────┐   hit     ┌──────────┐      die      ┌──────────┐    2s    ┌──────────┐
    │  Hover   │ ────────► │  Hurt    │ ────────────► │   Die    │ ──────► │ Destroy  │
    │  (bay)   │   (0.3s)  │  (flash) │                │  (death) │         │ (object) │
    └──────────┘           └──────────┘                └──────────┘         └──────────┘
```

Tất cả attack states dùng chung 1 class `BatAttackAnimState`:
- `OnEnter` → trigger animation (`Attack1/Attack2/Attack3`)
- Animation Event trong clip gọi `DropSphere()` / `SpawnDoTZone()` / `SpawnPillar()`
- Timer hết → tự động về `Hover`

---

## File Structure

```
Boss/
├── DamageSource.cs                  Enum + component + SystemSource static
├── BossHealthBarUI.cs               Thanh máu boss (Slider + Gradient)
│
└── BatBoss/
    ├── README.md                    Tài liệu này
    ├── BatBossController.cs         FSM chính, public methods cho Animation Events
    ├── BatHealth.cs                 Override TakeDamage, filter damage source
    ├── BatSphere.cs                 Toxic sphere: fall + AoE burst + hazard zone
    ├── Pillar.cs                    Pillar điểm yếu: destroy → 25% MaxHP
    ├── BossArenaController.cs       Trigger arena, quản lý gate + health bar
    │
    └── States/
        ├── BatAttackAnimState.cs    Generic: trigger anim → timer → về Hover
        ├── BatHoverState.cs         Bay pattern sin, timer → chọn attack
        ├── BatHurtState.cs          Flash red 0.3s → về Hover
        └── BatDieState.cs           2s delay → HandleEnemyDeath → Destroy
```

---

## File đã sửa (ảnh hưởng toàn project)

| File | Thay đổi |
|---|---|
| `Health/Health.cs` | Thêm `virtual TakeDamage(int, GameObject)`; cũ route sang mới |
| `Skill/Prefabs/ProjectilePref.cs` | `TakeDamage(damage, gameObject)` — truyền source |
| `PlayerThing/Status/DoTEffect.cs` | `TakeDamage(damage, DamageSource.SystemSource)` |

---

## Integration Steps

### Prefabs cần setup trong Editor:

| Prefab | Component cần thêm | Ghi chú |
|---|---|---|
| **ProjectilePref** | `DamageSource(sourceType = Ranged)` | Cho player ranged skill |
| **BatBoss** | `BatHealth`, `BatBossController`, `CharacterStats`, `Rigidbody2D` (kinematic), `Animator` | Tag = "Enemy" |
| **BatSphere** | `Rigidbody2D` (gravity), `Collider2D` (IsTrigger), `BatSphere` | Prefab rơi từ trên cao |
| **HazardZone** | `Collider2D` (IsTrigger), `HazardZone` | Prefab vùng độc |
| **Pillar** | `Collider2D` (IsTrigger), `Pillar` | Prefab điểm yếu |

### Animation Events:

| Animation Clip | Event Method | Frame |
|---|---|---|
| `Attack1` | `DropSphere()` | Giữa clip |
| `Attack2` | `SpawnDoTZone()` | Giữa clip |
| `Attack3` | `SpawnPillar()` | Giữa clip |
| (cuối clip) | `OnAttackAnimEnd()` (optional) | Cuối clip |

### BossArena Scene Setup:

```
[BossArenaController] (trigger collider)
 ├── gán boss → BatBoss prefab (inactive)
 ├── gán bossHPBar → BossHealthBarUI (trong Canvas, hidden)
 ├── gán arenaGate → GameObject cổng (active false)
 └── gán playerSpawn → Transform (vị trí spawn player)

[BatBoss] (inactive → active khi player vào arena)
 ├── BatHealth component
 ├── BatBossController component
 └── Các spawn points (sphereSpawnPoints[], pillarSpawnPoints[])
```

---

## Key Formulas

```
PillarBurstDamage = MaxHP * 25%
HoverPosition.y   = hoverOrigin.y + hoverHeight + sin(hoverPhase * 0.7) * 0.5
HoverPosition.x   = hoverOrigin.x + sin(hoverPhase) * hoverAmplitude

BatHealth filter: (source == null) OR (DamageSource ∉ {Ranged, Pillar, System}) → deflect
```

---

## Known Issues / TODOs

- `EnemyController.OnCollisionEnter2D` non-virtual → boss vẫn chạy base collision handler khi va chạm. Hiện tại `movement.OnHitObstacle()` chỉ flip sprite, vô hại.
- `Pillar` dùng `GetComponent<ProjectilePref>()` để detect projectile — fragile. Có thể migrate sang layer/tag check.
- Timer animation trong `BatAttackAnimState` cứng (1.2s / 1.5s). Nếu đổi animation clip, cần update timer tương ứng.
