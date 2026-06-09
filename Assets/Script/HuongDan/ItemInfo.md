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
- **Accessory**: Trang sức/Hiệu ứng đặc biệt hoặc tăng HP.
- **Shoes**: Giày/Tốc độ di chuyển.

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
#### Chest (Giáp)
- **Cloth Armor**
  - itemType: Equipment | equipmentType: Chest
  - stats: armor +1, hpRegen +1
- **Leather Armor**
  - itemType: Equipment | equipmentType: Chest
  - stats: armor +3, hpRegen +1
- **Iron Armor**
  - itemType: Equipment | equipmentType: Chest
  - stats: armor +4, hpRegen +3, controlResistance +10%
- **Meticulous Iron Armor**
  - itemType: Equipment | equipmentType: Chest
  - stats: armor +6, hpRegen +4, controlResistance +14%, health +5
- **Meteorite Armor**
  - itemType: Equipment | equipmentType: Chest
  - stats: armor +9, hpRegen +5, controlResistance +17%, health +10, dmgR +10%
- **Royal Armor**
  - itemType: Equipment | equipmentType: Chest
  - stats: armor +16, hpRegen +8, controlResistance +20%, health +15, dmgR +25%

#### Weapon (Vũ khí)
- **Basic Dagger**
  - itemType: Equipment | equipmentType: Weapon
  - stats: damage +2
- **Farmer Axe**
  - itemType: Equipment | equipmentType: Weapon
  - stats: damage +4, skillAmp +5%
- **Warrior Axe**
  - itemType: Equipment | equipmentType: Weapon
  - stats: damage +7, skillAmp +8%, critChance +5%
- **Hunter Spear**
  - itemType: Equipment | equipmentType: Weapon
  - stats: damage +10, skillAmp +10%, critChance +8%, armorPierce +15%
- **Serrated Sword**
  - itemType: Equipment | equipmentType: Weapon
  - stats: damage +14, skillAmp +12%, critChance +10%, armorPierce +25%, critDamage +10%
- **Royal Sword**
  - itemType: Equipment | equipmentType: Weapon
  - stats: damage +18, skillAmp +15%, critChance +12%, armorPierce +40%, critDamage +15%, lifesteal +5%
- **Royal Mace**
  - itemType: Equipment | equipmentType: Weapon
  - stats: damage +18, skillAmp +15%, critChance +12%, armorPierce +40%, critDamage +15%, lifesteal +5%
- **Royal Spear**
  - itemType: Equipment | equipmentType: Weapon
  - stats: damage +18, skillAmp +15%, critChance +12%, armorPierce +40%, critDamage +15%, lifesteal +5%

#### Accessory (Trang sức)
- **Silver Ring**
  - itemType: Equipment | equipmentType: Accessory
  - stats: skillAmp +10%, cooldownReduction +10%, hpRegen +3
- **Gemstone Necklace**
  - itemType: Equipment | equipmentType: Accessory
  - stats: skillAmp +18%, cooldownReduction +14%, hpRegen +4, critChance +5%
- **Ruby Necklace**
  - itemType: Equipment | equipmentType: Accessory
  - stats: skillAmp +26%, cooldownReduction +18%, hpRegen +5, critChance +12%, critDamage +12%
- **Royal Silver Medal**
  - itemType: Equipment | equipmentType: Accessory
  - stats: skillAmp +34%, cooldownReduction +22%, hpRegen +6, critChance +18%, critDamage +24%, health +10, lifesteal +10%
- **Royal Gold Medal**
  - itemType: Equipment | equipmentType: Accessory
  - stats: skillAmp +42%, cooldownReduction +26%, hpRegen +7, critChance +24%, critDamage +37%, health +16, lifesteal +20%
- **Lord's Necklace**
  - itemType: Equipment | equipmentType: Accessory
  - stats: skillAmp +50%, cooldownReduction +30%, hpRegen +8, critChance +30%, critDamage +50%, health +22, lifesteal +30%

#### Shoes (Giày)
- **Leather Shoes**
  - itemType: Equipment | equipmentType: Shoes
  - stats: movementSpeed +0.4, cooldownReduction +5%
- **Nomad Shoes**
  - itemType: Equipment | equipmentType: Shoes
  - stats: movementSpeed +0.55, cooldownReduction +8%, controlResistance +5%
- **Sturdy Shoes**
  - itemType: Equipment | equipmentType: Shoes
  - stats: movementSpeed +0.7, cooldownReduction +10%, controlResistance +8%, armor +4
