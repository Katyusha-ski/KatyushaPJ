# KatyushaPJ

> 2D side-scrolling Action RPG built with Unity URP.
> Người chơi điều khiển nhân vật chính cùng companion "Hachiware" (stand/spirit) chiến đấu qua các chapter.

---

## Scenes

| Scene | Role |
|---|---|
| `MainMenuScene` | Main menu |
| `GrassScene` | Chapter 1 main level |
| `StoneScene` | Chapter 2 main level |
| `SnowScene` | Chapter 3 main level |
| `ShaderTest` | Dev/testing |

## Game Loop (per chapter)

```
MainMenu → New Game → Village (hub)
  → VillageExitTrigger → MainLevel (GrassScene/StoneScene/SnowScene)
    → LevelEndTrigger → BossLevel (if boss exists) or next Village
      → BossEndTrigger → next chapter's Village
```

---

## Architecture Overview

All core managers are **persistent singletons** (`DontDestroyOnLoad`). Systems communicate via:
1. **Singleton access** (`ClassName.Instance`)
2. **C# events** (`System.Action`)
3. **Direct method calls**
4. **Interface-based decoupling** (enemy state machine)

### Dependency Map

```
GameManager
 ├── PlayerManager ──┬── PlayerController ──┬── PlayerMovementController
 │                   │                      ├── PlayerSkillInput
 │                   │                      ├── PlayerAnimationController
 │                   │                      └── Stand (Hachiware companion)
 │                   ├── CharacterStats (13 stats, modifier system)
 │                   ├── StatusEffectController (12+ effect types)
 │                   └── Health (damage formula, shield, regen)
 ├── Inventory ──┬── ItemData SO (5 types: Consumable/Equipment/Material/Quest/Skill)
 │               ├── EquipmentManager (applies ItemStats → CharacterStats)
 │               ├── ConsumableManager (bridge: EffectData → StatusEffect)
 │               └── ShopManager (chapter-based unlock, stock tracking)
 ├── ChapterManager ──┬── ChapterDataSO (mainSceneName, bossSceneName)
 │                    └── Triggers (LevelEndTrigger, BossEndTrigger, VillageExitTrigger)
 ├── SaveManager (static, JSON via JsonUtility)
 ├── AudioManager (AudioMixer, music/SFX volume)
 ├── UIManager (delegates to GameSceneController)
 └── ObjectPool (loot items, projectiles)
```

---

## System Breakdown

### 1. Manager Layer — `Assets/Script/Manager/`

| Class | File | Responsibility |
|---|---|---|
| **GameManager** | `GameManager.cs` | GameState machine (MainMenu/Gameplay/Pause), save/load orchestration, play time |
| **PlayerManager** | `PlayerManager.cs` | Player reference hub (controller, health, transform, rigidbody) |
| **UIManager** | `UIManager.cs` | Thin facade for scene/UI actions |
| **AudioManager** | `AudioManager.cs` | AudioMixer wrapper, PlaySFX() |

### 2. Player System — `Assets/Script/PlayerThing/`

| Class | File | Responsibility |
|---|---|---|
| **PlayerController** | `PlayerController.cs` | Top-level coordinator: movement input, skill input, health |
| **PlayerMovementController** | `PlayerMovementController.cs` | Walk/run/jump physics, ground detection, direction flip |
| **PlayerSkillInput** | `PlayerSkillInput.cs` | Input buffer system (E/Q/R/F), buffered activation when off cooldown |
| **PlayerAnimationController** | `PlayerAnimationController.cs` | Animator parameter driver (cached hash IDs) |
| **Stand** | `Stand.cs` | Hachiware companion; box-overlap punch attack with crit/lifesteal |
| **PlayerNA** | `PlayerNA.cs` | Normal attack (KeypadEnter) → `Stand.Punch()` |
| **InputConfig** | `InputConfig.cs` | ScriptableObject key binding definition |

### 3. Stats System — `Assets/Script/PlayerThing/Stats/`

| Class | File | Responsibility |
|---|---|---|
| **CharacterStats** | `CharacterStats.cs` | 13 stats with additive+multiplicative modifier lists. Events: `StatsChanged`, `MaxHPChanged`, `MovementSpeedChanged` |
| **StatsModifier** | `StatsModifier.cs` | Immutable modifier: `Value`, `Type` (Additive/Multiplicative), `Source` |
| **StatModifierConfig** | `StatsModifier.cs` | Serializable config used in ItemStats/EffectData |

**Stats:** Armor, LifeSteal, CCRes (cap 60%), Atk, CritRate, CritDamage, ArmorPierce, CDR (cap 40%), MaxHP, MovementSpeed, HPRegen, DmgR, SkillAmp

