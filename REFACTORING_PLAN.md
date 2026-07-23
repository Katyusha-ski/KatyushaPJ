# Enemy System Refactoring Plan – Revised

## Overview

Refactor enemy system để đạt SRP + DIP, nhưng **giữ pragmatic cho quy mô 5 enemy types**.  
Tránh over-engineering: không tách class quá nhỏ, không tạo factory riêng cho từng enemy.

### Giải thích đầy đủ

Mục tiêu của refactor này là giữ hệ thống enemy **dễ hiểu, dễ mở rộng và không bị ôm quá nhiều trách nhiệm trong một class**. Trong bản cũ, `EnemyController` thường phải lo cùng lúc: đọc input từ game, điều khiển di chuyển, đổi animation, quản lý cooldown, xử lý state, và cả logic đặc thù của từng enemy. Cách đó vẫn chạy được, nhưng càng về sau càng khó sửa vì một thay đổi nhỏ có thể kéo theo nhiều phần khác.

Kế hoạch này chia lại nhiệm vụ theo đúng vai trò:

- **EnemyController**: đóng vai trò trung tâm điều phối. Nó tạo các thành phần cần thiết, giữ state hiện tại, thực hiện chuyển state, và cung cấp dữ liệu chung cho state sử dụng.
- **MovementManager**: giữ toàn bộ logic di chuyển như patrol, quay mặt, đuổi theo, lùi lại, đổi hướng khi gặp chướng ngại, và tính khoảng cách tới player.
- **AnimationController**: chỉ lo thao tác với `Animator`, ví dụ bật/tắt bool, trigger attack/hurt/die/alert. Nhờ vậy state không cần biết chi tiết animation controller hoạt động thế nào.
- **IEnemyState**: chỉ nhận interface thay vì nhận trực tiếp `EnemyController`. Điều này giúp state không phụ thuộc vào MonoBehaviour cụ thể và giảm coupling giữa hành vi và implementation.
- **EnemyStateFactory**: tạo ra các state dùng chung cho toàn bộ enemy. Enemy con chỉ override những state hoặc hành vi thật sự khác biệt.
- **Enemy melee**: dùng chung bộ state cơ bản, chủ yếu khác nhau ở kiểu tấn công hoặc hiệu ứng khi chết.
- **Necromancer**: là enemy ranged nên cần thêm hành vi kitting và heal, vì vậy có vài state và interface phụ trợ riêng.

### Luồng hoạt động tổng quát

1. Khi enemy được khởi tạo, `EnemyController` lấy các component cần thiết như `Rigidbody2D`, `SpriteRenderer`, `Animator`, `CharacterStats` và xác định player.
2. Sau đó controller tạo `MovementManager`, `AnimationController` và `EnemyStateFactory`.
3. Controller cache các state thường dùng như Idle, Pursuit, Attack, Hurt, Die để tránh tạo lại nhiều lần.
4. Enemy vào state đầu tiên, thường là `Idle`.
5. Mỗi frame, `Update()` gọi `currentState.OnUpdate(...)` để state tự quyết định hành vi tiếp theo.
6. Nếu state cần đổi hướng hành vi, nó gọi `ctx.SwitchTo("TênState")` thay vì tự sửa trực tiếp controller.

### Ý nghĩa của cách chia này

Cách tổ chức này giúp mỗi phần có một lý do thay đổi rõ ràng:

- Nếu thay đổi cách enemy di chuyển, chỉ sửa `MovementManager`.
- Nếu thay đổi animation trigger, chỉ sửa `AnimationController`.
- Nếu thay đổi cách một state chuyển sang state khác, chỉ sửa class state tương ứng.
- Nếu thêm enemy mới, thường chỉ cần kế thừa `EnemyController` và override phần attack hoặc state đặc thù.

### Vì sao không tách nhỏ hơn nữa

Kế hoạch này cố tình không đi theo hướng tạo quá nhiều abstraction nhỏ như `AttackCooldownTracker`, `IDistanceProvider`, `IRetreatBehavior` hay factory riêng cho từng enemy. Lý do là quy mô hiện tại chỉ có khoảng 5 enemy types, nên nếu tách quá sâu sẽ làm code khó đọc hơn mà lợi ích thực tế không nhiều. Mục tiêu là **tách đúng mức cần thiết**, không over-engineering.

### Nguyên tắc chính
### Nguyên tắc chính

