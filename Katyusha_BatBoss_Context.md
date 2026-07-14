# Katyusha_BatBoss_Context.md — Handover Document

## Architecture Rules (Bất di bất dịch)
- **FSM base:** `IEnemyState` interface (`OnEnter` / `OnUpdate` / `OnExit`).
- **State transition:** `BatBossController.ChangeState()` / `SwitchTo(string)`.
- **Animation control:** ONLY `animator.SetTrigger()` allowed. NO `animator.Play()`, NO `bool` parameters (except `Run` which is set by generic states).
- **State cache:** `Dictionary<string, IEnemyState> stateCache` initialized in `CacheBossStates()`.
- **Base class:** `BatBossController : EnemyController` (overrides Patrol/Pursue/ExecuteAttack as no-ops).
- **DRY principle:** Boss reuses generic `HurtState`, `DieState`, `GenericAttackState`. Only `BatHoverState` remains unique.

---

## FSM State Map

```
Sleep (initial, animator.Play cho phép lúc này)
  └── OnWakeUpComplete() → isAwake = true → ChangeState("Hover")

Hover (BatHoverState) — unique, sine-wave floating
  ├── UpdateHover() — sin/cos wave movement
  ├── Timer 2s → PickNextAttack()
  │     ├── roll < 0.5 → SwitchTo("DropSphere")  [Atk1]
  │     └── else        → SwitchTo("SpawnDoT")    [Atk2]
  └── (Pillar spawn timer chạy riêng trong Update())

DropSphere / SpawnDoT (GenericAttackState)
  ├── OnEnter → combat.PlayAnimTrigger(trigger)  // "Atk1" or "Atk2"
  ├── Animation Event trong clip gọi:
  │     ├── DropSphere()      — instantiate BatSphere tại sphereSpawnPoints[random]
  │     └── SpawnAoECircle()  — instantiate holePrefab tại player position (ground level)
  └── Timer hết → SwitchTo("Hover")

Hurt (HurtState) — generic, no boss-specific class
  ├── Chỉ vào khi ForceHurtState() gọi từ BatHealth (Pillar explosion)
  ├── ForceHurtState() gọi FlashHurt() trước ChangeState
  ├── Không play "Hurt" trigger (playHurtTrigger = false)
  └── 0.3s → SwitchTo("Hover")

Die (DieState) — generic, with callback
  ├── OnEnter → play "Die" trigger
  ├── Callback → HandleEnemyDeath() (VFX, loot, OnBossDefeated event)
  └── 2s → Destroy(gameObject)
```

---

## BatHealth — Damage Filtering

| Source Type | Behavior |
|---|---|
| `null` / `Melee` / `Stand` / `EnemySkill` | Deflect (SFX), no damage |
| `Ranged` | Normal damage × 1.5 (bonus) |
| `Pillar` | Normal damage + gọi `ForceHurtState()` |
| `System` | Normal damage, no state change |

- **Override:** `BatHealth.TakeDamage(int, GameObject)` filters via `DamageSource.sourceType`.
- **Pillar trigger:** When `sourceType == Pillar`, calls `boss.ForceHurtState()` → `FlashHurt()` + `ChangeState(stateCache["Hurt"])`.

---

## Pillar Spawning System (Integrated in BatBossController)

| Parameter | Value |
|---|---|
| `maxActivePillars` | 3 |
| `pillarSpawnCooldown` | 7s |
| `maxPlayerDistance` | 12f (filter out points too far) |
| `minPillarDistance` | 5f (filter out points too close to existing pillars) |

**Flow:**
1. `Update()` → `UpdatePillarSpawning(dt)`
2. Removes null pillars from `activePillars` list
3. If count < max AND timer expired → `TrySpawnPillar()`
4. `TrySpawnPillar()` filters `pillarSpawnPoints[]` by distance rules
5. Instantiates `pillarPrefab` at random valid point → `Pillar.Init(this)`

