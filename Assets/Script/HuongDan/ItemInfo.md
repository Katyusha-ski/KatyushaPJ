# Thông tin item trong dự án

Tài liệu này tổng hợp các loại item và chỉ số dựa trên các cấu trúc hiện có trong dự án.

## Loại item (ItemType)
- **Consumable**: Vật phẩm tiêu thụ.
- **Equipment**: Trang bị có thể gắn, cung cấp chỉ số.
- **Material**: Nguyên liệu chế tạo/nâng cấp.
- **Quest**: Vật phẩm nhiệm vụ.
- **Skill**: Vật phẩm liên quan kỹ năng.

## Loại trang bị (EquipmentType)
- **None**: Không phải trang bị.
- **Chest**: Giáp/Phòng thủ.
- **Weapon**: Vũ khí/Sát thương.
- **Ring**: Nhẫn/Hiệu ứng đặc biệt hoặc tăng HP.
- **Feather**: Lông vũ/Tốc độ di chuyển.

## Thông tin cơ bản của item
- **itemName**: Tên hiển thị.
- **itemIcon**: Sprite icon.
- **itemType**: Loại item.
- **equipmentType**: Loại trang bị (nếu là Equipment).
- **description**: Mô tả.
- **isStackable**: Có thể cộng dồn hay không.
- **maxStackSize**: Số lượng tối đa trên một stack.

## Chỉ số trang bị (ItemStats)
### Offensive
- **damage**: Sát thương cơ bản (baseAtk).
- **critChance**: Tỉ lệ chí mạng (%).
- **critDamage**: Sát thương chí mạng (%).
- **armorPierce**: Xuyên giáp (%).

### Defensive
- **armor**: Giáp.
- **health**: Máu tối đa.

### Utility
- **movementSpeed**: Tốc độ di chuyển.
- **controlResistance**: Kháng khống chế (%).

### Special
- **lifesteal**: Hút máu (%).
- **cooldownReduction**: Giảm hồi chiêu (%).
- **hpRegen**: Hồi máu theo thời gian (mỗi 5s).
- **dmgR**: Giảm sát thương (%).
- **skillAmp**: Khuếch đại kỹ năng (%).

## Ghi chú
- Item loại **Equipment** có thể chứa **ItemStats** để cộng vào chỉ số nhân vật.
- Có thể cộng dồn nhiều trang bị bằng cách cộng từng chỉ số trong ItemStats.

## Danh sách item gợi ý
### Equipment
- **Leather Chest**
  - itemType: Equipment | equipmentType: Chest
  - stats: armor +3, hpRegen +1
- **Spectral Dagger**
  - itemType: Equipment | equipmentType: Weapon
  - stats: damage +2
- **Frost Spear**
  - itemType: Equipment | equipmentType: Weapon
  - stats: damage +3, skillAmp +10
- **Windfeather**
  - itemType: Equipment | equipmentType: Feather
  - stats: movementSpeed +0.4, cooldownReduction +10, skillAmp +5
- **Skystride Feather**
  - itemType: Equipment | equipmentType: Feather
  - stats: movementSpeed +0.7, cooldownReduction +10, skillAmp +5, controlResistance +10
- **Silent Veil Cloak**
  - itemType: Equipment | equipmentType: Chest
  - stats: armor +4, hpRegen +4, controlResistance +12
- **Ruby Signet**
  - itemType: Equipment | equipmentType: Ring
  - stats: lifesteal +5, hpRegen +3, damage +4
- **Shadow Thunder Sword**
  - itemType: Equipment | equipmentType: Weapon
  - stats: damage +12, critChance +8, armorPierce +8
- **Blacksteel Armor**
  - itemType: Equipment | equipmentType: Chest
  - stats: armor +16, health +35, dmgR +6
- **Gale Plume**
  - itemType: Equipment | equipmentType: Feather
  - stats: movementSpeed +1.0, controlResistance +8, armor +6
- **Ring of Echoes**
  - itemType: Equipment | equipmentType: Ring
  - stats: skillAmp +12, cooldownReduction +8, critChance +4
