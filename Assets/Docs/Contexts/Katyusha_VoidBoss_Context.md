# Katyusha_VoidBoss_Context.md — Bối cảnh hệ thống Void Boss (Chapter 6)

> File chốt sổ kiến trúc cho Void Boss (Chapter 6).
> Mọi thay đổi FSM, Health, Spawner phải cập nhật vào file này.

---

## 1. KIẾN TRÚC TỔNG THỂ

### 1.1 Class Hierarchy

```
EnemyController (base)
  └── VoidBossController (override: Start, Update, Patrol, Pursue, ...)
        ├── stateCache["VoidIdle"]      = VoidIdleState          (CUSTOM)
        ├── stateCache["Stomp"]         = GenericAttackState     (generic)
        ├── stateCache["SpikePierce"]   = GenericAttackState     (generic)
        ├── stateCache["VoidSphere"]    = GenericAttackState     (generic)
        ├── stateCache["AmbushSummon"]  = GenericAttackState     (generic)
        ├── stateCache["Pursuit"]       = VoidPursuitState       (CUSTOM)
        ├── stateCache["BloodMoon"]     = BloodMoonState         (CUSTOM)
        ├── stateCache["Hurt"]          = HurtState              (generic)
        └── stateCache["Die"]           = DieState               (generic + callback)
```

### 1.2 Core Mechanics

| Mechanic | File | Mô tả |
|---|---|---|
| **FSM State Machine** | `VoidBossController.cs` | `IEnemyState`-based, state cache trong `Dictionary<string, IEnemyState>` |
| **AI Decision (Idle)** | `VoidIdleState.cs` | Khoảng cách + ưu tiên Cooldown, quyết định mỗi 0.5s |
| **Pursuit** | `VoidPursuitState.cs` | Walk đến Player, skill-check interrupt mỗi frame |
| **Ultimate** | `BloodMoonState.cs` | Multi-wave telegraph timer-based |
| **Super Armor (Bá thể)** | `VoidBossController.cs` | `GetHurtState()` override trả về `currentState` — không flinch |
| **Facing Lock** | `VoidBossController.cs` | Không Flip trong `GenericAttackState` — bảo toàn animation |
| **Ghost Collision** | Unity Layer Matrix | Boss Layer `Enemy` đi xuyên Player, không code va chạm |
| **Hitbox Layer** | Prefab Layer `EnemyAttack` | Prefab `OnTriggerEnter2D` tự xử lý damage |
| **Cleanup On Death** | `VoidBossController.cs` | Destroy toàn bộ prefab đang tồn tại trên map |
| **Blood Moon Anti-Overlap** | `VoidBossController.cs` | Random ±X, ±Y + `minSpacing` tránh đè vùng nổ |
| **Health & Damage** | `Health.cs` (base) | Không custom Health |
| **NA1 (Stomp)** | Animation Event → spawn prefab | AoE + `StunEffect` |
| **NA2 (Spike Pierce)** | Animation Event → spawn prefab | Sát thương vật lý cực lớn |
| **Skill 1 (Void Sphere)** | Animation Event → spawn prefab | Homing projectile + Debuff Giảm Giáp |
| **Skill 2 (Ambush Summon)** | Animation Event → spawn prefab | Trap tại chân Player, chém 1 nhát rồi biến mất |
| **Hurt State** | `HurtState` | Không dùng (Super Armor) |
| **Death** | `DieState` + `HandleEnemyDeath()` | 2s delay, cleanup, VFX, loot |

---

## 2. FSM STATE MACHINE

### 2.1 State Map

