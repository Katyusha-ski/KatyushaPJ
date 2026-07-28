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
VoidIdle (VoidIdleState) — CUSTOM
  ├── Khởi tạo: isAwake = false, animation Idle chạy xuyên suốt.
  │     AI decision KHÔNG chạy khi chưa aggro.
  │     Khi Player vào vùng phát hiện → WakeUpFromAggro()
  │     → isAwake = true → AI decision bắt đầu.
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
  ├── Animation Event tại ranh giới frame 2 (riêng VoidSphere):
  │     ├── SpawnVoidSphere() tại time 0.1667 (frame 2) — ranh giới cầu tách khỏi Boss
  │     ├── Sau Instantiate → animator.Play() từ offset normalizedTime = 2/totalFrames
  │     └── activeProjectiles.Add(instantiated)
  ├── Các clip khác (Stomp/SpikePierce/AmbushSummon): Animation Event giữa clip
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
- **Wake-up (Aggro):** `WakeUpFromAggro()` set `isAwake = true`, UnlockFacing, SwitchTo("VoidIdle").
  - **Không còn Sleep clip** — Boss dùng Idle animation xuyên suốt (SỬA 1).
  - Cơ chế trigger "vùng phát hiện Player" gọi `WakeUpFromAggro()` — đã implement qua `VoidAggroTrigger.cs` (SỬA 8).

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
- `UnlockFacing()` gọi trong `VoidIdleState.OnEnter()` và `WakeUpFromAggro()`.
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
    // Fly clip starts from frame 0 (no cast frames on projectile)
    Animator sphereAnim = sphere.GetComponent<Animator>();
    if (sphereAnim != null && sphereAnim.runtimeAnimatorController != null
        && sphereAnim.runtimeAnimatorController.animationClips.Length > 0)
    {
        string clipName = sphereAnim.runtimeAnimatorController.animationClips[0].name;
        sphereAnim.Play(clipName, 0, 0f);
    }
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

- Stomp/SpikePierce: damage xử lý trực tiếp trong Controller qua `OverlapCircleAll`/`OverlapBoxAll` — không cần Prefab hitbox riêng. Các đòn khác (VoidSphere, AmbushTrap, BloodMoon) dùng Prefab riêng, instantiate ở Layer `EnemyAttack`.
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
| Damage Method | `Physics2D.OverlapCircleAll(transform.position, stompRadius, playerLayer)` — trực tiếp trong Controller, không cần Prefab hitbox |
| Damage | `stompDamage = 15` (PLACEHOLDER) |
| Effect | `StunEffect(stompStunDuration = 1s)` lên Player |

### 7.2 NA2 — Spike Pierce

| Element | Detail |
|---|---|
| Trigger | `"SpikePierce"` (GenericAttackState) |
| Animation Event | `SpawnSpikePierce()` |
| Damage Method | `Physics2D.OverlapBoxAll(center, size, 0f, playerLayer)` — hình chữ nhật dài theo hướng Boss đang face, không cần Prefab hitbox |
| Damage | `spikeDamage = 25` (PLACEHOLDER) |
| Effect | Sát thương vật lý đường thẳng, không debuff |

### 7.3 Skill 1 — Void Sphere

