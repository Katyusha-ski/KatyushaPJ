# Enemy System Refactoring Plan - Detailed Implementation Guide

## Overview
This document provides a comprehensive 5-phase refactoring plan for the enemy system, focusing on SOLID principles:
- **SRP** (Single Responsibility Principle)
- **OCP** (Open/Closed Principle)
- **ISP** (Interface Segregation Principle)
- **DIP** (Dependency Inversion Principle)

---

## PHASE 1: Foundation - Create New Infrastructure

### Objective
Build reusable, focused infrastructure components that will support the refactoring.

### 1.1 Create State Factory Pattern

**File**: `Assets/Script/EnemyThing/Factory/EnemyStateFactory.cs`

**Purpose**: Centralize state creation logic, making states testable and decoupled from EnemyController.

**Key Methods**:
```
- CreateIdleState(EnemyController enemy) ? IEnemyState
- CreateAlertState(EnemyController enemy) ? IEnemyState
- CreatePursuitState(EnemyController enemy) ? IEnemyState
- CreateAttackState(EnemyController enemy) ? IEnemyState
- CreateKittingState(EnemyController enemy) ? IEnemyState
- CreateHurtState(EnemyController enemy, IEnemyState prevState) ? IEnemyState
- CreateDieState(EnemyController enemy) ? IEnemyState
```

**Benefits**:
- Easy to extend for subclasses (override factory methods)
- Consistent state creation across all enemies
- Easy to test individual states

---

### 1.2 Create Movement Manager

**File**: `Assets/Script/EnemyThing/Controllers/MovementManager.cs`

**Purpose**: Encapsulate all movement-related logic.

**Responsibilities**:
- Track current direction
- Handle patrol movement
- Handle pursuit movement
- Handle retreat/kitting movement
- Manage sprite flipping

**Key Methods**:
```
- Patrol(float speed, int direction)
- PursuePlayer(Transform playerTransform, float speed)
- RetreatFromPlayer(Transform playerTransform, float speed)
- LookAtPlayer(Transform playerTransform)
- SetDirection(int newDirection)
- GetDirection() ? int
- UpdateSpriteFlip(SpriteRenderer sr)
```

**Dependencies**:
- Rigidbody2D
- SpriteRenderer
- Transform (player)

---

### 1.3 Create Animation Controller

**File**: `Assets/Script/EnemyThing/Controllers/AnimationController.cs`

**Purpose**: Centralize animation triggering and management.

**Responsibilities**:
- Trigger animation states
- Pass animation parameters to Animator
- Handle animation-related logic without game logic

**Key Methods**:
```
- TriggerAttack()
- TriggerMovement(float speed)
- TriggerIdle()
- TriggerAlert()
- TriggerHurt()
- TriggerDie()
- SetAnimatorParameter(string paramName, object value)
- SetAnimatorTrigger(string triggerName)
```

**Dependencies**:
- Animator

---

### 1.4 Create Attack Cooldown Tracker

**File**: `Assets/Script/EnemyThing/Controllers/AttackCooldownTracker.cs`

**Purpose**: Manage attack cooldown state and queries.

**Responsibilities**:
- Track last attack time
- Check if attack is available
- Record successful attacks

**Key Methods**:
```
- CanAttack() ? bool
- RecordAttack()
- GetTimeSinceLastAttack() ? float
- GetCooldownRemaining() ? float
- Reset()
```

**Dependencies**:
- Time (via Time.time)

---

### 1.5 Create Base Attack Handler

**File**: `Assets/Script/EnemyThing/Attack/IAttackHandler.cs`

**Purpose**: Interface for different attack types (melee, ranged, magic, etc.)

**Key Methods**:
```
- CanExecuteAttack() ? bool
- ExecuteAttack()
- GetAttackRange() ? float
- GetAttackDamage() ? int
```

---

## PHASE 2: Interface Segregation (ISP)

### Objective
Break down monolithic interfaces into focused, single-responsibility interfaces.

### Current Issues:
- `EnemyController` implements `IEnemyStateProvider` (fat interface)
- `IRangedEnemy` only used for specific enemy types
- States receive full `EnemyController`, but only need specific data

### 2.1 Split IEnemyStateProvider

**File**: `Assets/Script/EnemyThing/Core/Interfaces/IBaseEnemyController.cs`

**Purpose**: Core movement and vision capabilities