```
Sleep (khởi tạo, animator.Play "Void_Sleep")
  └── OnWakeUpComplete() → isAwake = true → UnlockFacing → ChangeState("VoidIdle")

VoidIdle (VoidIdleState) — CUSTOM
  ├── OnEnter: UnlockFacing, reset DECISION_INTERVAL (0.5s)
  ├── AI decision mỗi 0.5s:
  │     Nếu distance <= meleeRange:
  │       └── PickMeleeAttack() → LockFacing → Run=false → roll Stomp/SpikePierce
  │     Nếu distance > meleeRange:
  │       1. BloodMoon ready?   → UseBloodMoon()   → LockFacing, Run=false → "BloodMoon"
  │       2. Skill 2 ready?     → UseSkill2()       → LockFacing, Run=false → "AmbushSummon"
  │       3. Skill 1 ready?     → UseSkill1()       → LockFacing, Run=false → "VoidSphere"
  │       4. Cả skill đang CD   → SwitchTo("Pursuit")
  └── OnExit: (no-op)

Stomp / SpikePierce / VoidSphere / AmbushSummon (GenericAttackState)
  ├── OnEnter → combat.PlayAnimTrigger("Stomp"/...)
  ├── [FACING LOCK] Boss không Flip trong suốt thời gian này
  ├── Animation Event giữa clip:
  │     ├── SpawnVoidSphere() / SpawnAmbushTrap() / ...
  │     └── activeProjectiles.Add(instantiated)
  └── Timer (1.0s–1.2s) → SwitchTo("VoidIdle")  → UnlockFacing

Pursuit (VoidPursuitState) — CUSTOM
  ├── OnEnter → PlayAnimBool("Run", true)
  ├── OnUpdate mỗi frame:
  │     [Architect Decision: Option A] BloodMoon ready? → UseBloodMoon() → LockFacing, Run=false → "BloodMoon"
  │     │   (ưu tiên tuyệt đối, ngắt Pursuit ngay lập tức)
  │     Nếu ngoài melee range:
  │       Skill 2 ready? → UseSkill2() → LockFacing, Run=false → "AmbushSummon"
  │       Skill 1 ready? → UseSkill1() → LockFacing, Run=false → "VoidSphere"
  │     Nếu <= meleeRange → PickMeleeAttack() → LockFacing, Run=false
  │     Nếu mất vision    → "VoidIdle"
  │     else              → LookAtPlayer() + MoveTowardPlayer()
  └── OnExit → PlayAnimBool("Run", false)

BloodMoon (BloodMoonState) — CUSTOM
  ├── OnEnter → PlayAnimTrigger("BloodMoon"), reset wave counter
  ├── OnUpdate:
  │     waveTimer += dt
  │     Mỗi waveInterval → SpawnBloodMoonWave()
  │       └── Anti-Overlap: Random ±X, ±Y, cách tối thiểu minSpacing
  │     Hết waves → SwitchTo("VoidIdle")
  └── OnExit → (no-op)

Hurt (HurtState) — generic, playHurtTrigger = false
  └── KHÔNG BAO GIỜ vào

Die (DieState) — generic, duration = 2f
  ├── OnEnter → play "Die" trigger
  ├── Callback → HandleEnemyDeath()
  └── 2s → Destroy(gameObject)
```

### 2.2 State Cache Registration

```csharp
private void CacheBossStates()
{
    stateCache["VoidIdle"]     = new VoidIdleState();
    stateCache["Stomp"]       = new GenericAttackState("Stomp", 1.2f, "VoidIdle");
    stateCache["SpikePierce"] = new GenericAttackState("SpikePierce", 1.2f, "VoidIdle");
    stateCache["VoidSphere"]   = new GenericAttackState("VoidSphere", 1.0f, "VoidIdle");
    stateCache["AmbushSummon"] = new GenericAttackState("AmbushSummon", 1.0f, "VoidIdle");
    stateCache["Pursuit"]      = new VoidPursuitState();
    stateCache["BloodMoon"]    = new BloodMoonState();
    stateCache["Hurt"]         = new HurtState("VoidIdle", false);
    stateCache["Die"]          = new DieState(2f, () => HandleEnemyDeath());
}
```

### 2.3 Generic States Used

| State Class | File | Parameters |
|---|---|---|
| `GenericAttackState` | `States/Common/GenericAttackState.cs` | `(string animTrigger, float animDuration, string returnState)` |
| `HurtState` | `States/Common/HurtState.cs` | `(string returnState, bool playHurtTrigger)` |
| `DieState` | `States/Common/DieState.cs` | `(float dieDuration, Action onDeath)` |

### 2.4 State Transition Rules