| Nguyên tắc | Áp dụng |
|---|---|
| **SRP** | EnemyController chỉ làm state machine. Movement → MovementManager. Animation → AnimationController. |
| **DIP** | `IEnemyState` nhận interface, không nhận `EnemyController`. States không biết gì về MonoBehaviour. |
| **OCP** | Thêm enemy mới = thêm subclass + override attack handler. Không sửa state code. |
| **Keep it simple** | Không AttackCooldownTracker riêng. Không factory riêng cho từng enemy. 2 interface lớn đủ dùng. |

---

## File structure sau refactor

```
Assets/Script/EnemyThing/
├── Core/
│   ├── EnemyController.cs          (refactored — chỉ state machine)
│   ├── IEnemyMovement.cs           (NEW — interface cho movement/vision)
│   ├── IEnemyCombat.cs             (NEW — interface cho attack/cooldown)
│   ├── IEnemyRanged.cs             (NEW — interface cho ranged behavior)
│   └── IRangedEnemy.cs             (giữ lại, remove sau khi migrate hết)
├── Controllers/
│   ├── MovementManager.cs          (NEW)
│   └── AnimationController.cs      (NEW)
├── Factory/
│   └── EnemyStateFactory.cs        (NEW — 1 factory cho tất cả enemy)
├── States/
│   ├── Base/
│   │   ├── IEnemyState.cs          (sửa — nhận interface thay vì EnemyController)
│   │   ├── BaseAttackState.cs      (refactored)
│   │   └── BasePursuitState.cs     (refactored)
│   ├── Common/
│   │   ├── IdleState.cs            (refactored)
│   │   ├── AlertState.cs           (refactored)
│   │   ├── HurtState.cs            (refactored)
│   │   └── DieState.cs             (refactored)
│   └── Ranged/
│       ├── RangedAttackState.cs    (refactored)
│       ├── KittingState.cs         (refactored)
│       └── HealState.cs            (refactored)
└── Enemies/
    ├── Melee/
    │   ├── SlimeE.cs               (updated)
    │   ├── SkullE.cs               (updated)
    │   ├── GolemE.cs               (updated)
    │   └── NightBorneE.cs          (updated)
    └── Ranged/
        └── NecromancerE.cs         (updated)
```

---

## PHASE 1: Interfaces cốt lõi (DIP foundation)

### 1.1 IEnemyMovement

```csharp
// Assets/Script/EnemyThing/Core/IEnemyMovement.cs
public interface IEnemyMovement
{
    void Patrol();
    void LookAtPlayer();
    void MoveTowardPlayer();
    void Pursue();
    void RetreatFromPlayer();
    void SetDirection(int dir);
    int GetDirection();
    float GetDistanceToPlayer();
    float GetVisionRange();
}
```

### 1.2 IEnemyCombat

```csharp
// Assets/Script/EnemyThing/Core/IEnemyCombat.cs
public interface IEnemyCombat
{
    bool IsAttackReady();
    void ExecuteAttack();
    void RecordAttack();
    float GetAttackRange();
    void PlayAnimTrigger(string trigger);
    void PlayAnimBool(string name, bool value);
}
```

### 1.3 IEnemyRanged

```csharp
// Assets/Script/EnemyThing/Core/IEnemyRanged.cs
public interface IEnemyRanged
{
    float GetCloseDistance();
    float GetPreferredDistance();
    void Kitting();
}
```

**Tại sao chỉ 3 interface?**
- `IEnemyMovement` gom movement + vision + direction — đây là nhóm chức năng luôn đi cùng nhau trong enemy AI
- `IEnemyCombat` gom attack + cooldown + animation trigger — state chỉ cần biết "có đánh được không" và "đánh", không cần biết animator
- `IEnemyRanged` chỉ cho enemy bắn — không gộp vào combat interface vì không phải enemy nào cũng cần
- **Không tách** `IDistanceProvider` / `IRetreatBehavior` riêng — quá mịn, không đem lại lợi ích thực tế cho 5 enemy

---

## PHASE 2: Sửa IEnemyState (quan trọng nhất)

### 2.1 Interface mới

```csharp
// Assets/Script/EnemyThing/States/Base/IEnemyState.cs
public interface IEnemyState
{
    void OnEnter(IEnemyMovement movement, IEnemyCombat combat);
    void OnUpdate(IEnemyMovement movement, IEnemyCombat combat);
    void OnExit(IEnemyMovement movement, IEnemyCombat combat);
}
```

