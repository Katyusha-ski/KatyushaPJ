# Skill System - Kế hoạch phát triển

## Lịch sử thay đổi

### 2026-06-21: Fix CDR + Range + Dash

#### Đã sửa
- **CDR formula** (`Core/SkillBase.cs`): `1 / (1 - CDR/100)` thay vì `1 + CDR/100`
  - CDR=40 → giảm đúng 40% thời gian hồi
- **Null skill** (`SkillPanelUI.cs`): bỏ `EmptySkill`, xử lý null trực tiếp
  - Slot chưa mở skill → icon trống, cooldown = 0
- **Health**: thêm `isInvulnerable` flag, `SetInvulnerable()`, `TakeDamage()` check

#### Đã thêm mới

**Range Skill (5 cấp)**
- `Core/ISpawnPref.cs` — thêm `ProjectileConfig` struct + `SetProjectileConfig()` vào interface
- `Actions/ProjectileSkill.cs` — field mới: `projectileSpeed`, `pierceCount`, `applyPoison`, `applySlow`, `applyArmorDebuff` + configs
- `Prefabs/ProjectilePref.cs` — dùng `ProjectileConfig` từ SO, pierce tracking, effect qua `DoTEffect` + `StatModifierEffect`, targetTag

**Dash Skill (5 cấp)**
- `Actions/DashTransformSkill.cs` — field mới: `applyInvulnerable`, `applyDashDamage`, `applyStun`, `stunDuration`, `pushOutDistance`
  - Lv1: dash cơ bản
  - Lv2: +`applyInvulnerable` (UntargetableEffect)
  - Lv3: +`applyDashDamage`
  - Lv4: +`applyStun` (DashStun component)
  - Lv5: tăng speed/duration/pushOutDistance
- `Status/UntargetableEffect.cs` — StatusEffect mới: Buff, bật/tắt `isInvulnerable` trên Health
- `DashStun` (trong Actions/DashTransformSkill.cs) — disable EnemyController tạm thời

#### Còn lại
- Defend skill (5 cấp)
- Melee skill (5 cấp)
- Tạo SO assets + ItemData cho từng cấp (4 nhánh × 5 cấp)

---

## Cấu trúc file hiện tại

```
Assets/Script/Skill/
├── Core/                              # Abstract base classes, interfaces, managers
│   ├── SkillBase.cs                   # abstract SO: cooldown (CDR fixed), icon, skillType
│   ├── DirectDmgSkillBase.cs          # baseDamage + CalculateFinalDamage()
│   ├── SpawnDamageSkillBase.cs        # baseDamage + spawnPrefab + CalculateFinalDamage()
│   ├── SkillManager.cs                # generic manager (dùng cho enemy)
│   ├── PlayerSkillManager.cs          # player manager (load từ Inventory skillMatrix)
│   └── ISpawnPref.cs                  # ISpawnPref, ProjectileConfig, IProjectilePref
├── Actions/                           # Concrete skill ScriptableObjects
│   ├── ProjectileSkill.cs             # Range - 5 cấp (speed, pierce, poison, slow, armor debuff)
│   ├── DashTransformSkill.cs          # Dash - 5 cấp (invulnerable, dmg, stun)
│   ├── MeleeSkill.cs                  # Melee - 5 cấp (Hachi)
│   └── SpawnPrefabSkill.cs            # Spawn skill
├── Prefabs/                           # Prefab behavior scripts
│   └── ProjectilePref.cs              # Generic projectile (targetTag, pierce, effects)
└── Enemy/                             # Enemy-specific skill objects
    ├── StoneSpike.cs
    └── NercoHole.cs

Assets/Script/PlayerThing/Status/
├── Base/
│   ├── StatusEffect.cs
│   └── StatusEffectController.cs
├── UntargetableEffect.cs         # [MỚI] Buff - miễn damage khi dash
├── DoTEffect.cs                  # Debuff - sát thương theo thời gian
├── StatModifierEffect.cs         # Debuff/Buff - modifier stat
├── StunEffect.cs                 # CC - cho Player
└── ...
```

---

## TODO

- [ ] Defend skill (5 cấp)
  - [ ] Tạo `DefendSkill.cs` SO
  - [ ] Field: giáp ảo, movement khi def, reflect dmg, unstoppable,...
- [ ] Melee skill (5 cấp)
  - [ ] Tạo `MeleeSkill.cs` SO
  - [ ] Field: punch dmg, shockwave range, combo hits, club summon, sasumata + DoT + armor debuff
- [ ] Tạo SO assets cho từng cấp
  - [ ] Range Lv1-Lv5 (ProjectileSkill)
  - [ ] Dash Lv1-Lv5 (DashTransformSkill)
  - [ ] Defend Lv1-Lv5 (DefendSkill)
  - [ ] Melee Lv1-Lv5 (MeleeSkill)
- [ ] Tạo ItemData assets cho từng skill item
  - [ ] Range Lv1-Lv5 (MagicSphereItem)
  - [ ] Dash Lv1-Lv5 (DashTransformItem)
  - [ ] Defend Lv1-Lv5 (DefendItem)
  - [ ] Melee Lv1-Lv5 (HachiItem)
- [ ] Fix `defaultLayerName` not used in old DashTransformSkill ✅
- [ ] Validate `LayerMask.NameToLayer` returns -1 ✅
- [ ] Dash stun for enemies (DashStun component) ✅

---

## Cấp độ skill

### Range Skill
| Lv | Speed | Damage | Đặc biệt |
|----|-------|--------|----------|
| 1  | 8     | Thấp   | -        |
| 2  | 12    | TB     | -        |
| 3  | 12    | TB     | Poison + Slow |
| 4  | 14    | Cao    | + Armor Debuff |
| 5  | 16    | Rất cao| + Pierce |

### Dash Skill
| Lv | Speed | Duration | Đặc biệt |
|----|-------|----------|----------|
| 1  | 15    | 0.3s     | Dash cơ bản |
| 2  | 15    | 0.3s     | + Untargetable |
| 3  | 17    | 0.3s     | + Sát thương |
| 4  | 17    | 0.35s    | + Stun enemy |
| 5  | 20    | 0.4s     | + push xa hơn, dmg cao |

### Defend Skill
| Lv | Đặc biệt |
|----|----------|
| 1  | Giáp ảo nhỏ, đứng yên |
| 2  | Giáp ảo + di chuyển chậm |
| 3  | Phản damage nếu timing đúng |
| 4  | Unstoppable (không bị stun) |
| 5  | Di chuyển tự do |

### Melee Skill (Hachi)
| Lv | Kỹ năng | Đặc biệt |
|----|---------|----------|
| 1  | Đấm mạnh | Sát thương đơn mục tiêu |
| 2  | Vỗ tay xung kích | AoE rộng, dmg hơn Lv1 |
| 3  | Liên hoàn đấm | Đấm liên tiếp nhiều phát |
| 4  | Gậy club | Tạo club đập mạnh |
| 5  | Sasumata | Triệu hồi Sasumata, đâm mục tiêu gây DoT + giảm giáp |