- **Animation → State:** KHÔNG. Timer trong `GenericAttackState.OnUpdate()`.
- **Animation Event → Public Method:** `SpawnVoidSphere()`, `SpawnAmbushTrap()`, `SpawnBloodMoonWave()`, `OnAttackAnimEnd()`.
- **`OnAttackAnimEnd()` fallback:** `SwitchTo("VoidIdle")` nếu current là `GenericAttackState`.
- **Facing Lock:** Lock trước khi `SwitchTo` attack state, Unlock trong `VoidIdleState.OnEnter`.

---

## 3. SUPER ARMOR & FACING LOCK

### 3.1 GetHurtState() — "Bá thể"

```csharp
public override IEnemyState GetHurtState(IEnemyState currentState) => currentState;
```

Override trả về `currentState` → `ChangeState()` là no-op (OnExit/OnEnter cycle). Boss không flinch dưới mọi nguồn damage.

### 3.2 GetDieState()

```csharp
public override IEnemyState GetDieState() => stateCache["Die"];
```

### 3.3 Facing Lock

```csharp
private bool facingLocked;

public void LockFacing()   => facingLocked = true;
public void UnlockFacing() => facingLocked = false;

public override void LookAtPlayer()
{
    if (facingLocked) return;
    base.LookAtPlayer();
}
```

**Cơ chế:**
- `LockFacing()` gọi trong `PickMeleeAttack()`, `UseSkill1()`, `UseSkill2()`, `UseBloodMoon()` — trước khi SwitchTo attack state.
- `UnlockFacing()` gọi trong `VoidIdleState.OnEnter()` và `OnWakeUpComplete()`.
- `LookAtPlayer()` override kiểm tra flag → nếu locked, không Flip.
- `MoveTowardPlayer()` không override (non-virtual) nhưng không bao giờ chạy trong attack state vì GenericAttackState không gọi movement.

---

## 4. CLEANUP ON DEATH

Khi `HandleEnemyDeath()` được gọi, toàn bộ `VoidSphere`, `AmbushTrap`, `BloodMoonTelegraph` đang tồn tại trên map bị Destroy ngay lập tức.

### 4.1 Tracking List

```csharp
private List<GameObject> activeProjectiles = new();
```

Mỗi lần spawn (từ Animation Event), object được add vào list:
```csharp
public void SpawnVoidSphere()
{
    GameObject sphere = Instantiate(voidSpherePrefab, ...);
    activeProjectiles.Add(sphere);
}
```

### 4.2 Cleanup Flow

```csharp
public override void HandleEnemyDeath()
{
    isDead = true;

    for (int i = activeProjectiles.Count - 1; i >= 0; i--)
    {
        if (activeProjectiles[i] != null)
            Destroy(activeProjectiles[i]);
    }
    activeProjectiles.Clear();

    // ... VFX, healthbar, loot ...
}
```

**Chú ý:** Prefab tự huỷ (khi chạm đất/hết duration) sẽ để lại null entry. Vòng lặp reverse + null check xử lý sạch.

---

## 5. GHOST COLLISION & HITBOX LAYER

### 5.1 Layer Setup (Unity Editor)

| GameObject | Layer | Ghi chú |
|---|---|---|
| Boss | `Enemy` | BodyType Dynamic, Collider tĩnh |
| Player | `Player` | — |
| Prefab NA/Skill hitbox | `EnemyAttack` | Trigger Collider, `OnTriggerEnter2D` |
| Unity Layer Collision Matrix | `Enemy` ↔ `Player` = **IGNORE** | Boss chạy xuyên Player |

### 5.2 Nguyên tắc

- **KHÔNG code C#** xử lý va chạm giữa Boss và Player.
- Layer Collision Matrix của Unity quyết định: `Enemy` layer bỏ qua `Player` layer.
- Boss di chuyển xuyên Player — mượt mà, không cần check chặn đường.

### 5.3 Hitbox Prefab Layer

- Mọi Prefab đòn đánh (Stomp, SpikePierce, VoidSphere, AmbushTrap, BloodMoon) được instantiate ở Layer `EnemyAttack`.
- `EnemyAttack` layer **KHÔNG bị ignore** bởi `Player` → Collision Matrix cho phép `EnemyAttack` ↔ `Player` trigger.
- Prefab tự quản lý vòng đời: `OnTriggerEnter2D` → `Health.TakeDamage()` → tự Destroy.
- **TUYỆT ĐỐI không dùng** `Vector2.Distance` check cứng trong Controller.

