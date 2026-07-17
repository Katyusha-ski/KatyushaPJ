# Katyusha_BatBoss_Context.md — Bối cảnh hệ thống BatBoss

> File chốt sổ kiến trúc cho BatBoss (Chapter 4).
> Mọi thay đổi FSM, Health, Spawner phải cập nhập vào file này.

---

## 1. KIẾN TRÚC TỔNG THỂ

### 1.1 Class Hierarchy

```
EnemyController (base)
  └── BatBossController (override: Start, Update, Patrol, Pursue, ...)
        ├── stateCache["Hover"]      = BatHoverState        (unique)
        ├── stateCache["DropSphere"] = GenericAttackState    (generic)
        ├── stateCache["SpawnDoT"]   = GenericAttackState    (generic)
        ├── stateCache["Hurt"]       = HurtState             (generic)
        └── stateCache["Die"]        = DieState              (generic + callback)
```

### 1.2 Core Mechanics

| Mechanic | File | Mô tả |
|---|---|---|
| **FSM State Machine** | `BatBossController.cs` | `IEnemyState`-based, state cache trong `Dictionary<string, IEnemyState>` |
| **Hover Movement** | `BatBossController.cs` + `BatHoverState.cs` | Sine/cosine floating, 2s timer → attack |
| **Health & Damage** | `BatHealth.cs` | Override `TakeDamage()`: deflect Melee, 1.5x Ranged, Pillar → ForceHurtState |
| **Pillar System** | `BatBossController.cs` + `Pillar.cs` | Passive spawner (Update), max 3, 7s cooldown, burst damage 25% MaxHP |
| **Atk1 (DropSphere)** | `BatBossController.cs` + `BatSphere.cs` | Animation Event → `DropSphere()` → instantiate sphere → HazardZone |
| **Atk2 (SpawnAoE)** | `BatBossController.cs` + `holePrefab` | Animation Event → `SpawnAoECircle()` → instantiate `holePrefab` tại Player |
| **Hurt State** | `BatBossController.cs` + `HurtState` | Chỉ khi Pillar nổ → FlashHurt (red tint 0.15s) → 0.3s → Hover |
| **Death** | `BatBossController.cs` + `DieState` | 2s delay, callback `HandleEnemyDeath()` |

---

## 2. FSM STATE MACHINE

### 2.1 State Map

```
Sleep (khởi tạo, animator.Play)
  └── OnWakeUpComplete() → isAwake = true → ChangeState("Hover")

Hover (BatHoverState) — unique
  ├── UpdateHover() — sine/cosine floating
  ├── Timer 2s → PickNextAttack()
  │     ├── roll < 0.5 → SwitchTo("DropSphere")  [Atk1]
  │     └── else        → SwitchTo("SpawnDoT")    [Atk2]
  │     [Architect Decision: 50/50 pure random — không distance check, không conditional]
  └── Pillar spawn timer chạy song song trong Update()

DropSphere / SpawnDoT (GenericAttackState)
  ├── OnEnter → combat.PlayAnimTrigger("Atk1"/"Atk2")
  ├── Animation Event trong clip gọi:
  │     ├── DropSphere()      → BatSphere tại sphereSpawnPoints[random]
  │     └── SpawnAoECircle()  → holePrefab tại player position (ground level)
  └── Timer 1.2s → SwitchTo("Hover")

Hurt (HurtState) — generic, playHurtTrigger = false
  ├── Chỉ vào khi ForceHurtState() gọi từ BatHealth (Pillar explosion)
  ├── FlashHurt() gọi trước ChangeState trong ForceHurtState()
  └── 0.3s → SwitchTo("Hover")

Die (DieState) — generic, duration = 2f
  ├── OnEnter → play "Die" trigger
  ├── Callback → HandleEnemyDeath() gọi trong OnUpdate trước Destroy
  └── 2s → Destroy(gameObject)
```

### 2.2 State Cache Registration

```csharp
private void CacheBossStates()
{
    stateCache["Hover"]      = new BatHoverState();
    stateCache["DropSphere"] = new GenericAttackState("Atk1", 1.2f, "Hover");
    stateCache["SpawnDoT"]   = new GenericAttackState("Atk2", 1.2f, "Hover");
    stateCache["Hurt"]       = new HurtState("Hover", false);
    stateCache["Die"]        = new DieState(2f, () => HandleEnemyDeath());
}
```

### 2.3 Generic States Used

| State Class | File | Parameters |
|---|---|---|
| `GenericAttackState` | `States/Common/GenericAttackState.cs` | `(string animTrigger, float animDuration, string returnState)` |
| `HurtState` | `States/Common/HurtState.cs` | `(string returnState, bool playHurtTrigger)` |
| `DieState` | `States/Common/DieState.cs` | `(float dieDuration, Action onDeath)` |