- **Ghoststone Shoes**
  - itemType: Equipment | equipmentType: Shoes
  - stats: movementSpeed +0.85, cooldownReduction +12%, controlResistance +10%, armor +6, armorPierce +5%
- **Heaven Shoes**
  - itemType: Equipment | equipmentType: Shoes
  - stats: movementSpeed +1.0, cooldownReduction +15%, controlResistance +12%, armor +8, armorPierce +10%, lifesteal +5%

### Consumable
- **Green Grape**
  - itemType: Consumable
  - mô tả: Hồi 2 HP ngay lập tức.
- **Cooked Meat**
  - itemType: Consumable
  - mô tả: Hồi 5 HP ngay lập tức.
- **Sweet Cake**
  - itemType: Consumable
  - mô tả: Hồi 3 HP ngay, hồi 1 HP/giây trong 5 giây.
- **Vitality Potion**
  - itemType: Consumable
  - mô tả: Hồi 10 HP ngay, hồi 2 HP/giây trong 5 giây.
- **Witch's Vitality Potion**
  - itemType: Consumable
  - mô tả: Hồi 20 HP ngay, hồi 2 HP/giây trong 10 giây.
- **Strength Potion**
  - itemType: Consumable
  - mô tả: Tăng 10 ATK trong 60 giây.
- **Witch's Strength Potion**
  - itemType: Consumable
  - mô tả: Tăng 10 ATK, 100% critDamage trong 60 giây.
- **Antidote**
  - itemType: Consumable
  - mô tả: Giải toàn bộ hiệu ứng xấu.
- **Immunity Potion**
  - itemType: Consumable
  - mô tả: Miễn nhiễm hiệu ứng xấu trong 60 giây.
- **Holy Water**
  - itemType: Consumable
  - mô tả: Giải + miễn nhiễm hiệu ứng xấu trong 120 giây.
- **Resistance Potion**
  - itemType: Consumable
  - mô tả: Tăng 20% dmgR trong 60 giây.
- **Dragon's Blood**
  - itemType: Consumable
  - mô tả: Tăng 50% dmgR trong 60 giây.

### Material
- **Coal**
  - itemType: Material
  - mô tả: Fuel, can be traded with Usagi for money.
- **Beast Fang**
  - itemType: Material
  - mô tả: Material needed when buying armor from Usagi.
- **Blue Crystal**
  - itemType: Material
  - mô tả: Can be traded for some weapons from Usagi.
- **Red Crystal**
  - itemType: Material
  - mô tả: Can be traded for some weapons from Usagi.
- **Strange Crystal**
  - itemType: Material
  - mô tả: Trade for a certain weapon from Usagi.
- **Diamond**
  - itemType: Material
  - mô tả: Trade for a certain weapon from Usagi.
- **Bird Feather**
  - itemType: Material
  - mô tả: Trade for shoes from Usagi.
- **Phoenix Feather**
  - itemType: Material
  - mô tả: Trade for shoes from Usagi.
- **Fox Tail**
  - itemType: Material
  - mô tả: Trade for potions from Usagi.
- **Frog Leg**
  - itemType: Material
  - mô tả: Trade for potions from Usagi.
- **Snail Shell**
  - itemType: Material
  - mô tả: Trade for potions from Usagi.
- **Octopus Tentacle**
  - itemType: Material
  - mô tả: Trade for potions from Usagi.

### Quest
- **Scroll**
  - itemType: Quest
  - mô tả: A scroll containing information from the king.
- **Map**
  - itemType: Quest
  - mô tả: A map showing the way.
- **Key 1**
  - itemType: Quest
  - mô tả: The first key.
- **Key 2**
  - itemType: Quest
  - mô tả: The second key.
- **Key 3**
  - itemType: Quest
  - mô tả: The third key.
- **Key 4**
  - itemType: Quest
  - mô tả: The fourth key.
- **Key 5**
  - itemType: Quest
  - mô tả: The fifth key.
- **Angel's Mirror**
  - itemType: Quest
  - mô tả: A gift from the king given by an angel, but it was stolen.
- **Witch's Hat**
  - itemType: Quest
  - mô tả: Return this to the witch for guidance.

### Skill
- **Shoot**
  - itemType: Skill
  - mô tả: Hachi bắn random thing.
- **Dash**
  - itemType: Skill
  - mô tả: Hachi giúp bạn lướt đi.
- **Heal**
  - itemType: Skill
  - mô tả: Hachi hồi máu cho bạn.
- **Block**
  - itemType: Skill
  - mô tả: Hachi đỡ đòn cho bạn.