---

## 6. AI DECISION LOGIC

### 6.1 VoidIdleState (Bộ não chính)

```
OnEnter:
  UnlockFacing()
  decisionTimer = 0.5s

Mỗi 0.5s:
  distance = movement.GetDistanceToPlayer()

  Nếu distance <= meleeRange:
    PickMeleeAttack()
      → PlayRun(false), LockFacing()
      → 50% Stomp / 50% SpikePierce
      → SwitchTo GenericAttackState

  Nếu distance > meleeRange:
    (ưu tiên giảm dần)
    1. BloodMoon ready?  → UseBloodMoon() → PlayRun(false), LockFacing() → "BloodMoon"
    2. Skill 2 ready?    → UseSkill2()     → PlayRun(false), LockFacing() → "AmbushSummon"
    3. Skill 1 ready?    → UseSkill1()     → PlayRun(false), LockFacing() → "VoidSphere"
    4. Cả skill đang CD  → SwitchTo("Pursuit")
```

### 6.2 VoidPursuitState (Truy đuổi + Interrupt)

```
Mỗi frame:
  distance = movement.GetDistanceToPlayer()

  [Architect Decision: Option A] BloodMoon ready?
    → UseBloodMoon() + "BloodMoon"   [interrupt — ưu tiên tuyệt đối]

  Nếu ngoài melee range:
    Skill 2 ready? → UseSkill2() + "AmbushSummon"   [interrupt]
    Skill 1 ready? → UseSkill1() + "VoidSphere"     [interrupt]

  Nếu trong melee range:
    PickMeleeAttack()

  Nếu mất tầm nhìn:
    SwitchTo("VoidIdle")

  else:
    LookAtPlayer()
    MoveTowardPlayer()
```

### 6.3 Cooldown Tracking

```csharp
skill1ReadyTime = Time.time + skill1Cooldown;      // 4s
skill2ReadyTime = Time.time + skill2Cooldown;      // 10s
bloodMoonReadyTime = Time.time + bloodMoonCooldown; // 45s
```

---

## 7. ATTACK MECHANICS

### 7.1 NA1 — Stomp

| Element | Detail |
|---|---|
| Trigger | `"Stomp"` (GenericAttackState) |
| Animation Event | `SpawnStompAoE()` |
| Prefab | stompAoEPrefab — Layer `EnemyAttack` |
| Effect | AoE damage + `StunEffect` (prefab `OnTriggerEnter2D` → Health.TakeDamage) |

### 7.2 NA2 — Spike Pierce

| Element | Detail |
|---|---|
| Trigger | `"SpikePierce"` (GenericAttackState) |
| Animation Event | `SpawnSpikePierce()` |
| Prefab | spikePiercePrefab — Layer `EnemyAttack` |
| Effect | Sát thương vật lý cực lớn đường thẳng |

### 7.3 Skill 1 — Void Sphere

| Element | Detail |
|---|---|
| Trigger | `"VoidSphere"` (GenericAttackState) |
| Animation Event | `SpawnVoidSphere()` |
| Prefab | `voidSpherePrefab` — Layer `EnemyAttack` |
| Spawn Position | `transform.position` (từ boss) |
| Post-spawn | Homing → Player (VoidSphere tự điều hướng) |
| On Impact | Damage + Micro-Stun + Slow + Debuff Giảm Giáp |
| Cooldown | 4s |

### 7.4 Skill 2 — Ambush Summon

| Element | Detail |
|---|---|
| Trigger | `"AmbushSummon"` (GenericAttackState) |
| Animation Event | `SpawnAmbushTrap()` |
| Prefab | `ambushTrapPrefab` — Layer `EnemyAttack` |
| Spawn Position | `player.position` (dưới chân Player) |
| Behavior | Trap trồi lên, chém 1 nhát, tự huỷ |
| On Hit | Damage lớn + `SilentEffect` (Câm lặng) |
| Cooldown | 10s |

### 7.5 Ultimate — Blood Moon

