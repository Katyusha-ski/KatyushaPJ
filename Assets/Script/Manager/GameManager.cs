using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    MainMenu,
    Gameplay,
    Pause,
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameState CurrentGameState { get; private set; }
    public PlayerController player;

    [Header("Game state")]
    public int currentLevel = 1;
    private float playTime = 0f;
    private SaveData tempSaveData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (CurrentGameState == GameState.Gameplay)
        {
            playTime += Time.deltaTime;
        }
    }

    public void PauseGame()
    {
        CurrentGameState = GameState.Pause;
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        CurrentGameState = GameState.Gameplay;
        Time.timeScale = 1f;
    }

    /// <summary>
    /// Save all game state (called from UI or auto save)
    /// </summary>
    public void SaveGame()
    {
        if (Inventory.Instance == null)
        {
            Debug.LogError("Inventory instance was not found!");
            return;
        }

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
        else
        {
            Debug.LogError("Player reference was not found!");
            return;
        }

        SaveData data = new SaveData
        {
            currentLevel = this.currentLevel,
            // Inventory data
            inventoryItem = Inventory.Instance.GetSerializableInventory(),
            equipmentItem = Inventory.Instance.GetSerializableEquipment(),
            // Scene info
            currentSceneIndex = currentScene.buildIndex,
            currentSceneName = currentScene.name,
            // Player state
            playerHealth = playerHealth,
            playerPositionX = playerPosition.x,
            playerPositionY = playerPosition.y,
            playerPositionZ = playerPosition.z,
            // Metadata
            saveDataTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            playTime = this.playTime
        };

        SaveManager.SaveGame(data);
        Debug.Log("Game saved successfully!");
    }

    /// <summary>
    /// Load game and switch to saved scene
    /// </summary>
    public void LoadGame()
    {
        // Load save data
        SaveData saveData = SaveManager.LoadGame();

        // Check if save data is valid
        if (saveData == null)
        {
            Debug.LogError("No valid save data found!");
            return;
        }

        // Load game state
        this.currentLevel = saveData.currentLevel;
        this.playTime = saveData.playTime;

        // Store save data temporarily
        tempSaveData = saveData;

        // Load the saved scene
        if (saveData.currentSceneIndex > 0) // Don't load Main menu
        {
            Debug.Log($"Loading scene: {saveData.currentSceneName} (Index: {saveData.currentSceneIndex})");

            // Register to the sceneLoaded event
            SceneManager.sceneLoaded += OnSceneLoadedAfterLoadGame;

            // Load scene
            SceneManager.LoadScene(saveData.currentSceneIndex);
        }
        else
        {
            Debug.LogError("Invalid scene index in save data!");
        }
    }

    /// <summary>
    /// Start a new game
    /// </summary>
    public void NewGame()
    {
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
        else
        {
            // Fallback: load scene directly
            SceneManager.LoadScene("GrassScene");
        }
    }

    /// <summary>
    /// Check if save file exists
    /// </summary>
    public bool HasSaveFile()
    {
        return SaveManager.HasSaveFile();
    }

    /// <summary>
    /// Callback after scene is loaded - restore inventory and player
    /// </summary>
    private void OnSceneLoadedAfterLoadGame(Scene scene, LoadSceneMode mode)
    {
        // Unsubscribe to prevent multiple calls
        SceneManager.sceneLoaded -= OnSceneLoadedAfterLoadGame;

        if (tempSaveData == null) return;

        // Load inventory after scene is ready
        if (Inventory.Instance != null)
        {
            Inventory.Instance.LoadSerializableInventory(tempSaveData.inventoryItem);
            Inventory.Instance.LoadSerializableEquipment(tempSaveData.equipmentItem);
            Debug.Log("Inventory and equipment restored!");
        }
        else
        {
            Debug.LogWarning("Inventory instance not found after scene load!");
        }

        // Restore player state with delay (ensure player is spawned)
        Invoke(nameof(RestorePlayerState), 0.2f);

        Debug.Log($"Game loaded! Level: {currentLevel}, Play time: {playTime:F1}s");
    }

    /// <summary>
    /// Restore player state from save data
    /// </summary>
    private void RestorePlayerState()
    {
        if (tempSaveData == null) return;

        // Find player in the scene
        if (player == null)
        {
            player = Object.FindFirstObjectByType<PlayerController>();
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

            // Restore health
            Health health = player.GetComponent<Health>();
            if (health != null)
            {
                health.SetHealth(tempSaveData.playerHealth);
            }
            else
            {
                Debug.LogWarning("Health component not found on player!");
            }
        }
        else
        {
            Debug.LogWarning("Player not found in the scene to restore state!");
        }

        tempSaveData = null; 
    }

    /// <summary>
    /// Get save info for UI display
    /// </summary>
    public SaveData GetSaveInfo()
    {
        return SaveManager.LoadGame();
    }
}
