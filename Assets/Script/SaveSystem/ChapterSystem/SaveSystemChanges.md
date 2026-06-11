# So sánh Save System cũ và mới

## 1. GameManager.currentLevel → ChapterManager

### Cũ
```csharp
// GameManager tự quản lý số level
public int currentLevel = 1;

// LevelEndTrigger tự tăng + save
GameManager.Instance.currentLevel++;
GameManager.Instance.SaveGame();
```

### Mới
```csharp
// ChapterManager quản lý chapter
[SerializeField] private int currentChapterIndex;
public int CurrentChapterNumber => currentIndex + 1;

// LevelEndTrigger chỉ gọi ChapterManager
ChapterManager.Instance.CompleteChapter();
// → ChapterManager tự save + load scene làng mới
```

## 2. Scene transition

### Cũ
- `LevelEndTrigger.LoadNextLevel()` load scene theo **build index** (scene 1 → 2 → 3...)
- Tất cả scene đều dùng chung 1 trigger

### Mới
| Trigger | Scene đặt | Hành động |
|---------|-----------|-----------|
| `LevelEndTrigger` | **Main level** (cuối màn) | `CompleteChapter()` → tăng chapter → lưu → load "Village" |
| `VillageExitTrigger` | **Village** (cổng ra) | `GoToMainScene()` → load màn chính của chapter hiện tại |

## 3. SaveData field

### Cũ
```csharp
public int currentLevel;  // lưu số level
```

### Mới
```csharp
public int currentChapter;  // lưu số chapter
```

## 4. ScriptableObject — ChapterDataSO

Thay vì chỉ 1 con số, dùng SO để định nghĩa từng chapter:

```csharp
public class ChapterDataSO : ScriptableObject {
    public int chapterID;        // 1, 2, 3...
    public string chapterName;   // "Chapter 1: The Beginning"
    public string mainSceneName; // "Level_1", "Level_2"...
}
```

Tạo trong Project: `Assets/Resources/ChapterData/Chapter1.asset`, `Chapter2.asset`...

## 5. Luồng game mới

```
New Game
  → SetChapter(1)
  → Load "Village"

Village → cổng làng (VillageExitTrigger)
  → ChapterManager.GoToMainScene()
  → Load main scene (VD: "Level_1")

Main level → cuối màn (LevelEndTrigger)
  → ChapterManager.CompleteChapter()
  → currentChapterIndex++
  → SaveGame()
  → Load "Village" (làng mới, shop tự unlock item chapter mới)
```

## 6. Shop unlock

```csharp
// ShopEntrySO có field:
public int unlockChapter = 0;  // unlock ở chapter mấy

// ShopManager.UnlockByChapter() lọc:
entry.isUnlocked = entry.unlockChapter <= currentChapter;
```

File cũ: `LevelEndTrigger.cs`, `GameManager.cs`, `SaveData.cs`
File mới: `ChapterManager.cs`, `ChapterDataSO.cs`, `VillageExitTrigger.cs`