**Formula:** `stat = (base + sum(additives)) * product(1 + each multiplicative)`

### 4. Status Effect System — `Assets/Script/PlayerThing/Status/`

**Base:** `StatusEffect` (abstract) — lifecycle: `OnApply()` → `OnTick()` → `OnRemove()`

**Controller:** `StatusEffectController` — manages active effects, checks `IronBody` before applying CC

Concrete effects (all in same directory):

| Effect | Type | Behavior |
|---|---|---|
| `StunEffect` | CC | Stop movement + clear skill buffer |
| `RootEffect` | CC | Stop movement only |
| `SilentEffect` | CC | Clear skill buffer only |
| `DoTEffect` | Debuff | Damage over time with tick interval |
| `StatModifierEffect` | Buff/Debuff | Apply `StatModifierConfig` list to `CharacterStats` |
| `HealEffect` | Buff | Instant heal + tick-based HoT |
| `UndyingEffect` | Buff | HP cannot drop to 0 |
| `UntargetableEffect` | Buff | Immune to all damage |
| `VirtualShieldEffect` | Buff | Absorb damage shield |
| `IronBodyEffect` | Buff | CC immunity |
| `CleanseEffect` | Buff | Remove all CC effects |

**Entry point from items:** `ConsumableManager` reads `EffectData` → factory switch → creates `StatusEffect` → `StatusEffectController.ApplyEffect()`

### 5. Skill System — `Assets/Script/Skill/`

**SkillBase** (ScriptableObject): `skillName`, `icon`, `SkillType`, `cooldown`. CDR-modified cooldown tick. Abstract methods: `Initialize(CharacterStats)`, `Activate(GameObject user, int direction)`.

**Hierarchy:**
```
SkillBase (SO, abstract)
 ├── DirectDmgSkillBase — damage without spawned prefab
 ├── SpawnDamageSkillBase — spawns prefab (projectiles/traps), has CalculateFinalDamage()
 └── (concrete skills)
      ├── MeleeSkill — OverlapCircle attack, multi-hit, applies effects
      ├── ProjectileSkill — spawns IProjectilePref with config
      ├── DashSkill — coroutine: layer pass-through, Untargetable, damage+stun
      ├── DefendSkill — coroutine: shield, reflect, root/slow
      └── SpawnPrefabSkill — generic prefab spawner
```

**Concrete skills** are `[CreateAssetMenu]` ScriptableObjects in `ActionsSO/`.

**Skill matrix:** `Inventory.skillMatrix[4, 5]` — 4 skill types × 5 levels. Items must be acquired in order (Lv1 → Lv2 → ...). `PlayerSkillManager.ReloadSkills()` loads from inventory.

**Enemy skills:** `StoneSpike` (ground hazard), `NercoHole` (persistent AoE), `DeathExplosion` (on NightBorn death), `GolemMagic` (projectile).

### 6. Enemy System — `Assets/Script/EnemyThing/`

**Architecture: State Machine + Interface Injection**

**Core interfaces:** `IEnemyMovement`, `IEnemyCombat`, `IEnemyRanged`, `IEnemyStateContext`, `IEnemyState`, `IEnemyStateProvider`

**Controllers** (pure classes, not MonoBehaviours):
- `MovementManager` — patrol, pursue, retreat, obstacle collision
- `AnimationController` — wraps Animator with named methods

**Orchestrator:** `EnemyController` — state machine hub. `Update()` → `currentState.OnUpdate()`. References `PlayerManager.Instance.PlayerTransform`.

**States:**

| State | Transitions |
|---|---|
| **IdleState** | Patrol → Pursuit if player in vision range |
| **AlertState** | Alert anim → Pursuit or Idle |
| **BasePursuitState** | Move to player → Attack if in range, Idle if out of vision |
| **BaseAttackState** | Attack on cooldown → Pursuit if player moves away |
| **RangedAttackState** | Extends BaseAttackState, adds Kitting transition |
| **KittingState** | Retreat while attacking → Attack at preferred distance |
| **HealState** | Heal when HP < 50% |
| **HurtState** | 0.5s hurt anim → Pursuit |
| **DieState** | Death anim → destroy |

**Enemy types** (`Enemies/Melee/` and `Enemies/Ranged/`):

| Class | Type | Special |
|---|---|---|
| `SlimeE` | Melee | Alternates attack/hit animations |
| `SkullE` | Melee | Default |
| `NightBorneE` | Melee | Explosion burst + HazardZone on death |
| `GolemE` | Melee/Ranged hybrid | Has `SkillManager` (GolemMagic, StoneSpike) |
| `NecromancerE` | Ranged | Implements `IEnemyRanged`, has kitting + heal states, 3 skills |

