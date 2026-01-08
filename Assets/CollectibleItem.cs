using UnityEngine;
using TMPro;

public class CollectibleItem : MonoBehaviour
{
    [Header("Item Settings")]
    public int itemID;
    public float pickupRange = 2f;
    
    [Header("Question Settings")]
    [SerializeField] private QuestionData itemQuestion;
    [SerializeField] private TMP_FontAsset fontAsset;
    
    [Header("Audio Settings")]
    [Tooltip("Audio clip to read the question")]
    [SerializeField] private AudioClip questionAudio;
    
    [Header("UI Reference")]
    [SerializeField] private GameObject interactionPrompt;
    
    private Transform player;
    private bool isCollected = false;
    private bool isPlayerInRange = false;
    
    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
    }
    
    void Update()
    {
        if (isCollected || player == null) return;
        
        float distance = Vector3.Distance(transform.position, player.position);
        
        if (distance <= pickupRange)
        {
            if (!isPlayerInRange)
            {
                isPlayerInRange = true;
                ShowPrompt();
            }
            
            if (Input.GetKeyDown(KeyCode.E))
            {
                TryInteract();
            }
        }
        else
        {
            if (isPlayerInRange)
            {
                isPlayerInRange = false;
                HidePrompt();
            }
        }
    }
    
    void ShowPrompt()
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(true);
        }
    }
    
    void HidePrompt()
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
    }
    
    void TryInteract()
    {
        if (isCollected) return;
        
        if (itemQuestion == null)
        {
            CollectItem();
            return;
        }
        
        if (QuestionUI.Instance != null)
        {
            // Pass all 4 parameters: question, callback, font, audio
            QuestionUI.Instance.ShowQuestion(itemQuestion, OnQuestionAnswered, fontAsset, questionAudio);
        }
        else
        {
            CollectItem();
        }
    }
    
    void OnQuestionAnswered(bool isCorrect)
    {
        if (isCorrect)
        {
            CollectItem();
        }
        else
        {
            Debug.Log("Wrong answer! Try again.");
        }
    }
    
    void CollectItem()
    {
        isCollected = true;
        
        HidePrompt();
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddItem(itemID);
        }
        
        gameObject.SetActive(false);
        
        Debug.Log("Item " + itemID + " collected!");
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
} 