| Element | Detail |
|---|---|
| Trigger | `"BloodMoon"` (BloodMoonState) |
| Spawn Method | `SpawnBloodMoonWave()` gọi mỗi wave |
| Prefab | `bloodMoonTelegraphPrefab` — Layer `EnemyAttack` |
| Waves | 5 waves × 5 telegraphs/wave, interval 0.8s |
| Spawn Pattern | Random ±X (`Player.X ± a`) + ±Y (`Player.Y ± b`) + **Anti-Overlap** |
| Behavior | Telegraph → delay → nổ DPS cực khủng (prefab tự quản lý) |
| Cooldown | 45s |

#### Anti-Overlap Algorithm

```csharp
public void SpawnBloodMoonWave()
{
    float a = bloodMoonSpread;      // ±random X
    float b = bloodMoonSpread;      // ±random Y
    float minSpacing = 1.5f;
    List<Vector3> chosen = new();

    for (int attempt = 0; attempt < bloodMoonPerWave * 5; attempt++)
    {
        if (chosen.Count >= bloodMoonPerWave) break;

        Vector3 candidate = new(
            player.position.x + Random.Range(-a, a),
            player.position.y + Random.Range(-b, b),
            0f
        );

        bool tooClose = false;
        foreach (Vector3 p in chosen)
        {
            if (Vector3.Distance(candidate, p) < minSpacing)
            { tooClose = true; break; }
        }

        if (!tooClose)
        {
            chosen.Add(candidate);
            Instantiate(bloodMoonTelegraphPrefab, candidate, Quaternion.identity);
        }
    }
}
```

Mỗi wave sinh ra tới 5 telegraph tại vị trí ngẫu nhiên xung quanh Player, với khoảng cách tối thiểu `minSpacing` (1.5f) để các vùng nổ không đè lên nhau.

### 7.6 Animation Event Table

| Clip | Method | Timing | Note |
|---|---|---|---|
| `Stomp` | `SpawnStompAoE()` | Giữa clip | Prefab Layer `EnemyAttack` |
| `SpikePierce` | `SpawnSpikePierce()` | Giữa clip | Prefab Layer `EnemyAttack` |
| `VoidSphere` | `SpawnVoidSphere()` | Giữa clip | + `activeProjectiles.Add()` |
| `AmbushSummon` | `SpawnAmbushTrap()` | Giữa clip | + `activeProjectiles.Add()` |
| `BloodMoon` | (state tự quản lý) | N/A | + `activeProjectiles.Add()` |
| Any attack | `OnAttackAnimEnd()` | Cuối clip | Fallback: SwitchTo("VoidIdle") |

```csharp
public void OnAttackAnimEnd()
{
    if (currentState is GenericAttackState)
        SwitchTo("VoidIdle");
}
```

---

## 8. CUSTOM STATES

### 8.1 VoidIdleState

```csharp
public class VoidIdleState : IEnemyState
{
    private const float DECISION_INTERVAL = 0.5f;
    private float decisionTimer;

    public void OnEnter(...) {
        decisionTimer = DECISION_INTERVAL;
        if (ctx is VoidBossController boss) boss.UnlockFacing();
    }

    public void OnUpdate(...) {
        decisionTimer -= Time.deltaTime;
        if (decisionTimer > 0f) return;
        decisionTimer = DECISION_INTERVAL;
        if (ctx is VoidBossController boss) {
            float dist = movement.GetDistanceToPlayer();
            if (dist <= boss.MeleeRange) { boss.PickMeleeAttack(); return; }
            if (boss.IsBloodMoonReady()) { boss.UseBloodMoon(); SwitchTo("BloodMoon"); return; }
            if (boss.IsSkill2Ready())    { boss.UseSkill2();    SwitchTo("AmbushSummon"); return; }
            if (boss.IsSkill1Ready())    { boss.UseSkill1();    SwitchTo("VoidSphere"); return; }
            SwitchTo("Pursuit");
        }
    }
    public void OnExit(...) { }
}
```

### 8.2 VoidPursuitState

Chi tiết xem mục 6.2. Có `OnEnter/OnExit` quản lý `Run` bool.

### 8.3 BloodMoonState