**Không còn `EnemyController enemy`.** State không biết gì về MonoBehaviour.  
State chỉ gọi movement/combat qua interface — testable, decoupled.

### 2.2 IEnemyStateProvider (giữ nguyên, chỉ dùng trong EnemyController)

```csharp
public interface IEnemyStateProvider
{
    IEnemyState GetIdleState();
    IEnemyState GetPursuitState();
    IEnemyState GetAttackState();
    IEnemyState GetAlertState();
    IEnemyState GetHurtState(IEnemyState preState);
    IEnemyState GetDieState();
    IEnemyState GetKittingState();
}
```

---

## PHASE 3: Tách Managers (SRP)

### 3.1 MovementManager

```csharp
// Assets/Script/EnemyThing/Controllers/MovementManager.cs
public class MovementManager
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private CharacterStats stats;  // để lấy MovementSpeed
    private int direction = 1;
    private int lastPatrolDirection = 1;

    public MovementManager(Rigidbody2D rb, SpriteRenderer sr, CharacterStats stats)
    {
        this.rb = rb;
        this.sr = sr;
        this.stats = stats;
    }

    public void Patrol()
    {
        direction = lastPatrolDirection;
        rb.linearVelocityX = stats.MovementSpeed * direction;
        sr.flipX = direction < 0;
    }

    public void LookAtPlayer(Transform player)
    {
        if (player == null) return;
        float diffX = player.position.x - rb.position.x;
        if (Mathf.Abs(diffX) > 0.2f)
        {
            direction = diffX > 0 ? 1 : -1;
            sr.flipX = direction < 0;
        }
        rb.linearVelocity = Vector2.zero;
    }

    public void MoveTowardPlayer(Transform player, float speedMultiplier = 1.5f)
    {
        if (player == null) return;
        float diffX = player.position.x - rb.position.x;
        if (Mathf.Abs(diffX) > 0.2f)
        {
            direction = diffX > 0 ? 1 : -1;
            sr.flipX = direction < 0;
            rb.linearVelocityX = stats.MovementSpeed * speedMultiplier * direction;
        }
        else
        {
            rb.linearVelocityX = 0f;
        }
    }

    public void RetreatFromPlayer(Transform player, float speedMultiplier = 0.8f)
    {
        if (player == null) return;
        float diffX = player.position.x - rb.position.x;
        direction = diffX > 0 ? -1 : 1;
        sr.flipX = direction < 0;
        rb.linearVelocityX = stats.MovementSpeed * speedMultiplier * direction;
    }

    public float GetDistanceToPlayer(Transform player)
    {
        if (player == null) return Mathf.Infinity;
        return Vector2.Distance(rb.position, player.position);
    }

    public void SetDirection(int dir) { direction = dir; lastPatrolDirection = dir; }
    public int GetDirection() => direction;
    public void OnHitObstacle() { direction *= -1; lastPatrolDirection = direction; }
}
```

### 3.2 AnimationController

```csharp
// Assets/Script/EnemyThing/Controllers/AnimationController.cs
public class AnimationController
{
    private Animator animator;

    public AnimationController(Animator animator) => this.animator = animator;

    public void SetTrigger(string trigger) => animator.SetTrigger(trigger);
    public void SetBool(string name, bool value) => animator.SetBool(name, value);
    public void PlayRun(bool isRunning) => animator.SetBool("Run", isRunning);
    public void PlayAttack() => animator.SetTrigger("Attack");
    public void PlayHurt() => animator.SetTrigger("Hurt");
    public void PlayDie() => animator.SetTrigger("Die");
    public void PlayAlert() => animator.SetTrigger("Alert");
    public void ResetTrigger(string trigger) => animator.ResetTrigger(trigger);
}
```

### 3.3 AttackCooldown — không tách riêng

Attack cooldown chỉ là 2 field + 2 method.  
Giữ trong EnemyController, expose qua `IEnemyCombat`:

```csharp
// Trong EnemyController — implement IEnemyCombat
public bool IsAttackReady() => Time.time - lastTimeAttack >= attackCooldown;
public void RecordAttack() => lastTimeAttack = Time.time;
```

**Không tạo class AttackCooldownTracker** — lợi ích không đủ bù chi phí.

---

## PHASE 4: EnemyController refactored

