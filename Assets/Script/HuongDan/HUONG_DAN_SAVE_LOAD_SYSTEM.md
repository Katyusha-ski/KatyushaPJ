# HƯỚNG DẪN XÂY DỰNG HỆ THỐNG SAVE/LOAD HOÀN CHỈNH

> Hướng dẫn chi tiết từng bước để xây dựng hệ thống Save/Load game trong Unity

---

## MỤC LỤC

1. [Mở rộng SaveData](#-bước-1-mở-rộng-savedata)
2. [Thêm Error Handling vào SaveManager](#-bước-2-thêm-error-handling-vào-savemanager)
3. [Thêm Error Handling vào SerializableItemStack](#-bước-3-thêm-error-handling-vào-serializableitemstack)
4. [Cải tiến GameManager - Phần 1 (Tracking)](#-bước-4-cải-tiến-gamemanager---phần-1-tracking)
5. [Cải tiến GameManager - Phần 2 (Save)](#-bước-5-cải-tiến-gamemanager---phần-2-save)
6. [Cải tiến GameManager - Phần 3 (Load)](#-bước-6-cải-tiến-gamemanager---phần-3-load)
7. [Cải tiến NewGame](#-bước-7-cải-tiến-newgame)
8. [Cải tiến MainMenuUI](#-bước-8-cải-tiến-mainmenuui)
9. [Xác nhận Inventory đã có methods](#-bước-9-xác-nhận-inventory-đã-có-methods)
10. [Tạo SavePoint Script](#-bước-10-tạo-savepoint-script)
11. [Tạo LevelEndTrigger Script](#-bước-11-tạo-levelendtrigger-script)
12. [Test Checklist](#-bước-12-test-checklist)

---

## BƯỚC 1: MỞ RỘNG SAVEDATA

### File: `Assets\Script\SaveSystem\SaveData.cs`

Mục tiêu: Thêm các trường dữ liệu để lưu scene, trạng thái player, metadata

#### Code hiện tại:
```csharp
[System.Serializable]
public class SaveData
{
    public int currentLevel;
    public List<SerializableItemStack> inventoryItem;
}
```

#### Cần thêm vào (sau `inventoryItem`):
```csharp
// Equipment data
public List<SerializableItemStack> equipmentItem;

// Scene information
public int currentSceneIndex;
public string currentSceneName;

// Player state - để restore vị trí và máu
public int playerHealth;
public float playerPositionX;
public float playerPositionY;
public float playerPositionZ;

// Metadata - để hiển thị thông tin save
public string saveDateTime;  // "2024-12-20 14:30:15"
public float playTime;        // Tổng thời gian chơi (giây)
```

#### Kết quả cuối cùng:
```csharp
[System.Serializable]
public class SaveData
{
    // Game progression
    public int currentLevel;
    
    // Inventory data
    public List<SerializableItemStack> inventoryItem;
    public List<SerializableItemStack> equipmentItem;
    
    // Scene information
    public int currentSceneIndex;
    public string currentSceneName;
    
    // Player state
    public int playerHealth;
    public float playerPositionX;
    public float playerPositionY;
    public float playerPositionZ;
    
    // Metadata
    public string saveDateTime;
    public float playTime;
}
```

#### Giải thích:
| Field | Mục đích |
|-------|----------|
| `currentSceneIndex` | Build index của scene (0, 1, 2...) - dùng để load nhanh |
| `currentSceneName` | Tên scene - backup nếu index thay đổi |
| `playerHealth` | Máu hiện tại |
| `playerPosition(X,Y,Z)` | Vị trí player khi save |
| `saveDateTime` | Thời gian lưu - hiển thị trong UI |
| `playTime` | Tổng thời gian chơi - hiển thị trong UI |

---

## BƯỚC 2: THÊM ERROR HANDLING VÀO SAVEMANAGER

### File: `Assets\Script\SaveSystem\SaveManager.cs`

Mục tiêu: Thêm try-catch, validate JSON, logging

### 2.1. Sửa method `SaveGame()`

#### Tìm:
```csharp
public static void SaveGame(SaveData gameData)
{
    string json = JsonUtility.ToJson(gameData, true);
    File.WriteAllText(savePath, json);
}
```

#### Thay bằng:
```csharp
public static void SaveGame(SaveData gameData)
{
    try
    {
        string json = JsonUtility.ToJson(gameData, true);
        File.WriteAllText(savePath, json);
        Debug.Log($"Game saved to: {savePath}");
    }
    catch (System.Exception e)
    {
        Debug.LogError($"Failed to save game: {e.Message}");
    }
}
```

#### Giải thích:
- `try-catch`: Bắt lỗi nếu không ghi được file (ví dụ: đầy, không có quyền)
- `Debug.Log`: Xác nhận save thành công
- `Debug.LogError`: Báo lỗi chi tiết nếu thất bại

---

### 2.2. Sửa method `LoadGame()`

#### Tìm:
```csharp
public static SaveData LoadGame()
{
    if (File.Exists(savePath))
    {
        string json = File.ReadAllText(savePath);
        return JsonUtility.FromJson<SaveData>(json);
    }
    return null;
}
```

#### Thay bằng:
```csharp
public static SaveData LoadGame()
{
    if (File.Exists(savePath))
    {
        try
        {
            string json = File.ReadAllText(savePath);
            
            // Validate JSON không rỗng
            if (string.IsNullOrWhiteSpace(json))
            {
                Debug.LogError("Save file is empty!");
                return null;
            }
            
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            Debug.Log("Game loaded successfully!");
            return data;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load game: {e.Message}");
            return null;
        }
    }
    
    Debug.LogWarning("Save file not found!");
    return null;
}
```

#### Giải thích:
- `string.IsNullOrWhiteSpace(json)`: Kiểm tra file có nội dung không
- Catch exception nếu JSON bị corrupt hoặc format sai

---

### 2.3. Sửa method `DeleteSave()`

#### Tìm:
```csharp
public static void DeleteSave()
{
    if (File.Exists(savePath))
    {
        File.Delete(savePath);
    }
}
```

#### Thay bằng:
```csharp
public static void DeleteSave()
{
    try
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("Save file deleted!");
        }
    }
    catch (System.Exception e)
    {
        Debug.LogError($"Failed to delete save: {e.Message}");
    }
}
```

---

## BƯỚC 3: THÊM ERROR HANDLING VÀO SERIALIZABLEITEMSTACK

### File: `Assets\Script\SaveSystem\SerializableItemStack.cs`

Mục tiêu: Validate input, tìm item trong subfolder nếu không tìm thấy trong folder chính

### Tìm method `ToItemStack()`:
```csharp
public ItemStack ToItemStack()
{
    ItemData itemData = Resources.Load<ItemData>($"Items/{itemName}");
    return new ItemStack(itemData, amount);
}
```

### Thay bằng:
```csharp
public ItemStack ToItemStack()
{
    // Validate input
    if (string.IsNullOrEmpty(itemName))
    {
        Debug.LogWarning("SerializableItemStack: itemName is null or empty!");
        return null;
    }

    // Try load from main Items folder
    ItemData itemData = Resources.Load<ItemData>($"Items/{itemName}");
    
    // If not found, search in subfolders
    if (itemData == null)
    {
        string[] subfolders = { "Consumables", "Equipment", "Materials", "Quest", "Skills" };
        
        foreach (string folder in subfolders)
        {
            itemData = Resources.Load<ItemData>($"Items/{folder}/{itemName}");
            if (itemData != null)
            {
                Debug.Log($"Found ItemData in subfolder: Items/{folder}/{itemName}");
                break;
            }
        }
        
        if (itemData == null)
        {
            Debug.LogError($"ItemData not found: {itemName}. Make sure it exists in Resources/Items/");
            return null;
        }
    }

    return new ItemStack(itemData, amount);
}
```

#### Giải thích:
- `string.IsNullOrEmpty`: Kiểm tra tên item hợp lệ
- Tìm trong folder chính: `Resources/Items/Sword`
- Nếu không tìm thấy, duyệt qua các subfolder: `Resources/Items/Equipment/Sword`
- Trả về `null` thay vì crash game nếu không tìm thấy

#### Tại sao cần:
- Bạn có thể tổ chức items trong các subfolder
- Không crash game nếu thiếu item
- Log rõ ràng để debug

---

## BƯỚC 4: CẢI TIẾN GAMEMANAGER - PHẦN 1 (TRACKING)

### File: `Assets\Script\Manager\GameManager.cs`

### 4.1. Thêm using directive

Tìm dòng dùng:
```csharp
using UnityEngine;
```

Thêm sau:
```csharp
using UnityEngine.SceneManagement;
```

Giải thích: Cần để dùng `SceneManager.GetActiveScene()`, `SceneManager.LoadScene()`, v.v.

---

### 4.2. Thêm private fields

Tìm:
```csharp
[Header("Game state")]
public int currentLevel = 1;
```

Thêm vào sau:
```csharp
// Play time tracking
private float playTime = 0f;

// Temporary save data for loading
private SaveData tempSaveData;
```

Giải thích:
- `playTime`: Đếm tổng thời gian chơi (cộng dồn mỗi frame)
- `tempSaveData`: Lưu tạm data khi load game, dùng sau khi scene load xong

---

### 4.3. Thêm Update method (theo dõi play time)

Thêm method mới sau `ResumeGame()`:
```csharp
private void Update()
{
    // Track play time only during gameplay
    if (CurrentGameState == GameState.Gameplay)
    {
        playTime += Time.deltaTime;
    }
}
```

Giải thích:
- Chỉ đếm khi `GameState == Gameplay`
- Không đếm khi pause hoặc ở main menu
- `Time.deltaTime`: Thời gian giữa 2 frame (giây)

---

## BƯỚC 5: CẢI TIẾN GAMEMANAGER - PHẦN 2 (SAVE)

### Tìm method `SaveGame()` hiện tại:
```csharp
public void SaveGame()
{
    Debug.Log("Save game called - not implemented yet");
}
```

### Thay thế HOÀN TOÀN bằng:
```csharp
/// <summary>
/// Save all game state (inventory, equipment, scene, player)
/// </summary>
public void SaveGame()
{
    // Check inventory instance
    if (Inventory.Instance == null)
    {
        Debug.LogError("Inventory instance was not found!");
        return;
    }

    // Get current scene
    Scene currentScene = SceneManager.GetActiveScene();

    // Get player state
    int playerHealth = 0;
    Vector3 playerPosition = Vector3.zero;
    
    if (player != null)
    {
        Health health = player.GetComponent<Health>();
        if (health != null)
        {
            playerHealth = health.CurrentHealth;
        }
        playerPosition = player.transform.position;
    }

    // Create save data
    SaveData data = new SaveData 
    {
        currentLevel = this.currentLevel,
        inventoryItem = Inventory.Instance.GetSerializableInventory(),
        equipmentItem = Inventory.Instance.GetSerializableEquipment(),
        
        // Scene info
        currentSceneIndex = currentScene.buildIndex,
        currentSceneName = currentScene.name,
        
        // Player info
        playerHealth = playerHealth,
        playerPositionX = playerPosition.x,
        playerPositionY = playerPosition.y,
        playerPositionZ = playerPosition.z,
        
        // Metadata
        saveDateTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
        playTime = this.playTime
    };

    // Save to file
    SaveManager.SaveGame(data);
    Debug.Log($"Game saved! Scene: {currentScene.name}, Level: {currentLevel}");
}
```

Giải thích tóm tắt:
- Kiểm tra Inventory
- Lấy scene hiện tại
- Lấy trạng thái player (máu, vị trí)
- Tạo SaveData object với các trường cần thiết
- Gọi SaveManager để ghi file

---

## BƯỚC 6: CẢI TIẾN GAMEMANAGER - PHẦN 3 (LOAD)

### Tìm method `LoadGame()` hiện tại:
```csharp
public void LoadGame()
{
    Debug.Log("Load game called - not implemented yet");
}
```

### Thay thế HOÀN TOÀN bằng:
```csharp
/// <summary>
/// Load game and switch to saved scene
/// </summary>
public void LoadGame()
{
    // Load save data
    SaveData saveData = SaveManager.LoadGame();

    // Check if save exists
    if (saveData == null)
    {
        Debug.LogWarning("No save data found to load.");
        return;
    }

    // Load game state
    this.currentLevel = saveData.currentLevel;
    this.playTime = saveData.playTime;

    // Store temp data for after scene loads
    tempSaveData = saveData;

    // Load saved scene
    if (saveData.currentSceneIndex > 0) // Don't load Main Menu (index 0)
    {
        Debug.Log($"Loading scene: {saveData.currentSceneName} (Index: {saveData.currentSceneIndex})");
        
        // Register callback
        SceneManager.sceneLoaded += OnSceneLoadedAfterLoadGame;
        
        // Load scene
        SceneManager.LoadScene(saveData.currentSceneIndex);
    }
    else
    {
        Debug.LogWarning("Invalid scene index in save data!");
    }
}
```

### Thêm callback method (sau `LoadGame()`):
```csharp
/// <summary>
/// Callback after scene is loaded - restore inventory and player
/// </summary>
private void OnSceneLoadedAfterLoadGame(Scene scene, LoadSceneMode mode)
{
    // Unsubscribe để không gọi lại
    SceneManager.sceneLoaded -= OnSceneLoadedAfterLoadGame;

    if (tempSaveData == null) return;

    // Load inventory after scene is ready
    if (Inventory.Instance != null)
    {
        Inventory.Instance.LoadSerializableInventory(tempSaveData.inventoryItem);
        Inventory.Instance.LoadSerializableEquipment(tempSaveData.equipmentItem);
    }

    // Restore player state (delay to ensure player is spawned)
    Invoke(nameof(RestorePlayerState), 0.2f);

    Debug.Log($"Game loaded! Level: {currentLevel}, Play time: {playTime:F2}s");
}
```

### Thêm method restore player (sau callback):
```csharp
/// <summary>
/// Restore player position and health using SetHealth method
/// </summary>
private void RestorePlayerState()
{
    if (tempSaveData == null) return;

    // Find player if not assigned
    if (player == null)
    {
        player = FindObjectOfType<PlayerController>();
    }

    if (player != null)
    {
        // Restore position
        Vector3 savedPosition = new Vector3(
            tempSaveData.playerPositionX,
            tempSaveData.playerPositionY,
            tempSaveData.playerPositionZ
        );
        player.transform.position = savedPosition;

        // Restore health using SetHealth method (NO REFLECTION NEEDED!)
        Health health = player.GetComponent<Health>();
        if (health != null)
        {
            health.SetHealth(tempSaveData.playerHealth);
            Debug.Log($"Player state restored: Health={tempSaveData.playerHealth}, Position={savedPosition}");
        }
        else
        {
            Debug.LogWarning("Health component not found on player!");
        }
    }
    else
    {
        Debug.LogWarning("Player not found in scene for state restoration!");
    }

    // Clear temp data
    tempSaveData = null;
}
```

Lưu ý quan trọng:
- Không cần reflection nữa!
- Dùng `health.SetHealth(tempSaveData.playerHealth)` cho an toàn và dễ maintain

So sánh phương pháp:
| Phương pháp | Code | Ưu điểm | Nhược điểm |
|-------------|------|---------|------------|
| Reflection (Cũ) | `currentHealthField.SetValue(health, value);` | Truy cập được private field | Chậm, dễ lỗi, khó maintain |
| SetHealth (Mới) | `health.SetHealth(tempSaveData.playerHealth);` | Đơn giản, an toàn, có validation | Cần method public |

---

## BƯỚC 7: CẢI TIẾN NEWGAME

### Tìm method `NewGame()` hiện tại:
```csharp
public void NewGame()
{
    SaveManager.DeleteSave();
    currentLevel = 1;
    Debug.Log("New game started!");
}
```

### Thay bằng:
```csharp
/// <summary>
/// Start a new game (delete save, reset state, load first scene)
/// </summary>
public void NewGame()
{
    // Delete old save
    SaveManager.DeleteSave();

    // Reset game state
    currentLevel = 1;
    playTime = 0f;

    // Clear inventory
    if (Inventory.Instance != null)
    {
        for (int i = 0; i < Inventory.Instance.itemSlots.Count; i++)
            Inventory.Instance.itemSlots[i] = null;
        for (int i = 0; i < Inventory.Instance.equipment.Length; i++)
            Inventory.Instance.equipment[i] = null;

        Inventory.Instance.TriggerInventoryChanged();
    }

    Debug.Log("New game started!");
    
    // Load first gameplay scene
    if (GameSceneController.Instance != null)
    {
        GameSceneController.Instance.LoadGameScene("GrassScene");
    }
}
```

### Thêm method mới:
```csharp
/// <summary>
/// Get save info for UI display
/// </summary>
public SaveData GetSaveInfo()
{
    return SaveManager.LoadGame();
}
```

Dùng để hiển thị thông tin save trong UI mà không load game.

---

## BƯỚC 8: CẢI TIẾN MAINMENUUI

### File: `Assets\Script\UI\MainMenuUI.cs`

> **💡 Lưu ý quan trọng:** Vì bạn tạo buttons trực tiếp trong Unity Editor và assign onClick events qua Inspector nên **KHÔNG CẦN khai báo button references trong code**. Chỉ cần cập nhật một method duy nhất.

### 8.1. Cập nhật OnPlayButtonClick

#### Tìm:
```csharp
public void OnPlayButtonClick()
{
    GameSceneController.Instance.LoadGameScene("GrassScene");
}
```

#### Thay bằng:
```csharp
public void OnPlayButtonClick()
{
    // Start new game instead of loading scene directly
    if (GameManager.Instance != null)
    {
        GameManager.Instance.NewGame();
    }
}
```

#### Giải thích: 
Gọi `NewGame()` thay vì load scene trực tiếp để:
- Xóa save cũ
- Reset game state (currentLevel, playTime)
- Clear inventory
- Load scene đầu tiên

### 8.2. Các methods khác đã OK - KHÔNG CẦN SỬA:

✅ **OnLoadButtonClick()** - Đã gọi `GameManager.Instance.LoadGame()`
✅ **OnContinueButtonClick()** - Đã gọi `GameManager.Instance.LoadGame()`  
✅ **OnSaveButtonClick()** - Đã gọi `GameManager.Instance.SaveGame()`
✅ **OnNewGameButtonClick()** - Đã gọi `GameManager.Instance.NewGame()`

### 8.3. Optional: Enable/Disable buttons tự động

**Nếu muốn buttons tự động enable/disable dựa trên save file:**

#### Option 1: Trong Unity Editor (Khuyến nghị)
1. Select Continue/Load buttons trong Inspector
2. Uncheck `Interactable` khi chưa có save file
3. Check `Interactable` sau khi có save file

#### Option 2: Tự động qua code (Advanced)
```csharp
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject aboutMePanel;
    
    [Header("Optional: Dynamic Button Control")]
    public Button continueButton;  // Kéo từ Unity Editor nếu muốn auto control
    public Button loadButton;      // Kéo từ Unity Editor nếu muốn auto control

    private void Start()
    {
        if (aboutMePanel != null)
        {
            aboutMePanel.SetActive(false);
        }
        
        // Optional: Auto enable/disable buttons
        UpdateButtonStates();
    }
    
    /// <summary>
    /// Optional: Update button states based on save file existence
    /// </summary>
    private void UpdateButtonStates()
    {
        bool hasSave = GameManager.Instance != null && GameManager.Instance.HasSaveFile();

        if (continueButton != null)
        {
            continueButton.interactable = hasSave;
        }

        if (loadButton != null)
        {
            loadButton.interactable = hasSave;
        }
    }

    // ... existing methods không thay đổi
}
```

> **🎯 Khuyến nghị:** Sử dụng **Option 1** (Unity Editor) vì đơn giản và phù hợp với cách setup hiện tại của bạn.

---

## BƯỚC 9: XÁC NHẬN INVENTORY ĐÃ CÓ METHODS

### File: `Assets\Script\Inventory\Inventory.cs`

Kiểm tra xem các methods sau đã có chưa:

- `public List<SerializableItemStack> GetSerializableInventory()`
- `public List<SerializableItemStack> GetSerializableEquipment()`
- `public void LoadSerializableInventory(List<SerializableItemStack> serializableInventory)`
- `public void LoadSerializableEquipment(List<SerializableItemStack> serializableEquipment)`

Nếu CHƯA CÓ: scroll xuống cuối class và thêm vào trước dấu `}` cuối cùng.

*(Nếu đã có sẵn rồi thì bỏ qua bước này.)*

---

## BƯỚC 10: TẠO SAVEPOINT SCRIPT

### Tạo file mới: `Assets\Script\SaveSystem\SavePoint.cs`

```csharp
using UnityEngine;

/// <summary>
/// Save Point: Manual save trigger when player enters
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class SavePoint : MonoBehaviour
{
    [Header("Save Point Settings")]
    [Tooltip("Auto save when player enters trigger")]
    public bool autoSaveOnEnter = true;
    
    [Tooltip("Show message when saved")]
    public bool showSaveMessage = true;
    
    [Tooltip("Save point can only be used once")]
    public bool singleUse = false;
    
    [Header("Visual Feedback")]
    public GameObject saveEffectPrefab;
    public AudioClip saveSound;
    
    private bool hasBeenUsed = false;

    private void Awake()
    {
        Collider2D col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (singleUse && hasBeenUsed)
            {
                Debug.Log($"Save Point '{gameObject.name}' already used!");
                return;
            }

            if (autoSaveOnEnter)
            {
                SaveGameAtPoint();
            }
        }
    }

    public void SaveGameAtPoint()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager not found!");
            return;
        }

        GameManager.Instance.SaveGame();
        hasBeenUsed = true;

        if (saveEffectPrefab != null)
        {
            Instantiate(saveEffectPrefab, transform.position, Quaternion.identity);
        }

        if (saveSound != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(saveSound);
        }

        if (showSaveMessage)
        {
            Debug.Log($"Game Saved at: {gameObject.name}");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, Vector3.one * 2f);
    }
}
```

Cách sử dụng:
1. Tạo Empty GameObject trong scene
2. Add component `SavePoint`
3. Add `Box Collider 2D`, set `Is Trigger = true`
4. Player chạm vào sẽ Auto save (nếu bật)

---

## BƯỚC 11: TẠO LEVELENDTRIGGER SCRIPT

### Tạo file mới: `Assets\Script\SaveSystem\LevelEndTrigger.cs`

```csharp
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Level End Trigger: Auto save and load next scene
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class LevelEndTrigger : MonoBehaviour
{
    [Header("Level Transition Settings")]
    [Tooltip("Load next scene in build index")]
    public bool loadNextScene = true;
    
    [Tooltip("Or specify scene name")]
    public string targetSceneName;
    
    [Header("Save Settings")]
    [Tooltip("Auto save before loading next scene")]
    public bool saveBeforeTransition = true;
    
    [Header("Visual Settings")]
    public GameObject transitionEffectPrefab;
    public float transitionDelay = 1f;

    private bool isTriggered = false;

    private void Awake()
    {
        Collider2D col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isTriggered)
        {
            isTriggered = true;
            CompleteLevelAndTransition();
        }
    }

    private void CompleteLevelAndTransition()
    {
        Debug.Log($"Level completed! Transitioning...");

        if (saveBeforeTransition && GameManager.Instance != null)
        {
            GameManager.Instance.currentLevel++;
            GameManager.Instance.SaveGame();
            Debug.Log($"Progress saved! Current Level: {GameManager.Instance.currentLevel}");
        }

        if (transitionEffectPrefab != null)
        {
            Instantiate(transitionEffectPrefab, transform.position, Quaternion.identity);
        }

        Invoke(nameof(LoadNextLevel), transitionDelay);
    }

    private void LoadNextLevel()
    {
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            if (GameSceneController.Instance != null)
            {
                GameSceneController.Instance.LoadGameScene(targetSceneName);
            }
            else
            {
                SceneManager.LoadScene(targetSceneName);
            }
        }
        else if (loadNextScene)
        {
            if (GameSceneController.Instance != null)
            {
                GameSceneController.Instance.LoadNextScene();
            }
            else
            {
                // Fallback: load next by build index
                Scene current = SceneManager.GetActiveScene();
                SceneManager.LoadScene(current.buildIndex + 1);
            }
        }
    }
}
```

---

## BƯỚC 12: TEST CHECKLIST

- [ ] SaveGame ghi file thành công (kiểm tra Debug.Log)
- [ ] LoadGame load đúng scene và khôi phục inventory
- [ ] Player được restore vị trí và máu
- [ ] NewGame xóa save cũ và reset trạng thái
- [ ] MainMenuUI cập nhật trạng thái nút (Continue/Load)
- [ ] SavePoint hoạt động trong scene
- [ ] LevelEndTrigger lưu tiến độ và chuyển scene

---

Hoàn tất: tài liệu đã được chỉnh sửa để sửa lỗi chính tả, phục hồi dấu tiếng Việt và làm rõ các chú thích, giải thích. Các đoạn mã giữ nguyên chức năng; tôi chỉ sửa các chú thích trong markdown/code comments để dễ đọc. Nếu bạn muốn, tôi có thể:
- 1) tạo Pull Request với phiên bản đã chỉnh sửa lên repository,
- 2) hoặc chỉ xuất file đã sửa (như ở trên) để bạn dán vào repo,
- 3) hoặc sửa trực tiếp các file .cs liên quan (nếu muốn cập nhật code comments hoặc thêm method thiếu) — cho tôi biết lựa chọn của bạn.