```csharp
public class BloodMoonState : IEnemyState
{
    private int currentWave;
    private float waveTimer;

    public void OnEnter(...) {
        currentWave = 0; waveTimer = 0f;
        combat.PlayAnimTrigger("BloodMoon");
    }

    public void OnUpdate(...) {
        if (ctx is VoidBossController boss) {
            waveTimer += Time.deltaTime;
            if (waveTimer >= boss.BloodMoonWaveInterval) {
                waveTimer = 0f;
                boss.SpawnBloodMoonWave();
                currentWave++;
                if (currentWave >= boss.BloodMoonWaves)
                    ctx.SwitchTo("VoidIdle");
            }
        }
    }
    public void OnExit(...) { }
}
```

---

## 9. SERIALIZED FIELDS

```csharp
[Header("Boss Settings")]
[SerializeField] private float meleeRange = 2.5f;

[Header("Attack Prefabs")]
[SerializeField] private GameObject voidSpherePrefab;
[SerializeField] private GameObject ambushTrapPrefab;
[SerializeField] private GameObject bloodMoonTelegraphPrefab;

[Header("Skill Cooldowns")]
[SerializeField] private float skill1Cooldown = 4f;
[SerializeField] private float skill2Cooldown = 10f;
[SerializeField] private float bloodMoonCooldown = 45f;

[Header("Blood Moon Config")]
[SerializeField] private int bloodMoonWaves = 5;
[SerializeField] private float bloodMoonWaveInterval = 0.8f;
[SerializeField] private float bloodMoonSpread = 2.5f;
[SerializeField] private int bloodMoonPerWave = 5;
[SerializeField] private float bloodMoonMinSpacing = 1.5f;

[Header("Hurt Effect")]
[SerializeField] private SpriteRenderer bossSprite;
[SerializeField] private Color hurtTint = Color.red;
[SerializeField] private float hurtFlashDuration = 0.15f;

[Header("Death")]
[SerializeField] private GameObject deathVFX;
[SerializeField] private float dieDuration = 2f;
```

---

## 10. KEY FORMULAS

```
Melee Attack Range       = meleeRange (2.5f)
Blood Moon Spread        = ±a (2.5f) trên X, ±b (2.5f) trên Y
Blood Moon MinSpacing    = 1.5f
Waves                    = 5 waves × 5 telegraphs/wave
Wave Interval            = 0.8s

Cooldowns:
  Skill 1 (Void Sphere)    = 4s
  Skill 2 (Ambush Summon)  = 10s
  Ultimate (Blood Moon)    = 45s

Decision Interval         = 0.5s
GenericAttackState Timer  = 1.0s (skills) / 1.2s (melee)
Die Duration              = 2f
```

---

## 11. BỐN BỘ LUẬT VẬT LÝ / HITBOX

### Luật 1 — Facing Lock
- Trong `GenericAttackState` (Stomp, SpikePierce, VoidSphere, AmbushSummon), Boss bị khóa hướng.
- `LockFacing()` gọi trước `SwitchTo` attack state, `UnlockFacing()` gọi trong `VoidIdleState.OnEnter`.
- `LookAtPlayer()` override kiểm tra `facingLocked` → skip flip.
- `MoveTowardPlayer()` không override nhưng không chạy trong attack state.

### Luật 2 — Cleanup On Death
- `activeProjectiles` list lưu toàn bộ Prefab đã spawn.
- `HandleEnemyDeath()`: duyệt ngược list, `Destroy` từng object, `Clear()`.
- Prefab tự huỷ → null entry được xử lý an toàn.

### Luật 3 — Ghost Collision
- Boss Layer: `Enemy`, Player Layer: `Player`.
- Unity Layer Collision Matrix: `Enemy` × `Player` = **IGNORE**.
- **KHÔNG code C#** xử lý va chạm Boss–Player.

### Luật 4 — Hitbox Prefab Layer
- Mọi Prefab đòn đánh Layer: `EnemyAttack`.
- `EnemyAttack` × `Player` = Trigger (Collision Matrix cho phép).
- Prefab tự quản lý: `OnTriggerEnter2D` → `Health.TakeDamage()` → tự Destroy.
- **KHÔNG dùng** `Vector2.Distance` trong Controller để check damage.

---

## 12. KNOWN ISSUES / TODOs