| Element | Detail |
|---|---|
| Trigger | `"VoidSphere"` (GenericAttackState) |
| Animation Event | `SpawnVoidSphere()` — **tại ranh giới frame 2** (time 0.1667) |
| Animation Timing | Frame 0–2: Boss vận cầu (cầu dính vào sprite Boss). Frame 2 trở đi: cầu tồn tại độc lập |
| Prefab | `voidSpherePrefab` — Layer `EnemyAttack` |
| Prefab Animation | Chia 2 clip: `VoidSphere_Fly` (frame 2-4, 3 frame, 12fps, looping) + `VoidSphere_Explode` (frame 5-13, 9 frame, 12fps, non-looping). `sphereAnim.Play("VoidSphere_Fly", 0, 0f)` khi spawn. `VoidSphere.cs` gọi `animator.SetTrigger("Explode")` khi `OnTriggerEnter2D` (Player). Animation Event `OnVoidSphereExplodeEnd()` → ReturnToPool/Destroy |
| Prefab Script | `VoidSphereProjectile.cs` (standalone) — gắn trực tiếp trên prefab. Homing: `Rigidbody2D.velocity` → Player mỗi FixedUpdate |
| Spawn Position | `transform.position` (từ boss) |
| Post-spawn | Homing → Player (VoidSphere tự điều hướng) |
| Damage | `damage = 20` (PLACEHOLDER) |
| Timeout | `lifeTime = 5s` — hết 5s chưa trúng Player → tự Explode (dùng chung clip/effect Explode) → Destroy |
| On Impact | `OnTriggerEnter2D` → `Health.TakeDamage(damage)` + `StunEffect(0.5s)` + `StatModifierEffect(Slow: -50% MovementSpeed, 2s)` + `StatModifierEffect(Giảm Giáp: -15 Armor, 3s)` → `Destroy(gameObject)` |
| Cooldown | 4s |

### 7.4 Skill 2 — Ambush Summon

| Element | Detail |
|---|---|---|
| Trigger | `"AmbushSummon"` (GenericAttackState) |
| Animation Event | `SpawnAmbushTrap()` |
| Prefab | `ambushTrapPrefab` — Layer `EnemyAttack` |
| Spawn Position | `player.position + offset` — Hướng spawn xác định theo vị trí Player so với tâm map (x=0): Player bên trái map → trap spawn bên PHẢI Player (`Vector3.right * spawnOffsetDistance`); Player bên phải map → trap spawn bên TRÁI Player (`Vector3.left * spawnOffsetDistance`). Giả định tâm map luôn ở world x=0. |
| Spawn Offset | `spawnOffsetDistance = 2f` (`[SerializeField]`) |
| Prefab Behavior | Sprite tĩnh (không animation). Chờ `delayBeforeStrike` giây → tính 1 vector hướng về vị trí Player tại thời điểm bắt đầu lao (không homing) → dash theo đường thẳng với `dashSpeed` trong `Update()`. Dừng khi: (1) trúng Player → damage + `SilentEffect` → Destroy ngay; (2) dash hết `maxDashDistance` mà chưa trúng → tự Destroy. |
| Prefab Fields | `[SerializeField] float delayBeforeStrike`, `int damage`, `float silentDuration`, `float dashSpeed`, `float maxDashDistance` |
| Damage | `damage = 35` (PLACEHOLDER) |
| On Hit | Damage + `SilentEffect(silentDuration = 2s)` (Câm lặng) |
| Cooldown | 10s |

### 7.5 Ultimate — Blood Moon

| Element | Detail |
|---|---|
| Trigger | `"BloodMoon"` (BloodMoonState) |
| Spawn Method | `SpawnBloodMoonWave()` gọi mỗi wave |
| Prefab | `bloodMoonTelegraphPrefab` — Layer `EnemyAttack` |
| Prefab Script | `BloodMoonTelegraphController.cs` (standalone) — **gắn trực tiếp trên prefab** (không AddComponent runtime). `damageRadius`/`damageAmount` là `[SerializeField]` |
| Waves | 5 waves × 5 telegraphs/wave, interval 0.8s |
| Spawn Pattern | Random ±X (`Player.X ± a`) + ±Y (`Player.Y ± b`) + **Anti-Overlap** |
| Animation | `BloodExplosion.controller` + `BloodExplosion.anim` (8 frame, 12fps, non-looping). Frame 0 = telegraph indicator (Blood Explosion-Sheet_0) |
| Damage Timing | `DealAoEDamage()` tại frame 3 (time 0.25) — OverlapCircleAll (r=2.5f) → `Health.TakeDamage(30)` — **chỉ 1 lần** |
| Cleanup Timing | `OnExplosionAnimEnd()` tại frame cuối (time 0.58333) → `ObjectPool.ReturnToPool()` (không Destroy trực tiếp) |
| Anti-Double-Damage | `hasDealtDamage` flag reset trong `OnEnable()`, guard trong `DealAoEDamage()` |
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
| `VoidBoss_Atk1` (Stomp) | `SpawnStompAoE()` | 0.4167 (mid clip — impact) | + `OnAttackAnimEnd()` tại 0.6667 |
| `VoidBoss_Atk2` (SpikePierce) | `SpawnSpikePierce()` | 0.25 (mid clip — impact) | + `OnAttackAnimEnd()` tại 0.4167 |
| `VoidBoss_Skill1` (VoidSphere) | `SpawnVoidSphere()` | **Ranh giới frame 2** (0.1667) — cầu dính Boss frame 0-2 | Prefab play `VoidSphere_Fly` từ frame 0. Prefab `OnTriggerEnter2D` → `SetTrigger("Explode")` → `VoidSphere_Explode`. `OnAttackAnimEnd()` tại 0.4667 |
| `VoidBoss_Skill2` (AmbushSummon) | `SpawnAmbushTrap()` | 0.3333 (mid clip — triệu hồi) | + `OnAttackAnimEnd()` tại 0.5 |
| `VoidBoss_Skill3` (BloodMoon) | (state tự quản lý wave timer) | N/A — đã xoá event | `BloodMoonState.OnUpdate()` spawn wave mỗi 0.8s. Chỉ giữ `OnAttackAnimEnd()` an toàn |
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
[SerializeField] private float meleeRange = 6f;
// visionRange kế thừa từ EnemyController, set = 8f trong OnValidate()