**Bosses** (`Boss/`):

| Class | Chapter | FSM | Special Mechanics |
|-------|---------|-----|-------------------|
| **BatBoss** | 4 | 5 states: `BatHoverState` (unique) + 4 generic states | Deflect system (Melee/Stand bounce), Pillar burst (25% MaxHP), Atk1/Ak2 **50/50 pure random** |
| **VoidBoss** | 6 | 9 states: 3 custom (`VoidIdleState`, `VoidPursuitState`, `BloodMoonState`) + 6 generic states | Super Armor (no flinch), Facing Lock, BloodMoon Ultimate (5 waves × 5 telegraphs), **Ultimate ưu tiên ngắt Pursuit** |

### 7. Item System — `Assets/Script/ItemSystem/`

**Core data** (all ScriptableObjects):
- `ItemData` — central item definition (`itemId`, `itemName`, `ItemType`, `EquipmentType`, `ItemStats`, `List<EffectData>`, `SkillData`)
- `ItemStats` — `List<StatModifierConfig>`
- `EffectData` — effect config (type, duration, value, tick, statModifiers)
- `SkillData` — `SkillBase skill` + `int Level`
- `ItemStack` — runtime stack: `ItemData` + `amount`

**Inventory** (`Inventory/Inventory.cs`): Persistent singleton. 3 arrays: `itemSlots[30]`, `equipment[4]`, `skillMatrix[4,5]`. Events: `OnInventoryChanged`, `OnEquipmentChanged`, `OnSkillMatrixChanged`.

**Key data flows:**

```
Consumable use:
  UI → Inventory.UseItem() → ConsumableManager.Use()
    → creates StatusEffect from EffectData → StatusEffectController.ApplyEffect()
    → Inventory.RemoveItem()

Equipment equip:
  UI → Inventory.SwapEquipItem() → fires OnEquipmentChanged
    → EquipmentManager → CharacterStats.AddStatModifier() / RemoveStatModifier()

Skill item use:
  UI → Inventory.UseItem() → PlayerSkillManager.UseItem()
    → checks level gating → updates skillMatrix → fires OnSkillMatrixChanged
```

**Shop** (`Shop/`):
- `ShopManager` — runtime stock tracking, chapter-based unlock (`UnlockByChapter()`), purchase validation
- `ShopEntrySO` — ScriptableObject: item, cost(s), stock (-1 = infinite), unlockChapter
- `ShopUI` — category filter + item list + detail panel

**Loot** (`Itemfloat/`):
- `LootTable` (ScriptableObject) — list of `LootEntry` (item, dropChance, min/max amount). `GetRandomLoot()` rolls each entry independently.
- `LootManager` — on enemies; spawns `ItemFloat` via `ObjectPool` on death
- `ItemFloat` — world pickup; on trigger → `Inventory.AddItem()` → return to pool

### 8. Health System — `Assets/Script/Health/`

| Class | File | Responsibility |
|---|---|---|
| **Health** | `Health.cs` | Damageable entity. Damage formula: `max(1, (incoming - armor) * (1 - dmgR))`. Shield absorbs first. HP regen every 5s. Events: `OnDamaged` |
| **IHealthBar** | `IHealthBar.cs` | Interface for health bar display |
| **PlayerHealthBar** | `PlayerHealthBar.cs` | Shader-based (`_Health`) + text |
| **EnemyHealthBar** | `EnemyHealthBar.cs` | Slider-based |

### 9. Save System — `Assets/Script/SaveSystem/`

| Class | File | Responsibility |
|---|---|---|
| **SaveManager** (static) | `SaveManager.cs` | JSON save/load to `persistentDataPath/savefile.json` via `JsonUtility` |
| **SaveData** | `SaveData.cs` | Serializable container: chapter, inventory, equipment, skillMatrix, shop, scene, player pos/health, metadata |
| **SerializableItemStack** | `SerializableItemStack.cs` | Saves item by `itemName` string → `Resources.Load<ItemData>()` on load (searches subfolders: Items/, Items/Consumables/, Items/Equipments/, etc.) |
| **ChapterManager** | `ChapterSystem/ChapterManager.cs` | Persistent singleton. Chapter list + progression. Auto-saves on village load. |
| **ChapterDataSO** | `ChapterSystem/ChapterDataSO.cs` | ScriptableObject: `chapterID`, `chapterName`, `mainSceneName`, `bossSceneName` |
| **SavePoint** | `SavePoint.cs` | World trigger → `GameManager.SaveGame()` |
| **LevelEndTrigger** | `LevelEndTrigger.cs` | → `ChapterManager.CompleteChapter()` |
| **VillageExitTrigger** | `VillageExitTrigger.cs` | → `ChapterManager.GoToMainScene()` |
| **BossEndTrigger** | `BossEndTrigger.cs` | → `ChapterManager.CompleteBossChapter()` |