**Methods**:
```csharp
public interface IBaseEnemyController
{
    // Movement
    void Patrol();
    void LookAtPlayer();
    void MoveTowardPlayer();
    void Pursue();
    
    // Vision/Detection
    float GetDistanceToPlayer();
    float GetVisionRange();
    Transform GetPlayerTransform();
    
    // Direction
    int GetDirection();
    void SetDirection(int direction);
}
```

---

### 2.2 Create IRangedEnemyProvider

**File**: `Assets/Script/EnemyThing/Core/Interfaces/IRangedEnemyProvider.cs`

**Purpose**: Ranged-specific behaviors (kitting, retreat)

**Methods**:
```csharp
public interface IRangedEnemyProvider
{
    float GetCloseDistance();
    float GetPreferredDistance();
    void ExecuteKitting();
    void RetreatFromPlayer();
}
```

---

### 2.3 Split IRangedEnemy

**File**: `Assets/Script/EnemyThing/Core/Interfaces/IDistanceProvider.cs`

**Purpose**: Query distance preferences

**Methods**:
```csharp
public interface IDistanceProvider
{
    float GetCloseDistance();
    float GetPreferredDistance();
}
```

---

**File**: `Assets/Script/EnemyThing/Core/Interfaces/IRetreatBehavior.cs`

**Purpose**: Execute retreat logic

**Methods**:
```csharp
public interface IRetreatBehavior
{
    void RetreatFromPlayer();
    void ExecuteKitting();
}
```

---

### 2.4 Create IAttackHandler Interface (if not in Phase 1)

**File**: `Assets/Script/EnemyThing/Core/Interfaces/IAttackHandler.cs`

**Methods**:
```csharp
public interface IAttackHandler
{
    bool CanAttack();
    void ExecuteAttack();
    float GetAttackRange();
    int GetAttackDamage();
}
```

---

### 2.5 Update IEnemyStateProvider

**Location**: `Assets/Script/EnemyThing/States/Base/IEnemyState.cs`

**New Version**:
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

**Classes implementing IEnemyStateProvider**:
- `EnemyController` (base class)
- Subclasses can override specific state factories

---

## PHASE 3: Update State Classes (DIP + OCP)

### Objective
Refactor states to depend on interfaces, not concrete classes. Support extension without modification.

### 3.1 Update BaseAttackState

**File**: `Assets/Script/EnemyThing/States/Base/BaseAttackState.cs`

**Changes**:
- Depend on `IAttackHandler` instead of `EnemyController`
- Depend on `IBaseEnemyController` for movement
- Support subclass override points

**Pseudo-code**:
```csharp
public abstract class BaseAttackState : IEnemyState
{
    protected IAttackHandler attackHandler;
    protected IBaseEnemyController movementController;
    protected AttackCooldownTracker cooldown;
    
    public BaseAttackState(IAttackHandler handler, IBaseEnemyController controller)
    {
        attackHandler = handler;
        movementController = controller;
    }
    
    public virtual void OnEnter(EnemyController enemy) { }
    public virtual void OnUpdate(EnemyController enemy)
    {
        if (cooldown.CanAttack())
        {
            movementController.LookAtPlayer();
            attackHandler.ExecuteAttack();
        }
    }
    public virtual void OnExit(EnemyController enemy) { }
}
```

---

### 3.2 Update RangedAttackState

**File**: `Assets/Script/EnemyThing/States/Ranged/RangedAttackState.cs`

**Changes**:
- Depend on `IRangedEnemyProvider` for ranged behavior
- Depend on `IAttackHandler` for attack execution
- Support skill-based attacks

**Pseudo-code**:
```csharp
public class RangedAttackState : BaseAttackState
{
    private IRangedEnemyProvider rangedProvider;
    
    public RangedAttackState(IAttackHandler handler, IBaseEnemyController controller, 
                            IRangedEnemyProvider ranged)
        : base(handler, controller)
    {
        rangedProvider = ranged;
    }
    
    public override void OnUpdate(EnemyController enemy)
    {
        if (cooldown.CanAttack() && IsInAttackRange())
        {
            movementController.LookAtPlayer();
            attackHandler.ExecuteAttack();
            cooldown.RecordAttack();
        }
    }
}
```

---

### 3.3 Update KittingState

**File**: `Assets/Script/EnemyThing/States/Common/KittingState.cs`

**Changes**:
- Depend on `IRangedEnemyProvider`
- Depend on `IBaseEnemyController`
- Encapsulate distance checking logic