```csharp
// Assets/Script/EnemyThing/Core/EnemyController.cs
public class EnemyController : MonoBehaviour, IEnemyStateProvider, IEnemyMovement, IEnemyCombat
{
    // Config
    [SerializeField] protected float attackRange = 1.5f;
    [SerializeField] protected float visionRange = 5f;
    [SerializeField] protected float attackCooldown = 2f;

    // References
    protected Transform player;
    protected CharacterStats characterStats;

    // Managers
    protected MovementManager movement;
    protected AnimationController animationCtrl;
    protected EnemyStateFactory stateFactory;

    // State machine
    protected IEnemyState currentState;
    protected Dictionary<string, IEnemyState> stateCache = new();

    // Cooldown
    protected float lastTimeAttack = -Mathf.Infinity;

    protected virtual void Start()
    {
        var rb = GetComponent<Rigidbody2D>();
        var sr = GetComponent<SpriteRenderer>();
        var animator = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();

        if (characterStats == null)
        {
            Debug.LogWarning($"{gameObject.name} missing CharacterStats!", gameObject);
            return;
        }

        if (player == null && PlayerManager.Instance != null)
            player = PlayerManager.Instance.PlayerTransform;

        movement = new MovementManager(rb, sr, characterStats);
        animationCtrl = new AnimationController(animator);
        stateFactory = new EnemyStateFactory(this);

        CacheStates();
        ChangeState(GetIdleState());
    }

    protected virtual void Update()
    {
        currentState?.OnUpdate(this, this);
    }

    // --- IEnemyMovement ---
    public void Patrol() => movement.Patrol();
    public void LookAtPlayer() => movement.LookAtPlayer(player);
    public void MoveTowardPlayer() => movement.MoveTowardPlayer(player);
    public virtual void Pursue() { LookAtPlayer(); MoveTowardPlayer(); }
    public virtual void RetreatFromPlayer() => movement.RetreatFromPlayer(player);
    public void SetDirection(int dir) => movement.SetDirection(dir);
    public int GetDirection() => movement.GetDirection();
    public float GetDistanceToPlayer() => movement.GetDistanceToPlayer(player);
    public float GetVisionRange() => visionRange;

    // --- IEnemyCombat ---
    public bool IsAttackReady() => Time.time - lastTimeAttack >= attackCooldown;
    public void RecordAttack() => lastTimeAttack = Time.time;
    public float GetAttackRange() => attackRange;
    public virtual void ExecuteAttack() => animationCtrl.PlayAttack();
    public void PlayAnimTrigger(string trigger) => animationCtrl.SetTrigger(trigger);
    public void PlayAnimBool(string name, bool val) => animationCtrl.SetBool(name, val);

    // --- State management ---
    protected virtual void CacheStates()
    {
        stateCache["Idle"]    = GetIdleState();
        stateCache["Pursuit"] = GetPursuitState();
        stateCache["Attack"]  = GetAttackState();
        stateCache["Hurt"]    = GetHurtState(null);
        stateCache["Die"]     = GetDieState();
    }

    public void ChangeState(IEnemyState newState)
    {
        currentState?.OnExit(this, this);
        currentState = newState;
        currentState?.OnEnter(this, this);
    }

    public void ChangeStateByName(string name)
    {
        if (stateCache.TryGetValue(name, out var state) && state != null)
            ChangeState(state);
    }

    public IEnemyState GetCurrentState() => currentState;

    // --- State factory methods (virtual để override) ---
    public virtual IEnemyState GetIdleState()    => stateFactory.CreateIdleState();
    public virtual IEnemyState GetPursuitState() => stateFactory.CreatePursuitState();
    public virtual IEnemyState GetAttackState()  => stateFactory.CreateAttackState();
    public virtual IEnemyState GetAlertState()   => stateFactory.CreateAlertState();
    public virtual IEnemyState GetHurtState(IEnemyState preState) => stateFactory.CreateHurtState(preState);
    public virtual IEnemyState GetDieState()     => stateFactory.CreateDieState();
    public virtual IEnemyState GetKittingState() => stateFactory.CreateKittingState();

    // --- Collision ---
    protected void OnCollisionEnter2D(Collision2D c)
    {
        if (c.gameObject.CompareTag("Obstacle")) movement.OnHitObstacle();
    }
    protected void OnTriggerEnter2D(Collider2D c)
    {
        if (c.gameObject.CompareTag("Obstacle")) movement.OnHitObstacle();
    }
}
```

