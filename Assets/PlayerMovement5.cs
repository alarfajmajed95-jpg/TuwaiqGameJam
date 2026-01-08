using UnityEngine;

public class PlayerMovement5 : MonoBehaviour
{
    [Header("Ground Check")]
    public float sphereOffset = 0.1f;
    public float sphereRadius = 0.3f;
    public LayerMask groundLayer;
    
    [Header("Movement")]
    public float speed = 5f;
    public float sprintSpeed = 8f;
    public float crouchSpeedMultiplier = 0.6f;
    
    [Header("Jump")]
    public float jumpForce = 5f;
    
    [Header("Crouch")]
    public float crouchHeight = 1.2f;
    
    Rigidbody rb;
    CapsuleCollider col;
    float h;
    float v;
    bool isGrounded;
    bool canDoubleJump;
    bool isCrouching;
    float standHeight;
    Vector3 standCenter;
    Vector3 crouchCenter;
    
    // NEW: Control movement
    public static bool canMove = true;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        rb.freezeRotation = true;
        standHeight = col.height;
        standCenter = col.center;
        crouchCenter = new Vector3(0, crouchHeight / 2f, 0);
    }
    
    void Update()
    {
        // Don't allow input if movement disabled
        if (!canMove)
        {
            h = 0;
            v = 0;
            return;
        }
        
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        
        isGrounded = Physics.CheckSphere(
            transform.position + Vector3.up * sphereOffset,
            sphereRadius,
            groundLayer
        );
        
        if (isGrounded)
            canDoubleJump = true;
        
        if (!isCrouching && Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                DoJump();
            }
            else if (canDoubleJump)
            {
                DoJump();
                canDoubleJump = false;
            }
        }
        
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (!isCrouching) StartCrouch();
            else StopCrouch();
        }
    }
    
    void FixedUpdate()
    {
        // Don't move if disabled
        if (!canMove)
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            return;
        }
        
        float currentSpeed = speed;
        if (!isCrouching && Input.GetKey(KeyCode.LeftShift))
            currentSpeed = sprintSpeed;
        if (isCrouching)
            currentSpeed *= crouchSpeedMultiplier;
        
        Vector3 move = (transform.right * h + transform.forward * v) * currentSpeed;
        rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);
    }
    
    void DoJump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
    
    void StartCrouch()
    {
        isCrouching = true;
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y / 2, transform.localScale.z);
    }
    
    void StopCrouch()
    {
        isCrouching = false;
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * 2, transform.localScale.z);
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(
            transform.position + Vector3.up * sphereOffset,
            sphereRadius
        );
    }
}