**Pseudo-code**:
```csharp
public class KittingState : IEnemyState
{
    private IRangedEnemyProvider rangedProvider;
    private IBaseEnemyController movementController;
    
    public KittingState(IRangedEnemyProvider provider, IBaseEnemyController controller)
    {
        rangedProvider = provider;
        movementController = controller;
    }
    
    public void OnUpdate(EnemyController enemy)
    {
        float distance = movementController.GetDistanceToPlayer();
        float preferred = rangedProvider.GetPreferredDistance();
        
        if (distance < preferred)
        {
            movementController.Pursue();
        }
        // else continue kitting
    }
}
```

---

### 3.4 Update BasePursuitState

**File**: `Assets/Script/EnemyThing/States/Base/BasePursuitState.cs`

**Changes**:
- Depend on `IBaseEnemyController`
- Support custom pursuit logic via override points

**Key Methods**:
```csharp
public virtual bool IsInAttackRange(float distance, float attackRange)
public virtual void UpdatePursuitLogic(EnemyController enemy)
```

---

### 3.5 Update IdleState

**File**: `Assets/Script/EnemyThing/States/Common/IdleState.cs`

**Changes**:
- Depend on `IBaseEnemyController` for patrol
- Make patrol direction configurable

---

## PHASE 4: EnemyController Refactoring (SRP)

### Objective
Reduce `EnemyController` responsibilities to core state machine only.

### Current Issues:
- EnemyController handles: movement, animation, cooldown, state management
- Violates SRP
- Hard to test individual components

### 4.1 Refactor EnemyController

**File**: `Assets/Script/EnemyThing/Core/EnemyController.cs`

**New Responsibilities** (Core State Machine only):
- Manage current state
- Transition between states
- Provide references to managers
- Coordinate with scene (player, physics)

**Remove/Delegate**:
- Movement logic ? MovementManager
- Animation logic ? AnimationController
- Cooldown tracking ? AttackCooldownTracker
- State creation ? EnemyStateFactory

**Updated Structure**:
```csharp
public class EnemyController : MonoBehaviour, IEnemyStateProvider, IBaseEnemyController
{
    // Core managers
    private MovementManager movementManager;
    private AnimationController animationController;
    private AttackCooldownTracker cooldownTracker;
    private EnemyStateFactory stateFactory;
    
    // State machine
    private IEnemyState currentState;
    private Dictionary<string, IEnemyState> stateCache = new();
    
    // External references
    protected Transform player;
    protected Rigidbody2D rb;
    
    // Configuration
    [SerializeField] protected float visionRange = 5f;
    [SerializeField] protected float attackRange = 1.5f;
    [SerializeField] protected int attackDamage = 1;
    [SerializeField] protected float attackCooldown = 2f;
    [SerializeField] protected float speed = 2f;
    
    void Start()
    {
        InitializeManagers();
        InitializeStates();
        ChangeState(GetIdleState());
    }
    
    void Update()
    {
        if (currentState != null)
            currentState.OnUpdate(this);
    }
    
    private void InitializeManagers()
    {
        rb = GetComponent<Rigidbody2D>();
        var sr = GetComponent<SpriteRenderer>();
        var animator = GetComponent<Animator>();
        
        movementManager = new MovementManager(rb, sr);
        animationController = new AnimationController(animator);
        cooldownTracker = new AttackCooldownTracker(attackCooldown);
        stateFactory = CreateStateFactory(); // Override in subclasses
        
        if (player == null && PlayerManager.Instance != null)
            player = PlayerManager.Instance.PlayerTransform;
    }
    
    protected virtual EnemyStateFactory CreateStateFactory()
    {
        return new EnemyStateFactory(this);
    }
    
    public void ChangeState(IEnemyState newState)
    {
        if (currentState != null)
            currentState.OnExit(this);
        currentState = newState;
        currentState?.OnEnter(this);
    }
    
    // Implement IBaseEnemyController
    public void Patrol() => movementManager.Patrol(speed, GetPatrolDirection());
    public void LookAtPlayer() => movementManager.LookAtPlayer(player);
    public void MoveTowardPlayer() => movementManager.MoveTowardPlayer(player, speed);
    public void Pursue() { LookAtPlayer(); MoveTowardPlayer(); }
    public float GetDistanceToPlayer() => movementManager.GetDistanceToPlayer(player);
    public float GetVisionRange() => visionRange;
    public Transform GetPlayerTransform() => player;
    public int GetDirection() => movementManager.GetDirection();
    public void SetDirection(int direction) => movementManager.SetDirection(direction);
    
    // Implement IEnemyStateProvider (uses factory)
    public virtual IEnemyState GetIdleState() => stateFactory.CreateIdleState(this);
    public virtual IEnemyState GetAlertState() => stateFactory.CreateAlertState(this);
    // ... etc
    
    // Expose managers for states
    public AnimationController GetAnimationController() => animationController;
    public AttackCooldownTracker GetCooldownTracker() => cooldownTracker;
    public MovementManager GetMovementManager() => movementManager;
    
    // Expose configuration
    public float GetAttackRange() => attackRange;
    public int GetAttackDamage() => attackDamage;
}
```

