using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    // Events
    public event Action<int> OnItemCollected;
    public event Action OnAllItemsCollected;
    public event Action OnGameWon;
    public event Action OnGameLost;  // NEW: Loss event
    
    [Header("Game Settings")]
    public int totalItemsRequired = 4;
    public int maxMistakes = 2;  // NEW: Maximum allowed mistakes
    
    // Item states
    public bool hasItem1 = false;
    public bool hasItem2 = false;
    public bool hasItem3 = false;
    public bool hasItem4 = false;
    
    private int itemsCollected = 0;
    private int mistakeCount = 0;  // NEW: Track mistakes
    private bool allItemsCollected = false;
    private bool gameWon = false;
    private bool gameLost = false;  // NEW: Track loss state
    
    public bool AllItemsCollected => allItemsCollected;
    public bool GameWon => gameWon;
    public bool GameLost => gameLost;
    public int MistakeCount => mistakeCount;
    
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    public void AddItem(int id)
    {
        if (gameLost) return;  // Don't add items if game lost
        
        if (id == 1) hasItem1 = true;
        if (id == 2) hasItem2 = true;
        if (id == 3) hasItem3 = true;
        if (id == 4) hasItem4 = true;
        
        itemsCollected++;
        
        Debug.Log("Collected item: " + id);
        
        if (OnItemCollected != null)
        {
            OnItemCollected.Invoke(id);
        }
        
        CheckAllItems();
    }
    
    // NEW: Add mistake
    public void AddMistake()
    {
        if (gameLost || gameWon) return;
        
        mistakeCount++;
        Debug.Log("Mistake! Total mistakes: " + mistakeCount + "/" + maxMistakes);
        
        if (mistakeCount >= maxMistakes)
        {
            TriggerLoss();
        }
    }
    
    void CheckAllItems()
    {
        if (hasItem1 && hasItem2 && hasItem3 && hasItem4)
        {
            allItemsCollected = true;
            Debug.Log("All items collected!");
            
            if (OnAllItemsCollected != null)
            {
                OnAllItemsCollected.Invoke();
            }
        }
    }
    
    public bool CanUnlock()
    {
        return hasItem1 && hasItem2 && hasItem3 && hasItem4;
    }
    
    public void TriggerWin()
    {
        if (!allItemsCollected || gameLost) return;
        if (gameWon) return;
        
        gameWon = true;
        Debug.Log("YOU WIN!");
        
        if (OnGameWon != null)
        {
            OnGameWon.Invoke();
        }
    }
    
    // NEW: Trigger loss
    public void TriggerLoss()
    {
        if (gameWon) return;
        if (gameLost) return;
        
        gameLost = true;
        Debug.Log("GAME OVER! Too many mistakes!");
        
        if (OnGameLost != null)
        {
            OnGameLost.Invoke();
        }
    }
    
    public void RestartGame()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        );
    }
}