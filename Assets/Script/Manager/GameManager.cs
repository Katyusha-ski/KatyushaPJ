using UnityEngine;

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
    // Add other game state variables as needed

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
    /// Sava all game(called from UI or auto save)
    /// </summary>
    public void SaveGame()
    {
        if (Inventory.Instance == null)
        {
            Debug.LogError("Inventory instance was not found!");
            return;
        }
        SavaData data = new SavaData 
        {
            currentLevel = this.currentLevel,
            inventoryItem = Inventory.Instance.GetSerializableInventory(),
        };

        SaveManager.SaveGame(data);
        Debug.Log("Game saved successfully!");
    }

    /// <summary>
    /// Load all game(called from Main Menu)
    /// </summary>
   public void LoadGame()
    {
        if (Inventory.Instance == null)
        {
            Debug.LogError("Inventory instance was not found!");
            return;
        }
        
        SavaData saveData = SaveManager.LoadGame();

        if(saveData != null)
        {
            Debug.Log("No save data found to load.");
        }
        
        this.currentLevel = saveData.currentLevel;
        Inventory.Instance.LoadSerializableInventory(saveData.inventoryItem);
        Debug.Log($"Game loaded! Current level: {currentLevel}");
   }
    /// <summary>
    /// Start a new game
    /// </summary>
    public void NewGame()
    {
        SaveManager.DeleteSave();

        // Reset game state
        currentLevel = 1;

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
    }

    /// <summary>
    /// Check if save file exists
    /// </summary>
    public bool HasSaveFile()
    {
        return SaveManager.HasSaveFile();
    }
}