---

### 4.2 Update Start() and References

**Coordinate initialization**:
```csharp
void Start()
{
    InitializeManagers();      // Sets up MovementManager, AnimationController, etc.
    InitializeStates();        // Creates state cache
    ChangeState(GetIdleState()); // Start in Idle
}

protected virtual void InitializeStates()
{
    // Can be overridden by subclasses to add custom states
    // Base implementation only creates core states if needed
}
```

---

## PHASE 5: Subclass Updates

### Objective
Update enemy subclasses to use new architecture with minimal changes.

### 5.1 Update NecromancerE

**File**: `Assets/Script/EnemyThing/Enemies/Ranged/NecromancerE.cs`

**Changes**:
- Implement `IRangedEnemyProvider`
- Implement `IAttackHandler` for skill-based attacks
- Create custom `NecromancerStateFactory`
- Remove direct state creation in ExecuteAttack

**New Structure**:
```csharp
public class NecromancerE : EnemyController, IRangedEnemyProvider, IAttackHandler
{
    [Header("Ranged Config")]
    [SerializeField] private float closeDistance = 3f;
    [SerializeField] private float preferredDistance = 5f;
    
    [Header("Skill List")]
    public SkillManager skillManager;
    
    // Implement IRangedEnemyProvider
    public float GetCloseDistance() => closeDistance;
    public float GetPreferredDistance() => preferredDistance;
    
    public void ExecuteKitting()
    {
        // Custom kitting logic if needed
        Pursue();
    }
    
    public void RetreatFromPlayer()
    {
        GetMovementManager().RetreatFromPlayer(GetPlayerTransform(), speed);
    }
    
    // Implement IAttackHandler
    public bool CanAttack() => GetCooldownTracker().CanAttack();
    
    public void ExecuteAttack()
    {
        if (skillManager.skills[0].CanActivate)
        {
            GetAnimationController().SetAnimatorTrigger("Skill1");
            return;
        }
        if (skillManager.skills[1].CanActivate)
        {
            GetAnimationController().SetAnimatorTrigger("Skill2");
            return;
        }
    }
    
    public float GetAttackRange() => base.GetAttackRange(); // or override
    public int GetAttackDamage() => base.GetAttackDamage();
    
    public void CastSkill1() => skillManager.ActivateSkill(0, GetDirection());
    public void CastSkill2() => skillManager.ActivateSkill(1, GetDirection());
    public void Heal() => skillManager.ActivateSkill(2, 0);
    
    // Create custom state factory for Necromancer
    protected override EnemyStateFactory CreateStateFactory()
    {
        return new NecromancerStateFactory(this);
    }
    
    protected override void InitializeStates()
    {
        base.InitializeStates();
        // Can add Necromancer-specific states here if needed
    }
}
```

**New File**: `Assets/Script/EnemyThing/Factory/NecromancerStateFactory.cs`

```csharp
public class NecromancerStateFactory : EnemyStateFactory
{
    private NecromancerE necromancer;
    
    public NecromancerStateFactory(NecromancerE enemy) : base(enemy)
    {
        necromancer = enemy;
    }
    
    public override IEnemyState CreateAttackState(EnemyController enemy)
    {
        return new RangedAttackState(
            (IAttackHandler)necromancer,
            (IBaseEnemyController)enemy,
            necromancer
        );
    }
    
    public override IEnemyState CreateKittingState(EnemyController enemy)
    {
        return new KittingState(necromancer, (IBaseEnemyController)enemy);
    }
}
```

---

### 5.2 Update Other Enemies

**SlimeE (Melee)**:
- Implement `IAttackHandler` for melee attack
- Use base `EnemyStateFactory`
- Override `ExecuteAttack()` for melee-specific logic

**SkullE (Melee)**:
- Similar to SlimeE
- Can override `Pursue()` if custom pursuit needed

**GolemE (Melee)**:
- Similar to SlimeE
- Can add custom manager if needed