[Header("Attack Prefabs")]
[SerializeField] private GameObject voidSpherePrefab;
[SerializeField] private GameObject ambushTrapPrefab;
[SerializeField] private GameObject bloodMoonTelegraphPrefab;

[Header("Stomp")]
[SerializeField] private float stompRadius = 3f;
[SerializeField] private int stompDamage = 15;          // PLACEHOLDER
[SerializeField] private float stompStunDuration = 1f;

[Header("Spike Pierce")]
[SerializeField] private float spikeRange = 5f;
[SerializeField] private float spikeWidth = 1.5f;
[SerializeField] private int spikeDamage = 25;          // PLACEHOLDER

[Header("Layer Masks")]
[SerializeField] private LayerMask playerLayer;

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

[Header("Ambush Trap")]
[SerializeField] private float spawnOffsetDistance = 2f;

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
Melee Attack Range       = meleeRange (6f) — TÂM Boss tới Player
Vision Range             = visionRange (8f) — Mất tầm nhìn → về VoidIdle
Blood Moon Spread        = ±a (2.5f) trên X, ±b (2.5f) trên Y
Blood Moon MinSpacing    = 1.5f
Waves                    = 5 waves × 5 telegraphs/wave
Wave Interval            = 0.8s

Ambush Trap Spawn Offset  = spawnOffsetDistance (2f)
  Player.x < 0  → spawnPos = player.position + Vector3.right * offset
  Player.x >= 0 → spawnPos = player.position + Vector3.left  * offset
Ambush Trap Dash          = delayBeforeStrike (0.5s), dashSpeed (10f), maxDashDistance (4f)
  Sau delay: dash 1 đường thẳng fixed hướng về Player tại thời điểm bắt đầu lao

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

## 12. BOSS REVEAL CAMERA

### 12.1 Cơ chế

Khi Player vào vùng Aggro của VoidBoss (`VoidAggroTrigger`):

1. `VoidAggroTrigger.OnTriggerEnter2D()` disable collider của trigger để không kích hoạt lại.
2. Gọi `CameraFollow.ZoomToBossReveal()` — coroutine:
   - Lerp `orthographicSize` từ size hiện tại → `bossRevealZoomSize` trong `zoomOutDuration` giây.
   - Trong lúc Lerp, camera theo trung điểm Player + Boss (không chỉ Player).
   - Giữ `holdDuration` giây.
   - Lerp ngược lại size gốc trong `zoomInDuration` giây.
3. Hết cinematic → callback → `parentController.WakeUpFromAggro()` → Boss bắt đầu AI.

### 12.2 CameraFollow fields