| # | Issue | Priority | Ghi chú |
|---|---|---|---|
| 1 | Prefab `voidSpherePrefab` chưa implement | High | Homing projectile + hazard zone |
| 2 | Prefab `ambushTrapPrefab` chưa implement | High | Trap Layer `EnemyAttack`, tự huỷ |
| 3 | Prefab `bloodMoonTelegraphPrefab` chưa implement | High | Telegraph → delayed explosion |
| 4 | Prefab `stompAoEPrefab` / `spikePiercePrefab` chưa có | High | Cần spawn method + prefab field |
| 5 | Animator Controller chưa tạo trigger set | High | Stomp, SpikePierce, VoidSphere, AmbushSummon, BloodMoon, Die, Run |
| 6 | `SpawnStompAoE()` / `SpawnSpikePierce()` chưa có trong controller | Medium | Cần thêm method + prefab field |
| 7 | Boss animation clip length chưa đồng bộ `animDuration` | Medium | Sync sau khi có clip |
| 8 | `EnemyController.MoveTowardPlayer()` non-virtual | Low | Facing Lock an toàn vì GenericAttackState không gọi movement |

---

## 13. FILE MAP

```
Assets/Script/EnemyThing/
├── Boss/VoidBoss/
│   ├── VoidBossController.cs          ← FSM, Super Armor, Facing Lock,
│   │                                      Cleanup, Anti-Overlap BM
│   └── States/
│       ├── VoidIdleState.cs           ← AI Decision Brain
│       ├── VoidPursuitState.cs        ← Interruptible Pursuit
│       └── BloodMoonState.cs          ← Multi-wave Ultimate
│
├── Boss/BatBoss/   (existing)
│
├── States/Common/
│   ├── GenericAttackState.cs
│   ├── HurtState.cs
│   └── DieState.cs
│
    └── Core/
    ├── EnemyController.cs
    ├── IEnemyStateContext.cs
    ├── IEnemyMovement.cs
    └── IEnemyCombat.cs
```

---

## 14. ARCHITECT DECISIONS (2026-07-18)

### Decision 1 — Ultimate Priority (VoidBoss)

| Mục | Chi tiết |
|-----|----------|
| Vấn đề | `BloodMoon` hồi xong lúc Boss đang Pursuit → không được check → bỏ lỡ cơ hội xả ult |
| Giải pháp | **Option A** — Check `BloodMoon ready` đầu vòng lặp `VoidPursuitState.OnUpdate()`, trước skill 2/skill 1 |
| Phạm vi | Chỉ ngắt Pursuit. **TUYỆT ĐỐI KHÔNG** ngắt `GenericAttackState` đang chạy |
| File áp dụng | `VoidPursuitState.cs:18-22` |
| Trạng thái | ✅ Đã implement |

### Decision 2 — Số liệu định lượng (VoidBoss)

| Mục | Chi tiết |
|-----|----------|
| Vấn đề | Dùng Inspector field hay ScriptableObject config cho boss params |
| Giải pháp | **Option A** — `[SerializeField]` trực tiếp trong `VoidBossController`. Không SO riêng |
| Lý do | Các param đặc thù cho 1 boss, không dùng chung |
| Trạng thái | ✅ Giữ nguyên code hiện tại |

### Decision 3 — Cleanup on Death (VoidBoss)

| Mục | Chi tiết |
|-----|----------|
| Vấn đề | Boss chết giữa BloodMoon → wave chưa nổ có gây sát thương không? |
| Giải pháp | `HandleEnemyDeath()` duyệt `activeProjectiles` → Destroy tất cả. Null-check an toàn |
| File áp dụng | `VoidBossController.cs:248-267` |
| Trạng thái | ✅ Code hiện tại đã xử lý đúng |

### Decision 4 — BatBoss Atk Selection (50/50)

| Mục | Chi tiết |
|-----|----------|
| Vấn đề | Tiêu chí chọn Atk1 (DropSphere) vs Atk2 (SpawnDoT) |
| Giải pháp | **50/50 pure random** — không distance check, không conditional logic |
| File áp dụng | `BatBossController.PickNextAttack()` |
| Trạng thái | ✅ Giữ nguyên. Xem `Katyusha_BatBoss_Context.md` |