**Pattern**:
```csharp
public class MeleeEnemy : EnemyController, IAttackHandler
{
    public bool CanAttack() => GetCooldownTracker().CanAttack();
    
    public void ExecuteAttack()
    {
        GetAnimationController().TriggerAttack();
        GetCooldownTracker().RecordAttack();
    }
    
    public float GetAttackRange() => base.GetAttackRange();
    public int GetAttackDamage() => base.GetAttackDamage();
}
```

---

## Implementation Order Recommendation

### Week 1-2: Foundation Phase
1. Create AttackCooldownTracker (smallest component)
2. Create AnimationController
3. Create MovementManager
4. Create EnemyStateFactory

### Week 2-3: Interface Phase
5. Create interface files (IBaseEnemyController, IRangedEnemyProvider, etc.)
6. Have EnemyController implement them (backward compatible)

### Week 3: State Updates
7. Update BaseAttackState
8. Update RangedAttackState
9. Update KittingState, BasePursuitState, IdleState

### Week 4: Core Refactoring
10. Refactor EnemyController (use new managers and factory)
11. Update NecromancerE and other enemies
12. Testing and bug fixes

---

## Testing Strategy

### Unit Tests (Pseudo)
- Test each manager independently
- Test state transitions
- Test attack cooldown logic

### Integration Tests
- Test state machine flow
- Test enemy AI behavior
- Test skill execution

### Manual Tests
- Play scene and verify behavior
- Check animation transitions
- Verify attack timing
- Test edge cases (player far away, no skill ready, etc.)

---

## Benefits of This Refactoring

| Before | After |
|--------|-------|
| EnemyController: 200+ lines | EnemyController: ~100 lines (core only) |
| Mixed concerns | Single Responsibility |
| Hard to test components | Testable, isolated components |
| Tightly coupled states | Loosely coupled via interfaces |
| Difficult to extend | Easy to extend (new state types, new behaviors) |
| Code duplication (melee/ranged) | Shared infrastructure via managers |

---

## Backward Compatibility Notes

- All changes are **additive** initially
- Can migrate states gradually
- Keep old methods if needed during transition
- Remove deprecated methods after all enemies updated

---

## Common Pitfalls to Avoid

1. ? Don't pass full `EnemyController` to states - use specific interfaces
2. ? Don't create new managers in state classes - reference via EnemyController
3. ? Don't have states directly access components (rb, animator, sr)
4. ? Don't put game logic in managers - keep them focused
5. ? Do use dependency injection in constructors
6. ? Do make managers stateless/reusable
7. ? Do override factory methods for custom behavior

---

## File Structure After Refactoring

```
Assets/Script/EnemyThing/
??? Core/
?   ??? EnemyController.cs (refactored)
?   ??? Interfaces/
?   ?   ??? IBaseEnemyController.cs (new)
?   ?   ??? IRangedEnemyProvider.cs (new)
?   ?   ??? IDistanceProvider.cs (new)
?   ?   ??? IRetreatBehavior.cs (new)
?   ?   ??? IAttackHandler.cs (new)
?   ?   ??? IRangedEnemy.cs (legacy, can remove later)
?   ??? (keep existing files)
??? Controllers/ (new directory)
?   ??? MovementManager.cs
?   ??? AnimationController.cs
?   ??? AttackCooldownTracker.cs
??? Factory/ (new directory)
?   ??? EnemyStateFactory.cs
?   ??? NecromancerStateFactory.cs
??? States/
?   ??? Base/
?   ?   ??? BaseAttackState.cs (refactored)
?   ?   ??? BasePursuitState.cs (refactored)
?   ?   ??? IEnemyState.cs
?   ??? Common/
?   ?   ??? IdleState.cs (refactored)
?   ?   ??? KittingState.cs (refactored)
?   ?   ??? (others)
?   ??? Ranged/
?       ??? RangedAttackState.cs (refactored)
??? Enemies/
    ??? Melee/
    ?   ??? SlimeE.cs (updated)
    ?   ??? SkullE.cs (updated)
    ?   ??? GolemE.cs (updated)
    ??? Ranged/
        ??? NecromancerE.cs (updated)
```

---

## Next Steps

1. **Review** this plan with team
2. **Create** branch: `feature/enemy-refactor`
3. **Implement** Phase 1 components
4. **Test** each component thoroughly
5. **Proceed** to Phase 2-5 following the order

---

**Document Version**: 1.0  
**Last Updated**: 2024  
**Status**: Ready for Implementation
