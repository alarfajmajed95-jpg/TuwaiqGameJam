using UnityEngine;
using UnityEngine.UI;

public class LossUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject lossPanel;
    [SerializeField] private Button tryAgainButton;
    
    void Start()
    {
        // Hide panel at start
        if (lossPanel != null)
        {
            lossPanel.SetActive(false);
        }
        
        // Subscribe to loss event
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameLost += ShowLossScreen;
        }
        
        // Setup button
        if (tryAgainButton != null)
        {
            tryAgainButton.onClick.AddListener(OnTryAgainClicked);
        }
    }
    
    void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameLost -= ShowLossScreen;
        }
    }
    
    void ShowLossScreen()
    {
        if (lossPanel != null)
        {
            lossPanel.SetActive(true);
        }
        
        // Show cursor
        PlayerCamera5.UnlockCursor();
        
        Time.timeScale = 0f;
        
        Debug.Log("Loss screen shown!");
    }
    
    void OnTryAgainClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGame();
        }
    }
}