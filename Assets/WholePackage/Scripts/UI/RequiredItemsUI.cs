using UnityEngine;

public class RequiredItemsUI : MonoBehaviour
{
    [Header("Checkmark References")]
    [Tooltip("Drag checkmark GameObjects here (one for each item)")]
    [SerializeField] private GameObject[] checkmarks;
    
    void Start()
    {
        // Subscribe to item collected event
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnItemCollected += HandleItemCollected;
        }
        
        // Hide all checkmarks at start
        HideAllCheckmarks();
    }
    
    void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnItemCollected -= HandleItemCollected;
        }
    }
    
    void HandleItemCollected(int itemID)
    {
        // Convert itemID to array index (itemID 1 = index 0)
        int index = itemID - 1;
        ShowCheckmark(index);
    }
    
    void ShowCheckmark(int index)
    {
        if (index >= 0 && index < checkmarks.Length && checkmarks[index] != null)
        {
            checkmarks[index].SetActive(true);
            Debug.Log("Checkmark " + index + " shown!");
        }
    }
    
    void HideAllCheckmarks()
    {
        for (int i = 0; i < checkmarks.Length; i++)
        {
            if (checkmarks[i] != null)
            {
                checkmarks[i].SetActive(false);
            }
        }
    }
}