**Pillar.cs:**
- `HP = 20`, destroyed by PlayerNA / Stand / Projectile triggers
- On destroy → `DestroyPillar()` → deal `PillarBurstDamage` (25% MaxHP) via `DamageSourceType.Pillar`
- Burst damage triggers boss `HurtState` via `BatHealth` → `ForceHurtState()`

---

## Delayed AoE Circle (Thay thế SpawnDoTZone cũ)

**Method:** `BatBossController.SpawnAoECircle()` (gọi từ Animation Event của clip `Atk2`)

```
Instantiate(holePrefab, playerPosition + groundYOffset, identity)
```

- **Vị trí:** Player's X position, Y = boss.transform.position.y - hoverHeight (ground level).
- **Division of labor:**
  - **Controller (Agent):** Chỉ instantiate prefab, không gọi Init / setup gì thêm.
  - **Prefab's own script (Client):** Xử lý toàn bộ logic delayed AoE (summon circle, check player trong vùng, burst damage tại animation frame).
- **Serialized field:** `[SerializeField] private GameObject holePrefab;` (gán trong Inspector).

---

## Animation Events (gán trong Animation clip)

| Clip | Event Method | Timing |
|---|---|---|
| `Attack1` (`Atk1`) | `DropSphere()` | Giữa clip |
| `Attack2` (`Atk2`) | `SpawnAoECircle()` | Giữa clip |
| Any attack | `OnAttackAnimEnd()` (optional) | Cuối clip |

---

## Các Serialized Fields trên BatBossController

```csharp
[Header("Boss Settings")]
hoverHeight, hoverSpeed, hoverAmplitude

[Header("Spawn Prefabs")]
batSpherePrefab, pillarPrefab, holePrefab
sphereSpawnPoints[], pillarSpawnPoints[]

[Header("Pillar Spawn Config")]
maxActivePillars=3, pillarSpawnCooldown=7f
maxPlayerDistance=12f, minPillarDistance=5f

[Header("Hurt Effect")]
bossSprite, hurtTint, hurtFlashDuration=0.15f

[Header("Death")]
deathVFX
```

---

## State Cache Registration (CacheBossStates)

```csharp
stateCache["Hover"]      = new BatHoverState();                    // unique
stateCache["DropSphere"] = new GenericAttackState("Atk1", 1.2f, "Hover");  // generic
stateCache["SpawnDoT"]   = new GenericAttackState("Atk2", 1.2f, "Hover");  // generic
stateCache["Hurt"]       = new HurtState("Hover", false);         // generic, no trigger
stateCache["Die"]        = new DieState(2f, () => HandleEnemyDeath()); // generic + callback
```

### Generic States Used:
- **`GenericAttackState`** (`States/Common/`): Timer-based attack with trigger name, duration, return state.
- **`HurtState`** (`States/Common/`): Return-to-state parameterized, optional hurt trigger.
- **`DieState`** (`States/Common/`): Configurable duration + `onDeath` callback.

### Deleted Boss-Specific States:
- ~~`BatDieState.cs`~~ → replaced by `DieState` with callback
- ~~`BatHurtState.cs`~~ → replaced by `HurtState("Hover", false)`
- ~~`BatAttackAnimState.cs`~~ → replaced by `GenericAttackState`

---

## Key Formulas

```
HoverPosition.y = hoverOrigin.y + hoverHeight + sin(hoverPhase * 0.7) * 0.5
HoverPosition.x = hoverOrigin.x + sin(hoverPhase) * hoverAmplitude

PillarBurstDamage = Mathf.RoundToInt(cachedMaxHP * 0.25f)
AoECircleSpawnY  = transform.position.y - hoverHeight
```

---

## Known Issues / TODOs

- `EnemyController.OnCollisionEnter2D` non-virtual → boss chạy collision handler base (vô hại, chỉ flip sprite).
- `Pillar` detect projectile via `GetComponent<ProjectilePref>()` — fragile, nên migrate sang tag/layer.
- `GenericAttackState.animDuration` cứng (1.2s). Nếu đổi clip length, cần update.
- `batSpherePrefab` vẫn còn logic HazardZone cũ bên trong BatSphere.cs (có thể deprecated nếu AoE circle mới thay thế hoàn toàn).