### EnemyController còn lại bao nhiêu?

| Trước | Sau |
|---|---|
| ~259 dòng | ~120 dòng |
| movement + animation + cooldown + state | chỉ state machine + orchestration |
| state gọi `enemy.GetLastTimeAttack()` | state gọi `combat.IsAttackReady()` |

---

## PHASE 5: State classes refactored

### 5.1 IdleState

```csharp
public class IdleState : IEnemyState
{
    public void OnEnter(IEnemyMovement movement, IEnemyCombat combat)
    {
        combat.PlayAnimBool("Run", false);
    }

    public void OnUpdate(IEnemyMovement movement, IEnemyCombat combat)
    {
        if (movement.GetDistanceToPlayer() <= movement.GetVisionRange())
            // Cannot access ChangeStateByName from here — return signal
            return; // EnemyController sẽ check transition riêng
        movement.Patrol();
    }

    public void OnExit(IEnemyMovement movement, IEnemyCombat combat) { }
}
```

**Vấn đề:** State không còn `EnemyController` thì không gọi `ChangeStateByName()` được.  
**Giải pháp:** Dùng **StateReturn** pattern — state return enum signal:

```csharp
public enum StateSignal
{
    Continue,
    SwitchToIdle,
    SwitchToPursuit,
    SwitchToAttack,
    SwitchToHurt,
    SwitchToDie,
    SwitchToKitting,
}
```

```csharp
public interface IEnemyState
{
    StateSignal OnEnter(IEnemyMovement movement, IEnemyCombat combat);
    StateSignal OnUpdate(IEnemyMovement movement, IEnemyCombat combat);
    StateSignal OnExit(IEnemyMovement movement, IEnemyCombat combat);
}
```

Hoặc đơn giản hơn — **giữ context object nhẹ**:

```csharp
public interface IEnemyStateContext
{
    void SwitchState(IEnemyState state);
    void SwitchTo(string name);
}

public interface IEnemyState
{
    void OnEnter(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx);
    void OnUpdate(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx);
    void OnExit(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx);
}
```

`EnemyController` implement `IEnemyStateContext`.  
State gọi `ctx.SwitchTo("Pursuit")` mà không biết ai implement nó.

→ **Khuyến nghị dùng `IEnemyStateContext`** — đơn giản, dễ hiểu, không cần enum.

### 5.2 IdleState (với context)

```csharp
public class IdleState : IEnemyState
{
    public void OnEnter(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        combat.PlayAnimBool("Run", false);
    }

    public void OnUpdate(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        float dist = movement.GetDistanceToPlayer();
        if (dist <= movement.GetVisionRange())
            ctx.SwitchTo("Pursuit");
        else
            movement.Patrol();
    }

    public void OnExit(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx) { }
}
```

### 5.3 BaseAttackState

```csharp
public class BaseAttackState : IEnemyState
{
    public virtual void OnEnter(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        combat.PlayAnimBool("Run", false);
        movement.LookAtPlayer();
    }

    public virtual void OnUpdate(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        if (movement.GetDistanceToPlayer() > combat.GetAttackRange())
        {
            ctx.SwitchTo("Pursuit");
            return;
        }
        if (combat.IsAttackReady())
        {
            combat.ExecuteAttack();
            combat.RecordAttack();
        }
    }

    public virtual void OnExit(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx) { }
}
```

### 5.4 RangedAttackState

```csharp
public class RangedAttackState : BaseAttackState
{
    public override void OnUpdate(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        float dist = movement.GetDistanceToPlayer();
        if (dist > combat.GetAttackRange())
        {
            ctx.SwitchTo("Pursuit");
            return;
        }
        if (movement is IEnemyRanged ranged && dist < ranged.GetCloseDistance())
        {
            ctx.SwitchTo("Kitting");
            return;
        }
        base.OnUpdate(movement, combat, ctx);
    }
}
```

### 5.5 KittingState

