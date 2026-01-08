using UnityEngine;
using UnityEngine.UI;

public class WinUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private Button playAgainButton;
    
    void Start()
    {
        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameWon += ShowWinScreen;
        }
        
        if (playAgainButton != null)
        {
            playAgainButton.onClick.AddListener(OnPlayAgainClicked);
        }
    }
    
    void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameWon -= ShowWinScreen;
        }
    }
    
    void ShowWinScreen()
    {
        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }
        
        Time.timeScale = 0f;
    }
    
    void OnPlayAgainClicked()
    {
        Time.timeScale = 1f;
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGame();
        }
    }
}