```
[Header("Boss Reveal Zoom")]
[SerializeField] private float bossRevealZoomSize = 8f;
[SerializeField] private float zoomOutDuration = 1.5f;
[SerializeField] private float holdDuration = 1f;
[SerializeField] private float zoomInDuration = 1f;
```

### 12.3 Flow

```
Player vào trigger
  → trigger collider disabled (chạy 1 lần)
  → CameraZoomReveal (coroutine)
  │   ├── zoomOut (1.5s): focus midpoint Player+Boss
  │   ├── hold (1s)
  │   └── zoomIn (1s): về follow Player bình thường
  └── onComplete → boss.WakeUpFromAggro() → AI bắt đầu
```

### 12.4 Field cần Architect chỉnh số

| Field | File | Giá trị |
|---|---|---|
| `bossRevealZoomSize` | `CameraController.cs` | 8f |
| `zoomOutDuration` | `CameraController.cs` | 1.5s |
| `holdDuration` | `CameraController.cs` | 1s |
| `zoomInDuration` | `CameraController.cs` | 1s |

---

## 13. KNOWN ISSUES / TODOs

| # | Issue | Priority | Ghi chú |
|---|---|---|---|
| 1 | ~~Prefab `voidSpherePrefab` chưa implement~~ | ~~High~~ | ✅ **Hoàn tất.** VoidSphere.prefab có Animator + controller `VoidSphere_Projectile.controller` + `VoidSphere_Projectile.anim` (14 frame từ Void sphere-Sheet.png, 12fps, looping). Default sprite là Void sphere-Sheet_0 |
| 2 | ~~Prefab `ambushTrapPrefab` chưa implement~~ | ~~High~~ | ✅ **Đã tạo** `Assets/Resources/Prefab/Effect/AmbushTrap.prefab`. SpriteRenderer + Rigidbody2D + BoxCollider2D(trigger) + DamageSource(EnemySkill) |
| 3 | ~~Prefab `bloodMoonTelegraphPrefab` chưa implement~~ | ~~High~~ | ✅ **Đã tạo** `Assets/Resources/Prefab/Effect/BloodMoonTelegraph.prefab`. SpriteRenderer + Rigidbody2D + BoxCollider2D(trigger) + DamageSource(EnemySkill) |
| 4 | ~~Prefab `stompAoEPrefab` / `spikePiercePrefab` chưa có~~ | ~~High~~ | ❌ **Đã đổi hướng:** Không dùng Prefab hitbox riêng. Stomp/SpikePierce dùng `OverlapCircleAll`/`OverlapBoxAll` trực tiếp trong Controller (SỬA 4). Prefab StompAoE/SpikePierce đã tạo trước đó **không dùng nữa** |
| 5 | ~~Animator Controller chưa tạo trigger set~~ | ~~High~~ | ✅ **Đã sửa**. `VoidBoss.controller` có 7 parameters: Run(Bool), Stomp/SpikePierce/VoidSphere/AmbushSummon/BloodMoon/Die(Triggers). AnyState transitions cho tất cả triggers. Thêm VoidBoss_Die state (tạm dùng Idle anim — chờ animation thật) |
| 6 | ~~`VoidBoss.prefab` chưa tồn tại~~ | ~~High~~ | ✅ **Đã tạo** `Assets/Resources/Prefab/Enemy/VoidBoss.prefab` gồm: Transform, SpriteRenderer, Animator (VoidBoss.controller), VoidBossController, Rigidbody2D, BoxCollider2D, Health(300HP), CharacterStats. `bossSprite` tự trỏ SpriteRenderer. **Architect cần:** assign prefab references (hitbox prefabs), assign deathVFX, thêm collider hitbox (trigger) nếu cần |
| 7 | Boss animation clip length chưa đồng bộ `animDuration` | Medium | Animation hiện tại (Atk1/Atk2, timing lệch design) là bản placeholder dựng tạm để test flow — KHÔNG phải bản final, chưa cần sync animDuration/tên trigger. Sẽ đồng bộ lại khi có animation chính thức |
| 8 | `EnemyController.MoveTowardPlayer()` non-virtual | Low | Facing Lock an toàn vì GenericAttackState không gọi movement |
| 9 | ~~Bug B14: Double Destroy telegraph khi pool null~~ | ~~Critical~~ | ✅ **Đã fix**: `activeTelegraphs` tách riêng khỏi `activeProjectiles`. Telegraph CHỈ vào 1 list, cleanup xử lý đúng 1 lần |
| 10 | ~~Bug B15: ObjectPool thiếu tracking borrowed vs available~~ | ~~Medium~~ | ✅ **Đã fix** — Option B (PoolMember). Queue chỉ chứa available object. Thêm `Debug.LogWarning("[ObjectPool] Pool '...' exhausted...")` khi queue cạn. Nếu thấy warning này thường xuyên, cần tăng pool size trong Inspector |
| 11 | **TODO** Animation Die chưa có cho bất kỳ Boss nào | Medium | `VoidBoss_Die.anim` là placeholder (6 sprite keyframe từ sheet VoidBoss, non-looping, 0.83s). Animation dùng sprite Idle. Cần asset artist tạo sprite Die riêng. Xem `KatyushaPJ_Boss_System_Summary.md` mục 5.5 |
| 12 | **TODO** Assign hitbox prefabs vào VoidBoss.prefab Inspector | High | voidSpherePrefab, ambushTrapPrefab, bloodMoonTelegraphPrefab đang `{fileID: 0}`. ~~stompAoEPrefab/spikePiercePrefab~~ không cần nữa — dùng Overlap check trực tiếp. Cần kéo prefab vào Inspector trong Unity |
| 13 | **TODO** BossHealthBarUI trong scene | High | Prefab đã tạo tại `Assets/Resources/Prefab/UI/BossHealthBarUI.prefab`. **Architect cần:** kéo vào Canvas scene, gọi `SetHealthBar()` từ script scene |
| 14 | **TODO** VoidBoss maxHealth chưa chốt số | High | Prefab hiện để tạm 500HP — con số tự đặt, **không có trong design**. Architect cần xác nhận hoặc assign trong Inspector |
| 15 | **TODO** VoidBoss_Die.anim .meta GUID đã có | Done | `9ebbb5e1bfb812144bbe36ba82b18336`. Đã cập nhật vào controller Die state |
| 16 | ~~VoidSphere_Projectile.anim + controller~~ | ~~Done~~ | ĐÃ XOÁ. Thay bằng `VoidSphere_Fly.anim` (frame 2-4, loop) + `VoidSphere_Explode.anim` (frame 5-13, non-loop, event OnVoidSphereExplodeEnd) + `VoidSphere.controller` (trigger Explode: Fly→Explode). `VoidSphere.cs` xử lý OnTriggerEnter2D→Explode→cleanup |
| 17 | **DONE** BloodExplosion.anim | Done | 8 frame từ Blood Explosion-Sheet.png (PPU=100, 64×64, GUID `c944141a8eb358b4f81952e41097aef9`). 12fps, non-looping. Chưa gắn prefab nào — chờ Architect dùng sau |