- **Emberfang Blade**
  - itemType: Equipment | equipmentType: Weapon
  - stats: damage +14, critChance +6, lifesteal +5
- **Vanguard Plate**
  - itemType: Equipment | equipmentType: Chest
  - stats: armor +18, dmgR +8, health +25
- **Stoneguard Cuirass**
  - itemType: Equipment | equipmentType: Chest
  - stats: armor +20, health +40, hpRegen +3
- **Blood Rite Ring**
  - itemType: Equipment | equipmentType: Ring
  - stats: lifesteal +8, critDamage +15, health +15
- **Zephyr Quill**
  - itemType: Equipment | equipmentType: Feather
  - stats: movementSpeed +1.2, controlResistance +7, lifesteal +3
- **Void Pulse Band**
  - itemType: Equipment | equipmentType: Ring
  - stats: skillAmp +15, critDamage +10, damage +5
- **Stormbreaker Axe**
  - itemType: Equipment | equipmentType: Weapon
  - stats: damage +18, critDamage +18, armorPierce +8

### Consumable
- **Small Health Potion**
  - itemType: Consumable
  - mô tả: Hồi 25 HP ngay lập tức.
- **Large Health Potion**
  - itemType: Consumable
  - mô tả: Hồi 80 HP ngay lập tức.
- **Swift Elixir**
  - itemType: Consumable
  - mô tả: Tăng movementSpeed +0.5 trong 20s.
- **Cooldown Talisman**
  - itemType: Consumable
  - mô tả: Tăng cooldownReduction +10 trong 15s.
- **Ironhide Tonic**
  - itemType: Consumable
  - mô tả: Tăng armor +8 trong 20s.
- **Razorleaf Brew**
  - itemType: Consumable
  - mô tả: Tăng critChance +8 trong 15s.
- **Lifebloom Draught**
  - itemType: Consumable
  - mô tả: Hồi 5 HP mỗi 5s trong 30s.
- **Wardstone Oil**
  - itemType: Consumable
  - mô tả: Tăng dmgR +5 trong 12s.

### Material
- **Blacksteel Ore**
  - itemType: Material
  - mô tả: Nguyên liệu chế tạo giáp/weapon cấp thấp.
- **Thunder Crystal**
  - itemType: Material
  - mô tả: Dùng nâng cấp weapon hệ sét.
- **Spirit Feather Thread**
  - itemType: Material
  - mô tả: Nguyên liệu chế tạo feather tăng tốc.
- **Ashen Leather**
  - itemType: Material
  - mô tả: Da bền dùng chế tạo chest armor.
- **Frost Shard**
  - itemType: Material
  - mô tả: Mảnh băng dùng cường hóa vũ khí.
- **Sunstone Core**
  - itemType: Material
  - mô tả: Lõi năng lượng dùng khảm nhẫn.

### Quest
- **Hunter's Pursuit Charm**
  - itemType: Quest
  - mô tả: Vật phẩm nhiệm vụ từ NPC thợ săn.
- **Ancient Map Fragment**
  - itemType: Quest
  - mô tả: Thu thập 4 mảnh để mở khu vực bí ẩn.
- **Sealed Relic Case**
  - itemType: Quest
  - mô tả: Hộp chứa cổ vật cần giao cho học giả.
- **Signal Flare Kit**
  - itemType: Quest
  - mô tả: Dụng cụ gọi đồng đội trong nhiệm vụ.

### Skill
- **Skill Tome: Blade Tempest**
  - itemType: Skill
  - mô tả: Mở khóa kỹ năng gây sát thương diện rộng.
- **Arcane Seal: Ice Aegis**
  - itemType: Skill
  - mô tả: Mở khóa kỹ năng tạo khiên giảm sát thương.
- **Skill Tome: Thunder Lance**
  - itemType: Skill
  - mô tả: Mở khóa kỹ năng phóng sét tầm xa.
- **Arcane Seal: Shadow Step**
  - itemType: Skill
  - mô tả: Mở khóa kỹ năng dịch chuyển ngắn.