```csharp
public class KittingState : IEnemyState
{
    public void OnEnter(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        combat.PlayAnimBool("Run", false);
    }

    public void OnUpdate(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx)
    {
        float dist = movement.GetDistanceToPlayer();
        if (movement is IEnemyRanged ranged)
        {
            if (dist > combat.GetAttackRange())
            {
                ctx.SwitchTo("Pursuit");
                return;
            }
            if (dist >= ranged.GetCloseDistance() && dist <= ranged.GetPreferredDistance())
            {
                ctx.SwitchTo("Attack");
                return;
            }
            ranged.Kitting();  // retreat + shoot
        }
    }

    public void OnExit(IEnemyMovement movement, IEnemyCombat combat, IEnemyStateContext ctx) { }
}
```

### 5.6 HealState, HurtState, DieState, AlertState, BasePursuitState

Chuyển đổi tương tự: nhận `(IEnemyMovement, IEnemyCombat, IEnemyStateContext)`, dùng `ctx.SwitchTo()`.

Chi tiết xem file code cũ + thay `enemy.ChangeStateByName(x)` → `ctx.SwitchTo(x)`,  
`enemy.GetDistanceToPlayer()` → `movement.GetDistanceToPlayer()`,  
`enemy.SetAnimatorTrigger(x)` → `combat.PlayAnimTrigger(x)`.

---

## PHASE 6: EnemyStateFactory

1 factory duy nhất, dùng cho tất cả enemy.  
Subclass có thể override method để trả về state khác.

```csharp
// Assets/Script/EnemyThing/Factory/EnemyStateFactory.cs
public class EnemyStateFactory
{
    protected IEnemyMovement movement;
    protected IEnemyCombat combat;
    protected IEnemyStateContext ctx;

    public EnemyStateFactory(EnemyController enemy)
    {
        movement = enemy;
        combat = enemy;
        ctx = enemy;
    }

    public virtual IEnemyState CreateIdleState()    => new IdleState();
    public virtual IEnemyState CreatePursuitState() => new BasePursuitState();
    public virtual IEnemyState CreateAttackState()  => new BaseAttackState();
    public virtual IEnemyState CreateAlertState()   => new AlertState();
    public virtual IEnemyState CreateHurtState(IEnemyState preState) => new HurtState(preState);
    public virtual IEnemyState CreateDieState()     => new DieState();
    public virtual IEnemyState CreateKittingState() => new KittingState();
}
```

**Không tạo NecromancerStateFactory riêng.**  
NecromancerE chỉ cần override `GetAttackState()` và `GetKittingState()` để trả về `RangedAttackState` / `KittingState`.

---

## PHASE 7: Update enemy subclasses

### 7.1 NecromancerE

```csharp
public class NecromancerE : EnemyController, IEnemyRanged
{
    [SerializeField] private SkillManager skillManager;
    [SerializeField] private int skill1Index = 0;
    [SerializeField] private int skill2Index = 1;
    [SerializeField] private int healSkillIndex = 2;

    [SerializeField] private float closeDistance = 3f;
    [SerializeField] private float preferredDistance = 5f;

    private HealState healState;

    public float GetCloseDistance() => closeDistance;
    public float GetPreferredDistance() => preferredDistance;

    public void Kitting()
    {
        // Custom: shoot while retreating
        if (IsAttackReady())
        {
            ExecuteAttack();
            RecordAttack();
        }
        RetreatFromPlayer();
    }

    protected override void CacheStates()
    {
        base.CacheStates();
        stateCache["Kitting"] = GetKittingState();
        stateCache["Attack"]  = GetAttackState();
        healState = new HealState();
        stateCache["Heal"] = healState;
    }

    protected override void Update()
    {
        if (ShouldHeal() && !(currentState is HealState))
        {
            ctx.SwitchTo("Heal");
            return;
        }
        base.Update();
    }

    public override void ExecuteAttack()
    {
        if (CanUseSkill(skill1Index)) { PlayAnimTrigger("Skill1"); return; }
        if (CanUseSkill(skill2Index)) { PlayAnimTrigger("Skill2"); return; }
    }

    public override IEnemyState GetAttackState()  => new RangedAttackState();
    public override IEnemyState GetKittingState() => new KittingState();

    // --- Skill methods gọi từ animation event ---
    public void CastSkill1() => skillManager?.ActivateSkill(skill1Index, GetDirection());
    public void CastSkill2() => skillManager?.ActivateSkill(skill2Index, GetDirection());
    public void CastHeal()   => skillManager?.ActivateSkill(healSkillIndex, 0);

    private bool ShouldHeal()
    {
        var health = GetComponent<Health>();
        return health != null && health.CurrentHealth < health.MaxHealth * 0.5f
               && CanUseSkill(healSkillIndex);
    }

    private bool CanUseSkill(int index)
    {
        return skillManager != null && index >= 0 && index < skillManager.skills.Count
               && skillManager.skills[index] != null && skillManager.skills[index].CanActivate;
    }
}
```

