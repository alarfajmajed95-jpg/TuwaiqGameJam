using UnityEngine;

public class PlayerCamera5 : MonoBehaviour
{
    public Transform cam;
    public float sensitivity = 100f;
    
    float xRotation = 0f;
    
    public static bool canLook = true;

    void Start()
    {
        LockCursor();
    }

    void Update()
    {
        if (!canLook) return;
        
        float mouseX = Input.GetAxisRaw("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cam.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
    
    public static void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        canLook = true;
        PlayerMovement5.canMove = true;  // Enable movement
    }
    
    public static void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        canLook = false;
        PlayerMovement5.canMove = false;  // Disable movement
    }
}