---

## 14. TRẠNG THÁI UNITY EDITOR

Các hạng mục dưới đây **không phải bug code** — là phần việc Editor-side, cần làm sau khi code hoàn thiện:

| Hạng mục | Trạng thái | Ghi chú |
|---|---|---|
| **Animator Controller** `VoidBoss.controller` | ✅ Complete | 7 parameters (Run Bool + 6 Triggers). AnyState transitions. VoidBoss_Die state (placeholder). Xem mục 7.6 cho event table |
| **Animation Clips** — Die | ⚠️ Placeholder | `VoidBoss_Die.anim` đã tạo (6 sprite keyframe, non-looping, 0.83s) nhưng dùng sprite Idle. **Cần asset artist tạo sprite Die riêng.** Không cần `Void_Sleep` — Boss dùng Idle xuyên suốt (SỬA 1) |
| **Animation Events** trong clip | ✅ Complete | Tất cả 5 clip (Atk1, Atk2, Skill1, Skill2, Skill3) đã có Animation Events: Spawn method tại frame 2 + OnAttackAnimEnd tại frame cuối |
| **VoidBoss.prefab** | ⚠️ Tạo rồi — chờ assign refs | `Assets/Resources/Prefab/Enemy/VoidBoss.prefab`. **Cần:** kéo voidSpherePrefab, ambushTrapPrefab, bloodMoonTelegraphPrefab vào Inspector, assign deathVFX, set collider size |
| **voidSpherePrefab** | ✅ Complete | `Assets/Resources/Prefab/Effect/VoidSphere.prefab`. CircleCollider2D(Trigger) + Rigidbody2D(Kinematic) + `VoidSphereProjectile.cs` (homing + timeout 5s + damage + stun/slow/armor debuff → destroy) |
| **VoidSphere_Fly.anim** | ✅ Complete | `Assets/Animation/Enemy/VoidBoss/VoidSphere_Fly.anim` (frame 2-4, 3 frame, 12fps, loop). Void sphere-Sheet.png |
| **VoidSphere_Explode.anim** | ✅ Complete | `Assets/Animation/Enemy/VoidBoss/VoidSphere_Explode.anim` (frame 5-13, 9 frame, 12fps, non-loop). Event `OnVoidSphereExplodeEnd` tại frame cuối |
| **BloodExplosion.anim** | ✅ Complete | `Assets/Animation/Enemy/VoidBoss/BloodExplosion.anim` (8 frame Blood Explosion-Sheet, 12fps, non-looping). Chưa gắn prefab — dùng sau |
| **ambushTrapPrefab** | ✅ Complete | `Assets/Resources/Prefab/Effect/AmbushTrap.prefab`. BoxCollider2D(Trigger) + Rigidbody2D(Kinematic) + `AmbushTrapController.cs` (damage + SilentEffect → destroy) |
| **bloodMoonTelegraphPrefab** | ✅ Complete | `Assets/Resources/Prefab/Effect/BloodMoonTelegraph.prefab`. CircleCollider2D(Trigger) + `BloodMoonTelegraphController.cs` gắn trực tiếp (không AddComponent). `damageRadius`/`damageAmount` config qua Inspector |
| ~~stompAoEPrefab~~ | ❌ **Không dùng** | Đã đổi sang `OverlapCircleAll` trực tiếp trong Controller. Prefab cũ giữ lại nhưng không reference |
| ~~spikePiercePrefab~~ | ❌ **Không dùng** | Đã đổi sang `OverlapBoxAll` trực tiếp trong Controller. Prefab cũ giữ lại nhưng không reference |
| **Layer Collision Matrix** | ✅ Complete | `Enemy`(7) × `Player`(3) = IGNORE. `EnemyAttack`(10) × `Player`(3) = enabled |
| **ObjectPool.prefab** | ✅ Tạo rồi | `Assets/Resources/Prefab/ObjectPool.prefab` với entry `BloodMoonTelegraph` (size 25) |
| **BossHealthBarUI** trong scene | ⚠️ Prefab tạo rồi | `Assets/Resources/Prefab/UI/BossHealthBarUI.prefab` — WorldSpace Canvas với Slider + Fill + Border + BossnameText. **Cần:** kéo vào scene, gọi `SetHealthBar()` |
| **VoidAggroTrigger.cs** | ✅ Complete | `Assets/Script/EnemyThing/Boss/VoidBoss/VoidAggroTrigger.cs`. Gắn trên CHILD GameObject (CircleCollider2D, Trigger). OnTriggerEnter2D(Player) → disable collider → gọi `CameraFollow.ZoomToBossReveal()` → cinematic xong → `WakeUpFromAggro()` |
| **VoidBoss_Die.anim .meta** | ❌ Chưa có | Die state tạm dùng Idle anim GUID. Cần Unity Editor để sinh GUID thật cho placeholder clip |

