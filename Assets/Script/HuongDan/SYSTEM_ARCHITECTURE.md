# Enemy Behavior System Architecture
## Hierarchical State Machine with Factory Pattern

---

## Overview

This document describes the complete architecture of the enemy behavior system using:
- **State Pattern**: for behavior management
- **Factory Pattern**: for state creation
- **Template Method Pattern**: for code reuse

---

## Layer Structure

### Layer 0: Foundation (Interfaces)

```
IEnemyState (Interface)
- OnEnter(enemy)
- OnUpdate(enemy)
- OnExit(enemy)

IEnemyStateProvider (Factory Interface)
- GetPatrolState()
- GetChaseState()
- GetAttackState()
```

### Layer 1: Base States (Reusable)

```
BaseChaseState (Abstract)
- OnEnter(): SetBool("Run", true)
- OnUpdate(): Range checking + ExecuteChase()
- OnExit(): SetBool("Run", false)
- ExecuteChase(): abstract

BaseAttackState (Abstract)
- OnEnter(): SetBool("Run", false), LookAtPlayer()
- OnUpdate(): Range checking + ExecuteAttackPattern()
- OnExit()
- ExecuteAttackPattern(): abstract
```

### Layer 2: Concrete States

**Chase States:**
- ChaseState (generic): MoveTowardPlayer + LookAtPlayer
- GolemChaseState (special): Check skill[0], cast if ready
- WakeUpState (custom): For dream-like enemies

**Attack States:**
- AttackState (generic): Simple ExecuteAttack()
- SlimeAttackState (special): Alternate Hit + Attack
- GolemAttackState (special): Priority skill[1] + multi-attack

**Custom States:**
- PatrolState (global): Used by all enemies
- SleepState (custom): For special enemies like DreamEnemy

### Layer 3: State Machine Controller

```
EnemyController : MonoBehaviour, IEnemyStateProvider
- currentState: IEnemyState
- ChangeState(newState)
- Update() calls currentState.OnUpdate()

Factory Methods (Virtual):
- GetPatrolState(): return PatrolState
- GetChaseState(): return ChaseState
- GetAttackState(): return AttackState
```

### Layer 4: Enemy Classes

```
SkullE : EnemyController
- Uses all default states

SlimeE : EnemyController
- Override GetAttackState() -> SlimeAttackState

GolemE : EnemyController
- Override GetChaseState() -> GolemChaseState
- Override GetAttackState() -> GolemAttackState

DreamEnemyE : EnemyController
- Override GetPatrolState() -> SleepState
- Override GetChaseState() -> WakeUpState
```

---

## State Flow Diagrams

### Skull (Simple)
```
Patrol -> (if player visible) Chase -> (if in attack range) Attack
  ^                                       |
  |_______(if out of range)_______________|
```

### Slime (Custom Attack)
```
Patrol -> Chase -> Attack (Hit + Attack alternate)
  ^                 |
  |_______if far____|
```

### Golem (Complex)
```
Patrol -> Chase (with skill) -> Attack (skill + multi-attack)
  ^                                |
  |____________if far______________|
```

### DreamEnemy (Custom States)
```
Sleep -> (if player visible) WakeUp -> Attack
  ^                            |
  |_______(if out of range)____|
```

---

## File Structure

```
Assets/Script/EnemyThing/
├── IEnemyState.cs
├── IEnemyStateProvider.cs
├── EnemyController.cs
│
├── State/
│   ├── BaseChaseState.cs
│   ├── BaseAttackState.cs
│   ├── PatrolState.cs
│   │
│   ├── ChaseState.cs
│   ├── GolemChaseState.cs
│   ├── WakeUpState.cs
│   │
│   ├── AttackState.cs
│   ├── SlimeAttackState.cs
│   ├── GolemAttackState.cs
│   │
│   ├── SleepState.cs
│   └── (more custom states as needed)
│
├── SkullE.cs
├── SlimeE.cs
├── GolemE.cs
└── DreamEnemyE.cs
```

---

## Key Design Principles

### 1. Factory Pattern
Instead of:
```csharp
if (enemy is SlimeE) return new SlimeAttackState()
else return new AttackState()
```

Do this:
```csharp
return enemy.GetAttackState()
```

The enemy decides which state it needs!

### 2. Template Method Pattern
BaseAttackState defines the skeleton:
- OnUpdate() handles range checking (SHARED)
- ExecuteAttackPattern() is abstract (IMPLEMENTED BY SUBCLASS)

### 3. Polymorphism
Each enemy can override factory methods:
```csharp
public override IEnemyState GetAttackState()
{
    return new SlimeAttackState();  // Custom behavior
}
```

### 4. Open/Closed Principle
- OPEN for extension: Add new enemy = create class + override method
- CLOSED for modification: Don't edit existing ChaseState

---

## Behavior Comparison

| Enemy | Patrol | Chase | Attack | Status |
|-------|--------|-------|--------|--------|
| Skull | Default | Default | Default | Simple |
| Slime | Default | Default | Custom (Hit+Attack) | Medium |
| Golem | Default | Custom (skill) | Custom (skill+multi) | Complex |
| DreamEnemy | Custom (Sleep) | Custom (WakeUp) | Default | Unique |

---

## Implementation Checklist

- [ ] Create IEnemyStateProvider.cs
- [ ] Create BaseChaseState.cs
- [ ] Create BaseAttackState.cs (if not exists)
- [ ] Add GetPatrolState(), GetChaseState(), GetAttackState() to EnemyController
- [ ] Refactor ChaseState to extend BaseChaseState
- [ ] Create GolemChaseState
- [ ] Create WakeUpState (for custom enemies)
- [ ] Refactor AttackState to extend BaseAttackState
- [ ] Create GolemAttackState
- [ ] Create SleepState (for custom enemies)
- [ ] Update SkullE (no changes needed)
- [ ] Update SlimeE (override GetAttackState)
- [ ] Update GolemE (override GetChaseState + GetAttackState)
- [ ] Create DreamEnemyE (override GetPatrolState + GetChaseState)
- [ ] Test all behaviors

---

## Why This Architecture Works

This system is extremely flexible because:

1. **Zero Type Checking** - No if/else for enemy types
2. **Scalable** - Add new enemy = 1 class + 1-3 method overrides
3. **Reusable** - Base classes contain shared logic
4. **Maintainable** - Each state is a separate file
5. **Testable** - Mock factory methods easily
6. **Extensible** - Custom states without modifying existing code

Enemy can skip ANY state type (Patrol, Chase, or Attack) by overriding its factory method.

---

## Summary

This is a production-grade architecture that scales from simple enemies (Skull) to complex ones (Golem) and even unique behaviors (DreamEnemy) without modifying existing code.