### 7.2 GolemE

```csharp
public class GolemE : EnemyController
{
    [SerializeField] private SkillManager skillManager;
    private const int GolemMagicIndex = 0;
    private const int StoneSpikeIndex = 1;
    private bool checkAttack1 = false;

    public override void Pursue()
    {
        if (CanUseSkill(GolemMagicIndex))
        {
            PlayAnimBool("Run", false);
            PlayAnimTrigger("Attack 2");
            return;
        }
        base.Pursue();
    }

    public override void ExecuteAttack()
    {
        if (CanUseSkill(StoneSpikeIndex))
        {
            PlayAnimTrigger("Attack 3");
            return;
        }
        if (!checkAttack1) { PlayAnimTrigger("Attack"); checkAttack1 = true; }
        else               { PlayAnimTrigger("Attack 1"); checkAttack1 = false; }
    }

    // Animation events
    public void CastGolemMagic() { skillManager?.ActivateSkill(GolemMagicIndex, GetDirection()); }
    public void CastStoneSpike() { skillManager?.ActivateSkill(StoneSpikeIndex, 0); }

    private bool CanUseSkill(int idx) { /* giống NecromancerE */ }
}
```

### 7.3 SlimeE, SkullE, NightBorneE

Chỉ cần override `ExecuteAttack()` / `HandleEnemyDeath()` như hiện tại,  
nhưng thay `animator.SetTrigger(x)` → `PlayAnimTrigger(x)`.

---

## Implementation order

| Bước | File | Mất bao lâu |
|---|---|---|
| 1. Tạo `IEnemyMovement`, `IEnemyCombat`, `IEnemyRanged`, `IEnemyStateContext` | 4 file mới | 30 phút |
| 2. Sửa `IEnemyState` interface + các state class | 9 file | 2 giờ |
| 3. Tạo `MovementManager`, `AnimationController` | 2 file mới | 1 giờ |
| 4. Tạo `EnemyStateFactory` | 1 file mới | 30 phút |
| 5. Refactor `EnemyController` | 1 file | 2 giờ |
| 6. Update enemy subclasses (Slime, Skull, Golem, NightBorn, Necromancer) | 5 file | 2 giờ |
| 7. Chạy thử + sửa lỗi | — | 2 giờ |
| **Tổng** | | **~10 giờ** |

---

## Các lưu ý quan trọng

1. **`IEnemyStateContext`** thay `ChangeStateByName` — state không cần biết EnemyController
2. **Không tách AttackCooldownTracker** — giữ 2 field trong EnemyController
3. **Factory duy nhất** — không state factory riêng cho từng enemy, subclass chỉ override state creation method
4. **Giữ `OnCollisionEnter2D`/`OnTriggerEnter2D` trong EnemyController** — vì là MonoBehaviour lifecycle
5. **Thêm `[RequireComponent(typeof(CharacterStats))]`** vào EnemyController (đồng bộ với Health)

---

## Boss system (mở rộng sau này)

Khi thêm boss, tạo:

```
Assets/Script/EnemyThing/Enemies/Boss/
├── BossController.cs       (kế thừa EnemyController, thêm phase system)
├── BossPhase.cs            (data class: HP threshold → state list)
└── BossArena.cs            (domain barrier, teleport player)
```

`BossController` override `Update()` để check HP threshold + switch phase.

---

## Backward compatibility

- Tất cả thay đổi đều **additive** — enemy cũ vẫn chạy song song
- Giữ `IRangedEnemy.cs` cũ, xoá sau khi NecromancerE migrate xong
- Có thể migrate từng enemy một, không cần làm hết cùng lúc

---

## Common pitfalls

1. **Đừng** để state gọi `GetComponent<>()` — đó là việc của EnemyController
2. **Đừng** pass `EnemyController` vào state — dùng interface
3. **Đừng** tách class quá nhỏ (AttackCooldownTracker, IDistanceProvider, ...)
4. **Nên** dùng `IEnemyStateContext.SwitchTo()` thay vì return enum
5. **Nên** giữ config (visionRange, attackRange...) là `[SerializeField]` trong EnemyController