### 10. UI System — `Assets/Script/UI/`

| Class | File | Description |
|---|---|---|
| **MainMenuUI** | `MainMenuUI.cs` | Play, Continue, New Game, Save, About, Quit |
| **MenuUI** (base) | `MenuUI.cs` | Pause/menu panel base: `ShowMenuAndPause()`, `HideMenuAndResume()` |
| **OptionUI** | `OptionUI.cs` | Music/SFX volume sliders → AudioManager |
| **GameOverUI** | `GameOverUI.cs` | Singleton. Shows on player death. |
| **VictoryUI** | `VictoryUI.cs` | Singleton. Victory panel. |
| **CharacterStatsUI** | `CharacterStatsUI.cs` | Displays all 13 stats from CharacterStats |
| **SkillUI** | `SkillUI.cs` | Individual skill slot (icon + cooldown overlay) |
| **SkillPanelUI** | `SkillPanelUI.cs` | Container for 4 skill slots, updates each frame |
| **InventoryUI** | `Inventory/InventoryUI.cs` | Full inventory grid (30 slots + 4 equipment). Right-click detail popup. |
| **Slot** | `Inventory/Slot.cs` | Single inventory slot UI (icon, quantity, index) |
| **SlotDragHandler** | `Inventory/SlotDragHandler.cs` | Drag-equip/unequip/swap, right-click details |
| **SkillSystemUI** | `Inventory/SkillSystemUI.cs` | 4×5 skill matrix UI |

**Shop UI:**
- `CategoryUI` — filter buttons, fires `OnCategorySelected(ItemType)`
- `ItemListUI` — scrollable item list
- `ItemDetailUI` — selected item details + buy button
- `ShopSlotUI` — individual slot (icon, affordability color)

### 11. Utility — `Assets/Script/`

| Location | Class | Description |
|---|---|---|
| `Effect/` | `CameraFollow` | Smooth lerp follow camera |
| `Effect/` | `ButtonSFX` | Singleton button click SFX |
| `Effect/` | `AutoDestroy` | Timed particle/effect cleanup |
| `Pattern/` | `ObjectPool` | Generic singleton pool (`Dictionary<tag, Queue>`) |
| `Scene/` | `GameSceneController` | Scene navigation singleton |
| `Shader/` | `HPBar.shader` | UI shader: clips by `_Health`, color gradient + glint |
| `Shader/` | `PurpleSmoke.shader` | Multi-layer noise morphing, vertex sway, color cycling |

---

## Key Formulas

```
CDR: actualCooldown = baseCooldown / (1 - CDR/100)    [CDR cap: 40%]
Damage: finalDamage = max(1, (incoming - armor) * (1 - dmgR))
Crit: totalDamage = damage * (1 + critDamage/100)      [if crit roll succeeds]
Stat: value = (base + sum(additives)) * product(1 + each multiplicative)
HP Regen: every 5 seconds, heal = HPRegen
```

---

## Known Issues / TODOs

- Save/load uses `itemName` string lookup via `Resources.Load` — fragile; migrate to `itemId`
- Armor pierce stat exists in `CharacterStats` but not applied in damage calculation
- `AssisterController` is legacy/unused
- Defend and Melee skill SO assets not yet created
- `SkillMatrix` save serialization structure needs review

---

## Related Documentation

| File | Content |
|---|---|
| `REFACTORING_PLAN.md` | Enemy system refactor: SRP + DIP, state machine architecture |
| `Docs/SKILL_SYSTEM_PLAN.md` | Skill system design, CDR fix, 5 levels per skill |
| `Assets/Script/ItemSystem/Core/REFACTOR_ItemStats_EquipmentManager.md` | Unified stat modifier API migration |
| `Assets/Script/SaveSystem/ChapterSystem/SaveSystemChanges.md` | Old level-based → chapter-based migration |
| `Assets/Script/HuongDan/ItemInfo.md` | Full item catalog (Vietnamese) |
| `Assets/Script/HuongDan/ROADMAP_UNITY_FIREBASE_INTERN.md` | Firebase integration roadmap |
| `Docs/Contexts/Katyusha_BatBoss_Context.md` | BatBoss architecture: FSM, Health, Pillar system, deflect mechanics |
| `Docs/Contexts/Katyusha_VoidBoss_Context.md` | VoidBoss architecture: FSM, AI decision, Super Armor, BloodMoon, cleanup |
| `Docs/Contexts/KatyushaPJ_Boss_System_Summary.md` | Boss system summary: BatBoss, VoidBoss, Duo Golem design |