### 2.4 State Transition Rules

- **Animation → State:** KHÔNG. State chuyển bằng timer trong `GenericAttackState.OnUpdate()`.
- **Animation Event → Public Method:** Animation clip gọi `DropSphere()` / `SpawnAoECircle()` / `OnAttackAnimEnd()`.
- **`OnAttackAnimEnd()` fallback:** Safety net, gọi `SwitchTo("Hover")` nếu current state là `GenericAttackState`.

---

## 3. HEALTH SYSTEM

### 3.1 BatHealth Damage Filter

```
                    incoming damage + damageSource
                               │
                    ┌──────────┴──────────┐
                    │  damageSource == null│
                    └──────────┬──────────┘
                               │ (YES)
                         ┌─────┴─────┐
                         │  Deflect  │ → PlayDeflect SFX, return (0 damage)
                         └───────────┘
                               │ (NO)
                    ┌──────────┴──────────┐
                    │ GetComponent<        │
                    │   DamageSource>()    │
                    └──────────┬──────────┘
                               │ null
                         ┌─────┴─────┐
                         │  Deflect  │
                         └───────────┘
                               │ not null
                    ┌──────────┴──────────┐
                    │  sourceType switch  │
                    └──────────┬──────────┘
                               │
          ┌────────────────────┼────────────────────┐
          │ Ranged             │ Pillar             │ System
          ▼                    ▼                    ▼
   damage × 1.5          base.TakeDamage()    base.TakeDamage()
   base.TakeDamage()     ForceHurtState()     (no state change)
          │                    │                    │
   ┌──────┴──────┐      ┌─────┴─────┐             │
   │ Boss không  │      │ Vào Hurt  │             │
   │ vào Hurt    │      │ State     │             │
   │ (shield)    │      │ (flash)   │             │
   └─────────────┘      └───────────┘             │
          │                    │                    │
          └────────────────────┼────────────────────┘
                               │
                    ┌──────────┴──────────┐
                    │  Melee/Stand/        │
                    │  EnemySkill/default  │
                    └──────────┬──────────┘
                               │
                         ┌─────┴─────┐
                         │  Deflect  │
                         └───────────┘
```

### 3.2 Damage Source Types

| `DamageSourceType` | Đến từ | Effect on Boss |
|---|---|---|
| `Melee` | PlayerNA (normal attack) | Deflect (0 damage) |
| `Stand` | Hachiware companion | Deflect (0 damage) |
| `Ranged` | ProjectilePref (player skills) | 1.5x bonus damage |
| `Pillar` | Pillar explosion | Normal damage + ForceHurtState |
| `System` | Internal (future) | Normal damage, no state change |
| `EnemySkill` | Other enemies | Deflect (0 damage) |
| `null` | Fallback | Deflect (0 damage) |

### 3.3 GetHurtState() — "Khiên chống khựng"

```csharp
public override IEnemyState GetHurtState(IEnemyState currentState) => currentState;
```

**Tại sao override này tồn tại:**

1. `Health.TakeDamage()` (base class) tự động gọi `GetHurtState()` cho tất cả enemy:
   ```csharp
   // Health.cs:123-131
   else if(gameObject.CompareTag("Enemy"))
   {
       EnemyController ec = GetComponent<EnemyController>();
       IEnemyState hurtState = ec.GetHurtState(ec.GetCurrentState());
       ec.ChangeState(hurtState);
   }
   ```
2. Boss set `stateFactory = null`, nếu không override sẽ **NullReferenceException**.
3. Override trả về `currentState` → `ChangeState()` là no-op → Boss không flinch.
4. **Chỉ** `ForceHurtState()` (từ Pillar explosion) mới đưa Boss vào HurtState.

### 3.4 GetDieState() — Death Entry Point

```csharp
public override IEnemyState GetDieState() => stateCache["Die"];
```

- `Health.Die()` gọi `GetDieState()` khi HP ≤ 0.
- `stateCache["Die"]` là `DieState(2f, () => HandleEnemyDeath())`.
- `HandleEnemyDeath()` → VFX, healthbar hide, loot, `OnBossDefeated` event.

---

## 4. PILLAR SPAWNING SYSTEM

### 4.1 Parameters

| Field | Value | Ý nghĩa |
|---|---|---|
| `maxActivePillars` | 3 | Số pillar tối đa cùng lúc |
| `pillarSpawnCooldown` | 7s | Thời gian chờ giữa các lần spawn |
| `maxPlayerDistance` | 12f | Pillar cách Player tối đa (filter) |
| `minPillarDistance` | 5f | Pillar cách pillar khác tối thiểu (filter) |

