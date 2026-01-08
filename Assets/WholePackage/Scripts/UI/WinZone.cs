using UnityEngine;
using System.Collections;

public class WinZone : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float flyHeight = 10f;
    [SerializeField] private float flySpeed = 3f;
    
    [Header("Sphere Collider Settings")]
    [SerializeField] private float triggerRadius = 3f;
    
    private bool isUnlocked = false;
    private bool hasWon = false;
    private Vector3 startPosition;
    private SphereCollider sphereCollider;
    
    void Start()
    {
        startPosition = transform.position;
        
        // Setup Sphere Collider
        SetupSphereCollider();
        
        // Subscribe to event
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnAllItemsCollected += Unlock;
        }
    }
    
    void SetupSphereCollider()
    {
        // Get or add Sphere Collider
        sphereCollider = GetComponent<SphereCollider>();
        
        if (sphereCollider == null)
        {
            sphereCollider = gameObject.AddComponent<SphereCollider>();
        }
        
        sphereCollider.isTrigger = true;
        sphereCollider.radius = triggerRadius;
        
        // Add Rigidbody if needed (for trigger to work)
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.isKinematic = true;
        rb.useGravity = false;
    }
    
    void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnAllItemsCollected -= Unlock;
        }
    }
    
    void Unlock()
    {
        isUnlocked = true;
        Debug.Log("Flag unlocked! Walk to it to win!");
    }
    
    void OnTriggerEnter(Collider other)
    {
        // Check if player entered
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered flag zone!");
            TryWin();
        }
    }
    
    void TryWin()
    {
        if (!isUnlocked)
        {
            Debug.Log("Collect all items first!");
            return;
        }
        
        if (hasWon) return;
        
        hasWon = true;
        Debug.Log("You win! Flag flying up!");
        
        StartCoroutine(FlyUpAnimation());
    }
    
    IEnumerator FlyUpAnimation()
    {
        Vector3 targetPosition = startPosition + Vector3.up * flyHeight;
        
        // Fly up
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                flySpeed * Time.deltaTime
            );
            yield return null;
        }
        
        transform.position = targetPosition;
        
        yield return new WaitForSeconds(0.5f);
        
        // Trigger win
        if (GameManager.Instance != null)
        {
            GameManager.Instance.TriggerWin();
        }
    }
    
    // Show sphere in editor
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, triggerRadius);
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.3f);
        Gizmos.DrawSphere(transform.position, triggerRadius);
    }
}