---
## 15. FILE MAP

```

Assets/Script/
├── Effect/CameraController.cs         ← CameraFollow + Boss Reveal Zoom
│
└── EnemyThing/
    ├── Boss/VoidBoss/
    │   ├── VoidBossController.cs          ← FSM, Super Armor, Facing Lock,
    │   │                                      Cleanup, Anti-Overlap BM
    │   ├── VoidAggroTrigger.cs            ← Aggro zone → camera zoom → wake
    │   ├── VoidSphereProjectile.cs        ← Homing projectile + timeout
    │   ├── AmbushTrapController.cs        ← Trap delay dash + SilentEffect
    │   ├── BloodMoonTelegraphController.cs← Pool-managed AoE telegraph
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

## 16. ARCHITECT DECISIONS (2026-07-18)

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

### Decision 5 — ObjectPool tracking (đã fix)

| Mục | Chi tiết |
|-----|----------|
| Vấn đề | `ObjectPool.SpawnFromPool()` dequeue rồi enqueue ngay → không phân biệt object đang dùng vs rảnh. Có thể reposition object đang active. Ảnh hưởng Loot + VoidBoss BloodMoon |
| Giải pháp | **Option B** — PoolMember component gắn trên mỗi object (tự động trong Awake). Queue chỉ chứa available object. `SpawnFromPool` = Dequeue (không re-enqueue). `ReturnToPool` = đọc PoolMember → Enqueue đúng queue. Khi queue rỗng → Instantiate fallback + `Debug.LogWarning` cảnh báo pool size thiếu |
| File áp dụng | `Pattern/PoolMember.cs` (mới), `Pattern/ObjectPool.cs` (sửa 3 method) |
| Impact callers | ✅ 0 caller cần sửa: LootManager, ItemFloat, VoidBossController giữ nguyên signature |
| Trạng thái | ✅ **Đã implement** |

### Decision 6 — Bỏ Sleep animation, dùng Idle xuyên suốt (SỬA 1, 2026-07-26)

| Mục | Chi tiết |
|-----|----------|
| Vấn đề | VoidBoss không có clip/chuyển animation riêng cho "chưa phát hiện Player" — animation Idle chạy xuyên suốt |
| Giải pháp | Xoá `animator.Play("Void_Sleep", ...)`. `isAwake = false` gate AI logic, không gate animation. Thêm `WakeUpFromAggro()` để trigger aggro. Animation Idle chạy từ Start |
| File áp dụng | `VoidBossController.cs` (Start, WakeUpFromAggro) |
| Còn thiếu | ~~Cơ chế trigger "vùng phát hiện Player" gọi `WakeUpFromAggro()` chưa có~~ ✅ Đã implement (SỬA 8) |
| Trạng thái | ✅ Code đã sửa |

### Decision 7 — Timing Animation Event Void Sphere (SỬA 2, 2026-07-26)

| Mục | Chi tiết |
|-----|----------|
| Vấn đề | Frame 0-2 là Boss vận cầu (dính vào sprite), frame 2+ cầu độc lập — event phải đúng ranh giới |
| Giải pháp | `SpawnVoidSphere()` tại time 0.1667 (frame 2). Prefab Instantiate → `sphereAnim.Play(clipName, 0, 0f)` — clip `VoidSphere_Fly` đã được trim từ frame 2 nên play từ 0f là đúng |
| File áp dụng | `VoidBoss_Skill1.anim` (event), `VoidBossController.cs` (spawn), Context docs |
| Trạng thái | ✅ Đã implement |

### Decision 8 — Aggro zone + xoá OnWakeUpComplete (SỬA 8, 2026-07-27)

| Mục | Chi tiết |
|-----|----------|
| Vấn đề | Cơ chế trigger "vùng phát hiện Player" gọi `WakeUpFromAggro()` chưa có. `OnWakeUpComplete()` còn sót lại từ SỬA 1 nhưng không được dùng |
| Giải pháp | Tạo `VoidAggroTrigger.cs` — script gắn trên CHILD GameObject (CircleCollider2D, Trigger). `OnTriggerEnter2D(Player)` → `parentController.WakeUpFromAggro()`. Xoá `OnWakeUpComplete()` khỏi `VoidBossController.cs` |
| File áp dụng | `VoidAggroTrigger.cs` (mới), `VoidBossController.cs` (xoá OnWakeUpComplete) |
| Trạng thái | ✅ Đã implement |