### 4.2 Flow

```
Update()
  └── UpdatePillarSpawning(dt)
        ├── RemoveAll(null) — dọn pillar đã phá huỷ
        ├── if count >= maxActivePillars → reset timer, return
        ├── pillarSpawnTimer -= dt
        └── if timer ≤ 0 → TrySpawnPillar()
              ├── RemoveAll(null)
              ├── Filter pillarSpawnPoints[]:
              │     ├── pt == null → skip
              │     ├── distance to Player > 12f → skip
              │     ├── distance to any active pillar < 5f → skip
              │     └── validPoints.Add(pt)
              ├── if validPoints == 0 → return false
              ├── Random valid point → Instantiate(pillarPrefab)
              └── pillar.Init(this)

Pillar.cs:
  ├── HP = 20
  ├── OnTriggerEnter2D:
  │     ├── Player (PlayerNA / Stand) → TakeHit(1)
  │     └── ProjectilePref → TakeHit(1)
  ├── TakeHit(amount): currentHP -= amount, HitFlash
  ├── if currentHP ≤ 0 → DestroyPillar()
  │     ├── destroySFX + destroyVFX
      │   └── add DamageSource(Pillar) → bossHealth.TakeDamage(PillarBurstDamage, gameObject)
  │           └── ForceHurtState() → FlashHurt + HurtState(0.3s) → Hover
  └── PillarBurstDamage = round(cachedMaxHP * 0.25f)
```

---

## 5. ATTACK MECHANICS

### 5.1 Atk1 — DropSphere (BatSphere)

| Element | Detail |
|---|---|
| Trigger | `"Atk1"` (set bởi `GenericAttackState`) |
| Animation Event | `DropSphere()` |
| Prefab | `batSpherePrefab` |
| Spawn Position | `sphereSpawnPoints[random]` |
| Post-spawn | `BatSphere.Init(player)` — tự điều hướng đến Player |
| On Impact | Tạo `HazardZone` (damage vùng) |

### 5.2 Atk2 — SpawnAoE (HolePrefab)

| Element | Detail |
|---|---|
| Trigger | `"Atk2"` (set bởi `GenericAttackState`) |
| Animation Event | `SpawnAoECircle()` |
| Prefab | `holePrefab` (delayed AoE circle) |
| Spawn Position | `player.position` với `Y = transform.position.y - hoverHeight` (ground level) |
| Post-spawn | Prefab tự xử lý animation circle → delayed burst → damage vùng |

### 5.3 Animation Event Table

| Clip | Method | Timing |
|---|---|---|
| `Attack1` (`Atk1`) | `DropSphere()` | Giữa clip (khi tay quái vật vung ra) |
| `Attack2` (`Atk2`) | `SpawnAoECircle()` | Giữa clip |
| Any attack | `OnAttackAnimEnd()` | Cuối clip (fallback an toàn) |

```csharp
// Fallback safety — nếu GenericAttackState timer failed
public void OnAttackAnimEnd()
{
    if (currentState is GenericAttackState)
        SwitchTo("Hover");
}
```

---

## 6. UNIQUE STATE: BatHoverState

### 6.1 Code

```csharp
public class BatHoverState : IEnemyState
{
    private float timer; // 2s

    public void OnEnter(...) { timer = 2f; }

    public void OnUpdate(...)
    {
        if (ctx is BatBossController boss)
        {
            boss.UpdateHover(Time.deltaTime);  // sine/cosine float
            timer -= Time.deltaTime;
            if (timer <= 0f) boss.PickNextAttack();
        }
    }

    public void OnExit(...) { }
}
```

### 6.2 Hover Formula

```
hoverPhase += dt * hoverSpeed
xOff = sin(hoverPhase) * hoverAmplitude
yOff = sin(hoverPhase * 0.7f) * 0.5f

targetPosition:
  x = hoverOrigin.x + xOff
  y = hoverOrigin.y + hoverHeight + yOff

transform.position = lerp(transform.position, target, dt * 2f)
```

---

## 7. SERIALIZED FIELDS

