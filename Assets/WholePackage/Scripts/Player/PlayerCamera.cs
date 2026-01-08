using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Transform mainCamera;
    [SerializeField] private float sensitivity;

    private float _mouseX;
    private float _mouseY;
    private float _xRotation;

    public static bool IsCameraInputOn { get; set; }

    private void Awake()
    {
        IsCameraInputOn = true;
    }

    private void LateUpdate()
    {
        if (IsCameraInputOn)
        {
            // Get Mouse X input and multiply by sensitivity
            // Get Mouse Y input and multiply by sensitivity
            _mouseX = Input.GetAxis("Mouse X") * sensitivity;
            _mouseY = Input.GetAxis("Mouse Y") * sensitivity;

            // xRotation minus mouseY to rotate around X axis,
            //  and clamp the value of xRotation min=-90, max=90 to limit the camera from rotating vertically more than these values
            _xRotation -= _mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

            // Apply the xRotation to the camera, for vertical only
            mainCamera.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);

            // Rotate the player horizontally, and because the camera is a child of player then it will rotate with it horizontally
            transform.Rotate(Vector3.up * _mouseX);
        }
    }
}
