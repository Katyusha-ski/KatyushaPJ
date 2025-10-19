# ?? H??NG D?N X�Y D?NG H? TH?NG SAVE/LOAD HO�N CH?NH

> **H??ng d?n chi ti?t t?ng b??c ?? x�y d?ng h? th?ng Save/Load game trong Unity**

---

## ?? **M?C L?C**

1. [M? r?ng SaveData](#-b??c-1-m?-r?ng-savedata)
2. [Th�m Error Handling v�o SaveManager](#?-b??c-2-th�m-error-handling-v�o-savemanager)
3. [Th�m Error Handling v�o SerializableItemStack](#-b??c-3-th�m-error-handling-v�o-serializableitemstack)
4. [C?i ti?n GameManager - Ph?n 1 (Tracking)](#-b??c-4-c?i-ti?n-gamemanager---ph?n-1-tracking)
5. [C?i ti?n GameManager - Ph?n 2 (Save)](#-b??c-5-c?i-ti?n-gamemanager---ph?n-2-save)
6. [C?i ti?n GameManager - Ph?n 3 (Load)](#-b??c-6-c?i-ti?n-gamemanager---ph?n-3-load)
7. [C?i ti?n NewGame](#-b??c-7-c?i-ti?n-newgame)
8. [C?i ti?n MainMenuUI](#-b??c-8-c?i-ti?n-mainmenuui)
9. [X�c nh?n Inventory Methods](#-b??c-9-x�c-nh?n-inventory-?�-c�-methods)
10. [T?o SavePoint Script](#-b??c-10-t?o-savepoint-script)
11. [T?o LevelEndTrigger Script](#-b??c-11-t?o-levelendtrigger-script)
12. [Test Checklist](#-b??c-12-test-checklist)

---

## ?? **B??C 1: M? R?NG SAVEDATA**

### **File:** `Assets\Script\SaveSystem\SaveData.cs`

**?? M?c ti�u:** Th�m c�c tr??ng d? li?u ?? l?u scene, player state, metadata

#### **Code hi?n t?i:**
```csharp
[System.Serializable]
public class SaveData
{
    public int currentLevel;
    public List<SerializableItemStack> inventoryItem;
}
```

#### **C?n th�m v�o (sau `inventoryItem`):**
```csharp
// Equipment data
public List<SerializableItemStack> equipmentItem;

// Scene information - ?? Continue ?�ng m�n
public int currentSceneIndex;
public string currentSceneName;

// Player state - ?? restore v? tr� v� m�u
public int playerHealth;
public float playerPositionX;
public float playerPositionY;
public float playerPositionZ;

// Metadata - ?? hi?n th? th�ng tin save
public string saveDateTime;  // "2024-12-20 14:30:15"
public float playTime;        // T?ng th?i gian ch?i (gi�y)
```

#### **K?t qu? cu?i c�ng:**
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

#### **?? Gi?i th�ch:**
| Field | M?c ?�ch |
|-------|----------|
| `currentSceneIndex` | Build index c?a scene (0, 1, 2...) - d�ng ?? load nhanh |
| `currentSceneName` | T�n scene - backup n?u index thay ??i |
| `playerHealth` | M�u hi?n t?i |
| `playerPosition(X,Y,Z)` | V? tr� player khi save |
| `saveDateTime` | Th?i gian l?u - hi?n th? trong UI |
| `playTime` | T?ng th?i gian ch?i - hi?n th? trong UI |

---

## ??? **B??C 2: TH�M ERROR HANDLING V�O SAVEMANAGER**

### **File:** `Assets\Script\SaveSystem\SaveManager.cs`

**?? M?c ti�u:** Th�m try-catch, validate JSON, logging

### **2.1. S?a method `SaveGame()`**

#### **T�m:**
```csharp
public static void SaveGame(SaveData gameData)
{
    string json = JsonUtility.ToJson(gameData, true);
    File.WriteAllText(savePath, json);
}
```

#### **Thay b?ng:**
```csharp
public static void SaveGame(SaveData gameData)
{
    try
    {
        string json = JsonUtility.ToJson(gameData, true);
        File.WriteAllText(savePath, json);
        Debug.Log($"? Game saved to: {savePath}");
    }
    catch (System.Exception e)
    {
        Debug.LogError($"? Failed to save game: {e.Message}");
    }
}
```

#### **?? Gi?i th�ch:**
- `try-catch`: B?t l?i n?u kh�ng ghi ???c file (v� d?: ??a ??y, kh�ng c� quy?n)
- `Debug.Log`: X�c nh?n save th�nh c�ng
- `Debug.LogError`: B�o l?i chi ti?t n?u th?t b?i

---

### **2.2. S?a method `LoadGame()`**

#### **T�m:**
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

#### **Thay b?ng:**
```csharp
public static SaveData LoadGame()
{
    if (File.Exists(savePath))
    {
        try
        {
            string json = File.ReadAllText(savePath);
            
            // Validate JSON kh�ng r?ng
            if (string.IsNullOrWhiteSpace(json))
            {
                Debug.LogError("? Save file is empty!");
                return null;
            }
            
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            Debug.Log("? Game loaded successfully!");
            return data;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"? Failed to load game: {e.Message}");
            return null;
        }
    }
    
    Debug.LogWarning("?? Save file not found!");
    return null;
}
```

#### **?? Gi?i th�ch:**
- `string.IsNullOrWhiteSpace(json)`: Ki?m tra file c� n?i dung kh�ng
- Catch exception n?u JSON b? corrupt ho?c format sai

---

### **2.3. S?a method `DeleteSave()`**

#### **T�m:**
```csharp
public static void DeleteSave()
{
    if (File.Exists(savePath))
    {
        File.Delete(savePath);
    }
}
```

#### **Thay b?ng:**
```csharp
public static void DeleteSave()
{
    try
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("? Save file deleted!");
        }
    }
    catch (System.Exception e)
    {
        Debug.LogError($"? Failed to delete save: {e.Message}");
    }
}
```

---

## ?? **B??C 3: TH�M ERROR HANDLING V�O SERIALIZABLEITEMSTACK**

### **File:** `Assets\Script\SaveSystem\SerializableItemStack.cs`

**?? M?c ti�u:** Validate input, t�m item trong subfolder n?u kh�ng t�m th?y trong folder ch�nh

### **T�m method `ToItemStack()`:**
```csharp
public ItemStack ToItemStack()
{
    ItemData itemData = Resources.Load<ItemData>($"Items/{itemName}");
    return new ItemStack(itemData, amount);
}
```

### **Thay b?ng:**
```csharp
public ItemStack ToItemStack()
{
    // Validate input
    if (string.IsNullOrEmpty(itemName))
    {
        Debug.LogWarning("?? SerializableItemStack: itemName is null or empty!");
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
                Debug.Log($"? Found ItemData in subfolder: Items/{folder}/{itemName}");
                break;
            }
        }
        
        if (itemData == null)
        {
            Debug.LogError($"? ItemData not found: {itemName}. Make sure it exists in Resources/Items/");
            return null;
        }
    }

    return new ItemStack(itemData, amount);
}
```

#### **?? Gi?i th�ch:**
- `string.IsNullOrEmpty`: Ki?m tra t�n item h?p l?
- T�m trong folder ch�nh tr??c: `Resources/Items/Sword`
- N?u kh�ng t�m th?y, duy?t qua c�c subfolder: `Resources/Items/Equipment/Sword`
- Return `null` thay v� crash n?u kh�ng t�m th?y

#### **? T?i sao c?n?**
- ? B?n c� th? t? ch?c items trong c�c subfolder
- ? Kh�ng crash game n?u thi?u item
- ? Log r� r�ng ?? debug

---

## ?? **B??C 4: C?I TI?N GAMEMANAGER - PH?N 1 (TRACKING)**

### **File:** `Assets\Script\Manager\GameManager.cs`

### **4.1. Th�m using directive**

#### **T�m d�ng ??u file:**
```csharp
using UnityEngine;
```

#### **Th�m v�o sau:**
```csharp
using UnityEngine.SceneManagement;
```

> **?? Gi?i th�ch:** C?n ?? d�ng `SceneManager.GetActiveScene()`, `SceneManager.LoadScene()`, etc.

---

### **4.2. Th�m private fields**

#### **T�m:**
```csharp
[Header("Game state")]
public int currentLevel = 1;
```

#### **Th�m v�o sau:**
```csharp
// Play time tracking
private float playTime = 0f;

// Temporary save data for loading
private SaveData tempSaveData;
```

#### **?? Gi?i th�ch:**
- `playTime`: ??m t?ng th?i gian ch?i (c?ng d?n m?i frame)
- `tempSaveData`: L?u t?m data khi load game, d�ng sau khi scene load xong

---

### **4.3. Th�m Update method (theo d�i play time)**

#### **Th�m method m?i sau `ResumeGame()`:**
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

#### **?? Gi?i th�ch:**
- Ch? ??m khi `GameState == Gameplay`
- Kh�ng ??m khi pause ho?c ? main menu
- `Time.deltaTime`: Th?i gian gi?a 2 frame (gi�y)

---

## ?? **B??C 5: C?I TI?N GAMEMANAGER - PH?N 2 (SAVE)**

### **T�m method `SaveGame()` hi?n t?i:**
```csharp
public void SaveGame()
{
    Debug.Log("Save game called - not implemented yet");
}
```

### **Thay th? HO�N TO�N b?ng:**
```csharp
/// <summary>
/// Save all game state (inventory, equipment, scene, player)
/// </summary>
public void SaveGame()
{
    // Check inventory instance
    if (Inventory.Instance == null)
    {
        Debug.LogError("? Inventory instance was not found!");
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
    Debug.Log($"?? Game saved! Scene: {currentScene.name}, Level: {currentLevel}");
}
```

### **?? Gi?i th�ch t?ng ph?n:**

| B??c | Code | M?c ?�ch |
|------|------|----------|
| **1. Ki?m tra Inventory** | `if (Inventory.Instance == null)` | ??m b?o Inventory ?� ???c kh?i t?o |
| **2. L?y scene hi?n t?i** | `Scene currentScene = SceneManager.GetActiveScene();` | `GetActiveScene()`: Scene ?ang ch?i |
| **3. L?y player state** | `Health health = player.GetComponent<Health>();` | L?y m�u t? component `Health` |
| **4. T?o SaveData object** | `SaveData data = new SaveData { ... };` | Object initializer syntax (C# 3.0+) |
| **5. L?u v�o file** | `SaveManager.SaveGame(data);` | G?i SaveManager ?? l?u file |

---

## ?? **B??C 6: C?I TI?N GAMEMANAGER - PH?N 3 (LOAD)**

### **T�m method `LoadGame()` hi?n t?i:**
```csharp
public void LoadGame()
{
    Debug.Log("Load game called - not implemented yet");
}
```

### **Thay th? HO�N TO�N b?ng:**
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
        Debug.LogWarning("?? No save data found to load.");
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
        Debug.Log($"?? Loading scene: {saveData.currentSceneName} (Index: {saveData.currentSceneIndex})");
        
        // Register callback
        SceneManager.sceneLoaded += OnSceneLoadedAfterLoadGame;
        
        // Load scene
        SceneManager.LoadScene(saveData.currentSceneIndex);
    }
    else
    {
        Debug.LogWarning("?? Invalid scene index in save data!");
    }
}
```

### **?? Gi?i th�ch t?ng ph?n:**

| B??c | M?c ?�ch |
|------|----------|
| **1. Load data t? file** | `SaveData saveData = SaveManager.LoadGame();` |
| **2. Ki?m tra null** | Return ngay n?u kh�ng c� save file |
| **3. Load game state** | `this.currentLevel = saveData.currentLevel;` |
| **4. L?u temp data** | **T?I SAO?** V� inventory ch?a c� ? Main Menu, ph?i ??i scene load xong |
| **5. ??ng k� callback** | `+=`: ??ng k� event handler, callback s? ???c g?i SAU KHI scene load xong |
| **6. Load scene** | `SceneManager.LoadScene(saveData.currentSceneIndex);` |

---

### **Th�m callback method (sau `LoadGame()`):**
```csharp
/// <summary>
/// Callback after scene is loaded - restore inventory and player
/// </summary>
private void OnSceneLoadedAfterLoadGame(Scene scene, LoadSceneMode mode)
{
    // Unsubscribe ?? kh�ng g?i l?i
    SceneManager.sceneLoaded -= OnSceneLoadedAfterLoadGame;

    if (tempSaveData == null) return;

    // Load inventory after scene is ready
    if (Inventory.Instance != null)
    {
        Inventory.Instance.LoadSerializableInventory(tempSaveData.inventoryItem);
        Inventory.Instance.LoadSerializableEquipment(tempSaveData.equipmentItem);
    }

    // Restore player state (delay ?? ??m b?o player ?� spawn)
    Invoke(nameof(RestorePlayerState), 0.2f);

    Debug.Log($"? Game loaded! Level: {currentLevel}, Play time: {playTime:F2}s");
}
```

### **?? Gi?i th�ch:**

| B??c | Code | M?c ?�ch |
|------|------|----------|
| **1. Unsubscribe event** | `SceneManager.sceneLoaded -= OnSceneLoadedAfterLoadGame;` | X�a event handler ?? kh�ng g?i l?i ? c�c l?n load scene kh�c |
| **2. Load inventory** | `Inventory.Instance.LoadSerializableInventory(...)` | **B�Y GI?** m?i load inventory v� `Inventory.Instance` ?� ???c t?o trong scene m?i |
| **3. Invoke RestorePlayerState** | `Invoke(nameof(RestorePlayerState), 0.2f);` | `Invoke`: G?i method sau 0.2 gi�y, Delay ?? player ?� spawn xong |

---

### **Th�m method restore player (sau callback):**
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
            Debug.Log($"?? Player state restored: Health={tempSaveData.playerHealth}, Position={savedPosition}");
        }
        else
        {
            Debug.LogWarning("?? Health component not found on player!");
        }
    }
    else
    {
        Debug.LogWarning("?? Player not found in scene for state restoration!");
    }

    // Clear temp data
    tempSaveData = null;
}
```

> **?? L?u � quan tr?ng:** 
> - ? **KH�NG C?N Reflection n?a!** 
> - ? D�ng `health.SetHealth(tempSaveData.playerHealth)` ??n gi?n
> - ? Method `SetHealth()` ?� c� validation v� auto-update health bar
> - ? An to�n v� d? maintain h?n

### **?? So s�nh Tr??c vs Sau:**
| Ph??ng ph�p | Code | ?u ?i?m | Nh??c ?i?m |
|-------------|------|---------|------------|
| **Reflection (C?)** | `currentHealthField.SetValue(health, value);` | Truy c?p ???c private field | Ch?m, d? l?i, kh� maintain |
| **SetHealth (M?i)** | `health.SetHealth(tempSaveData.playerHealth);` | ??n gi?n, an to�n, c� validation | C?n method public |

---

## ?? **B??C 7: C?I TI?N NEWGAME**

### **T�m method `NewGame()` hi?n t?i:**
```csharp
public void NewGame()
{
    SaveManager.DeleteSave();
    currentLevel = 1;
    Debug.Log("New game started!");
}
```

### **Thay b?ng:**
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

    Debug.Log("?? New game started!");
    
    // Load first gameplay scene
    if (GameSceneController.Instance != null)
    {
        GameSceneController.Instance.LoadGameScene("GrassScene");
    }
}
```

### **?? Th�m ?i?m:**
- Reset `playTime`
- Clear inventory v� equipment
- Load scene ??u ti�n

---

### **Th�m method m?i:**
```csharp
/// <summary>
/// Get save info for UI display
/// </summary>
public SaveData GetSaveInfo()
{
    return SaveManager.LoadGame();
}
```

> **?? D�ng ?? hi?n th? th�ng tin save trong UI (kh�ng load game)**

---

## ?? **B??C 8: C?I TI?N MAINMENUUI**

### **File:** `Assets\Script\UI\MainMenuUI.cs`

### **8.1. Th�m button references**

#### **T�m:**
```csharp
[Header("UI Panels")]
public GameObject aboutMePanel;
```

#### **Th�m v�o sau:**
```csharp
[Header("Buttons")]
public Button continueButton;
public Button loadButton;
```

> **?? Gi?i th�ch:** Reference ?? enable/disable buttons

---

### **8.2. C?p nh?t Start method**

#### **T�m:**
```csharp
private void Start()
{
    if (aboutMePanel != null)
    {
        aboutMePanel.SetActive(false);
    }
}
```

#### **Th�m v�o cu?i Start:**
```csharp
// Enable/Disable buttons based on save file
UpdateButtonStates();
```

---

### **8.3. Th�m method UpdateButtonStates**

#### **Th�m method m?i sau Start:**
```csharp
/// <summary>
/// Update button interactable states based on save file existence
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

    Debug.Log($"?? Continue button enabled: {hasSave}");
}
```

#### **?? Gi?i th�ch:**
- `HasSaveFile()`: Ki?m tra file save c� t?n t?i kh�ng
- `button.interactable = false`: Disable button (m�u x�m, kh�ng click ???c)

---

### **8.4. C?p nh?t OnPlayButtonClick**

#### **T�m:**
```csharp
public void OnPlayButtonClick()
{
    GameSceneController.Instance.LoadGameScene("GrassScene");
}
```

#### **Thay b?ng:**
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

> **?? Gi?i th�ch:** G?i `NewGame()` thay v� load scene tr?c ti?p (?? x�a save c?, reset state)

---

### **8.5. C?p nh?t OnSaveButtonClick**

#### **T�m:**
```csharp
public void OnSaveButtonClick()
{
    GameManager.Instance.SaveGame();
}
```

#### **Thay b?ng:**
```csharp
public void OnSaveButtonClick()
{
    GameManager.Instance.SaveGame();
    UpdateButtonStates(); // Update button states after save
}
```

> **?? Gi?i th�ch:** Update button states ?? enable Continue button sau khi save

---

## ? **B??C 9: X�C NH?N INVENTORY ?� C� METHODS**

### **File:** `Assets\Script\Inventory\Inventory.cs`

**Ki?m tra xem c�c methods sau ?� c� ch?a:**

- `public List<SerializableItemStack> GetSerializableInventory()`
- `public List<SerializableItemStack> GetSerializableEquipment()`
- `public void LoadSerializableInventory(List<SerializableItemStack> serializableInventory)`
- `public void LoadSerializableEquipment(List<SerializableItemStack> serializableEquipment)`

> **? N?u CH?A C�:** scroll xu?ng cu?i class v� th�m v�o tr??c d?u `}`
> 
> *(?� c� s?n r?i d?a tr�n file context, b? qua b??c n�y)*

---

## ?? **B??C 10: T?O SAVEPOINT SCRIPT**

### **T?o file m?i:** `Assets\Script\SaveSystem\SavePoint.cs`

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
            Debug.Log($"? Game Saved at: {gameObject.name}");
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

### **?? C�ch s? d?ng:**
1. T?o Empty GameObject trong scene
2. Add component `SavePoint`
3. Add `Box Collider 2D`, set `Is Trigger = true`
4. Player ch?m v�o ? Auto save

---

## ?? **B??C 11: T?O LEVELENDTRIGGER SCRIPT**

### **T?o file m?i:** `Assets\Script\SaveSystem\LevelEndTrigger.cs`

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
            Debug.Log($"? Progress saved! Current Level: {GameManager.Instance.currentLevel}");
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
                int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
                if (nextIndex < SceneManager.sceneCountInBuildSettings)
                {
                    SceneManager.LoadScene(nextIndex);
                }
                else
                {
                    Debug.LogWarning("No more scenes to load!");
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, Vector3.one);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(transform.position, Vector3.one * 1.5f);
    }
}
```

### **?? C�ch s? d?ng:**
1. T?o Empty GameObject ? cu?i level
2. Add component `LevelEndTrigger`
3. Add `Box Collider 2D`, set `Is Trigger = true`
4. Player ch?m v�o ? Auto save + Load m�n k? ti?p

---

## ?? **B??C 12: TEST CHECKLIST**

### **Test 1: New Game**
- [ ] Click "Play" button
- [ ] Load GrassScene
- [ ] Inventory tr?ng
- [ ] Player ? v? tr� m?c ??nh

### **Test 2: Save Game**
- [ ] Nh?t v�i items
- [ ] Click "Save" ho?c ch?m SavePoint
- [ ] Console log: `"? Game saved to..."`
- [ ] File `savefile.json` t?n t?i t?i `Application.persistentDataPath`

### **Test 3: Continue**
- [ ] Tho�t v? Main Menu
- [ ] Continue button: ? Enabled (m�u s�ng)
- [ ] Click Continue
- [ ] Load ?�ng scene ?� save
- [ ] Inventory gi? nguy�n items
- [ ] Player ? ?�ng v? tr� ?� save

### **Test 4: Level End**
- [ ] Ch?m v�o LevelEndTrigger
- [ ] Auto save
- [ ] Load m�n k? ti?p
- [ ] `currentLevel++`

---

## ?? **T�M T?T C�C FILE C?N S?A**

| File | Thay ??i | M?c ?? | 
|------|----------|--------|
| `SaveData.cs` | Th�m 9 fields m?i | ?? **B?t bu?c** |
| `SaveManager.cs` | Th�m try-catch, logging | ?? **Khuy?n ngh?** |
| `SerializableItemStack.cs` | Th�m validation, subfolder search | ?? **Khuy?n ngh?** |
| `GameManager.cs` | Th�m Save/Load logic, tracking | ?? **B?t bu?c** |
| `Health.cs` | ? ?� c� `SetHealth()` method | ? **?� OK** |
| `MainMenuUI.cs` | Th�m button management | ?? **Khuy?n ngh?** |
| `Inventory.cs` | (?� c� s?n methods) | ? **OK** |
| `SavePoint.cs` | T?o m?i | ?? **Optional** |
| `LevelEndTrigger.cs` | T?o m?i | ?? **Optional** |

---

## ?? **TH? T? TH?C HI?N ?? XU?T**

| B??c | Task | Th?i gian |
|------|------|-----------|
| 1?? | **SaveData.cs** - Th�m fields | 3 ph�t |
| 2?? | **SaveManager.cs** - Th�m error handling | 5 ph�t |
| 3?? | **SerializableItemStack.cs** - Th�m validation | 5 ph�t |
| 4?? | **GameManager.cs** - Th�m Save logic | 15 ph�t |
| 5?? | **GameManager.cs** - Th�m Load logic | 10 ph�t ? |
| 6?? | **MainMenuUI.cs** - Button management | 5 ph�t |
| 7?? | **SavePoint.cs** - T?o script m?i | 3 ph�t |
| 8?? | **LevelEndTrigger.cs** - T?o script m?i | 3 ph�t |
| 9?? | **Test** | 15 ph�t ? |

**?? T?ng th?i gian:** ~1 gi? (Gi?m 30 ph�t nh? kh�ng d�ng Reflection!)

---

## ?? **C?I THI?N M?I NH?T**

### **? ?u ?i?m c?a vi?c d�ng `SetHealth()` method:**

| Ti�u ch� | Reflection (C?) | SetHealth() (M?i) |
|----------|-----------------|-------------------|
| **Hi?u n?ng** | ? Ch?m | ? Nhanh |
| **??n gi?n** | ? Ph?c t?p (10+ d�ng) | ? ??n gi?n (1 d�ng) |
| **An to�n** | ?? D? l?i | ? Type-safe |
| **Validation** | ? Kh�ng | ? C� `Mathf.Clamp()` |
| **Auto UI Update** | ? Ph?i code th�m | ? T? ??ng |
| **Maintain** | ? Kh� | ? D? |

### **?? Code tr??c vs sau:**

#### **Tr??c (Reflection - 15 d�ng):**
```csharp
// Ph?c t?p v� d? l?i
var currentHealthField = typeof(Health).GetField("currentHealth", 
    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
var maxHealthField = typeof(Health).GetField("maxHealth", 
    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

if (currentHealthField != null && maxHealthField != null)
{
    currentHealthField.SetValue(health, tempSaveData.playerHealth);
    
    if (health.healthBar != null)
    {
        int maxHealth = (int)maxHealthField.GetValue(health);
        health.healthBar.SetHealth(tempSaveData.playerHealth, maxHealth);
    }
}
```

#### **Sau (SetHealth - 1 d�ng):**
```csharp
// ??n gi?n v� an to�n
health.SetHealth(tempSaveData.playerHealth);
```

---

## ?? **K?T LU?N**

**Ch�c b?n code th�nh c�ng! N?u g?p l?i ? b??c n�o, h�y h?i t�i!** ??

> **?? L?u �:** Nh? c� method `SetHealth()`, vi?c restore player health gi? ?�y ??n gi?n v� an to�n h?n r?t nhi?u!