```csharp
[Header("Boss Settings")]
[SerializeField] private float hoverHeight = 4f;
[SerializeField] private float hoverSpeed = 0.8f;
[SerializeField] private float hoverAmplitude = 1.5f;

[Header("Spawn Prefabs")]
[SerializeField] private GameObject batSpherePrefab;    // Atk1 projectile
[SerializeField] private GameObject pillarPrefab;       // Passive spawn
[SerializeField] private GameObject holePrefab;         // Atk2 AoE
[SerializeField] private Transform[] sphereSpawnPoints;
[SerializeField] private Transform[] pillarSpawnPoints;

[Header("Pillar Spawn Config")]
[SerializeField] private int maxActivePillars = 3;
[SerializeField] private float pillarSpawnCooldown = 7f;
[SerializeField] private float maxPlayerDistance = 12f;
[SerializeField] private float minPillarDistance = 5f;

[Header("Hurt Effect")]
[SerializeField] private SpriteRenderer bossSprite;
[SerializeField] private Color hurtTint = Color.red;
[SerializeField] private float hurtFlashDuration = 0.15f;

[Header("Death")]
[SerializeField] private GameObject deathVFX;
```

---

## 8. KEY FORMULAS

```
Hover Position:
  x = hoverOrigin.x + sin(hoverPhase) * hoverAmplitude
  y = hoverOrigin.y + hoverHeight + sin(hoverPhase * 0.7) * 0.5

PillarBurstDamage = Round(cachedMaxHP * 0.25f)
AoECircleSpawnY  = transform.position.y - hoverHeight

Ranged Damage    = incoming * 1.5  (bonus multiplier)
```

---

## 9. KNOWN ISSUES / TODOs

| # | Issue | Priority | Ghi chú |
|---|---|---|---|
| 1 | `EnemyController.OnCollisionEnter2D` non-virtual → boss runs base handler (harmless, only flips sprite) | Low | Có thể virtual hoá nếu cần override |
| 2 | `Pillar` detect projectile bằng `GetComponent<ProjectilePref>()` — fragile | Medium | Migrate sang tag/layer |
| 3 | `GenericAttackState.animDuration` hardcode 1.2s | Medium | Đồng bộ với clip length thực tế |
| 4 | `batSpherePrefab` vẫn giữ logic HazardZone cũ trong BatSphere.cs | Low | Có thể deprecated nếu AoE circle mới thay thế |
| 5 | Save/Load dùng `itemName` string — fragile khi rename | High | Migrate sang `itemId` |

---

## 10. DELETED FILES (Refactor Log)

| File | Replaced By | Lý do |
|---|---|---|
| `BatAttackAnimState.cs` | `GenericAttackState.cs` (States/Common/) | DRY — tham số hoá hoàn toàn |
| `BatHurtState.cs` | `HurtState` với `(returnState: "Hover", playHurtTrigger: false)` | DRY — generic state đã hỗ trợ |
| `BatDieState.cs` | `DieState` với `(duration: 2f, onDeath: HandleEnemyDeath)` | DRY — generic state + callback |

---

## 11. FILE MAP

```
Assets/Script/EnemyThing/
├── Boss/BatBoss/
│   ├── BatBossController.cs          ← FSM orchestrator
│   ├── BatHealth.cs                   ← Health override (deflect, 1.5x, pillar)
│   ├── Pillar.cs                      ← Passive pillar logic
│   ├── BatSphere.cs                   ← Atk1 projectile
│   ├── BatHole.cs                     ← Atk2 delayed AoE
│   ├── HazardZone.cs                  ← Damage zone
│   ├── BossArenaController.cs         ← Arena control
│   ├── BossHealthBarUI.cs             ← UI health bar
│   ├── States/
│   │   ├── BatHoverState.cs           ← UNIQUE - sine cosine hover
│   │   └── (BatDieState.cs / BatHurtState.cs / BatAttackAnimState.cs → DELETED)
│   └── DamageSource.cs
│
├── States/Common/
│   ├── GenericAttackState.cs          ← (trigger, duration, returnState)
│   ├── HurtState.cs                   ← (returnState, playHurtTrigger)
│   └── DieState.cs                    ← (duration, onDeath)
│
    └── Core/
    ├── EnemyController.cs             ← Base
    ├── IEnemyStateContext.cs
    ├── IEnemyMovement.cs
    └── IEnemyCombat.cs
```

---

## 12. ARCHITECT DECISIONS (2026-07-18)

### Decision — Atk Selection (50/50 Pure Random)

| Mục | Chi tiết |
|-----|----------|
| Vấn đề | Tiêu chí chọn Atk1 (DropSphere) vs Atk2 (SpawnDoT) |
| Quyết định | **50/50 pure random** — `Random.value < 0.5f` → DropSphere, else → SpawnDoT |
| Cấm | **KHÔNG** thêm distance check, health check, hay bất kỳ conditional logic nào vào `PickNextAttack()` |
| File áp dụng | `BatBossController.PickNextAttack()` (dòng 114-121) |
| Lý do | Giữ tính bất định của boss pattern, tránh exploit |
| Trạng thái | ✅ Giữ nguyên code hiện tại |
