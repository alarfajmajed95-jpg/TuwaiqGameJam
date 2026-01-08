using UnityEngine;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private Transform mainCamera;
    [SerializeField] private float interactionRayDistance;
    [SerializeField] private LayerMask interacitonLayer;
    [SerializeField] private GameObject crosshairGameObjectUI;
    [SerializeField] private Sprite redCircleCrosshair;
    [SerializeField] private Sprite handCrosshair;

    private PlayerInventory _playerInventory;
    private RectTransform _crosshairRectUI;
    private Image _crosshairImageUI;
    private IInteractable _currentTarget;

    public static bool IsPlayerHidden { get; set; }
    public static bool IsPlayerReading { get; set; }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(mainCamera.position, mainCamera.forward * interactionRayDistance);
    }

    private void Awake()
    {
        _playerInventory = GetComponent<PlayerInventory>();
        _crosshairImageUI = crosshairGameObjectUI.GetComponent<Image>();
        _crosshairRectUI = crosshairGameObjectUI.GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (Physics.Raycast(mainCamera.position, mainCamera.forward, out RaycastHit hitInfo, interactionRayDistance, interacitonLayer))
        {
            if (hitInfo.collider.TryGetComponent(out IInteractable interactableObject))
            {
                if (_currentTarget != interactableObject)
                {
                    _currentTarget = interactableObject;

                    _crosshairRectUI.sizeDelta = new Vector2(60f, 60f);
                    _crosshairImageUI.sprite = handCrosshair;
                }

                if (Input.GetButtonDown("Fire1"))
                {
                    interactableObject.Interact(_playerInventory);
                }
            }
            else
            {
                _currentTarget = null;

                _crosshairRectUI.sizeDelta = new Vector2(10f, 10f);
                _crosshairImageUI.sprite = redCircleCrosshair;
            }
        }
        else
        {
            _currentTarget = null;

            _crosshairRectUI.sizeDelta = new Vector2(10f, 10f);
            _crosshairImageUI.sprite = redCircleCrosshair;
        }
    }
}
