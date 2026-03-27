using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class TouchController : MonoBehaviour
{
    [SerializeField] private InputActionReference spawnAction;
    [SerializeField] private Camera _mainCamera;

    private string _selectedItemID;
    private InputAction _spawnInputAction;

    private void Awake()
    {
        ResolveMainCamera();
    }

    private void OnEnable()
    {
        if (spawnAction == null || spawnAction.action == null)
        {
            Debug.LogWarning("[TouchController] Spawn action reference is missing.", this);
            return;
        }

        _spawnInputAction = spawnAction.action;
        _spawnInputAction.Enable();
        _spawnInputAction.performed += OnTouchPerformed;
        ResolveMainCamera();
    }

    private void OnDisable()
    {
        if (_spawnInputAction == null)
        {
            return;
        }

        _spawnInputAction.performed -= OnTouchPerformed;
        _spawnInputAction.Disable();
        _spawnInputAction = null;
    }

    public void SelectItem(string id)
    {
        _selectedItemID = _selectedItemID == id ? null : id;
        Debug.Log($"[Selection] Objet actuel : {_selectedItemID ?? "Aucun"}");
    }

    private void OnTouchPerformed(InputAction.CallbackContext context)
    {
        if (!string.IsNullOrEmpty(_selectedItemID))
        {
            HandleSpawn(context, ItemIDs.TestCube);
        }
    }

    public void HandleSpawn(InputAction.CallbackContext context, string itemID = ItemIDs.TestCube)
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (!context.performed)
        {
            return;
        }

        ResolveMainCamera();

        if (_mainCamera == null)
        {
            Debug.LogWarning("[TouchController] No active camera found for spawning.", this);
            return;
        }

        if (!TryGetPointerPosition(out Vector2 pointerPosition))
        {
            Debug.LogWarning("[TouchController] No pointer position is currently available.", this);
            return;
        }

        Ray ray = _mainCamera.ScreenPointToRay(pointerPosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GameObject spawnedObject = AppBootstrapper.PlacableService.SpawnObject(itemID, hit.point, Quaternion.identity);

            if (spawnedObject != null)
            {
                LevelRunStats.Instance?.RegisterCubeUsed();
            }

            return;
        }

        Debug.Log($"[TouchController] Pas de sol pour spawner l'objet a la position {pointerPosition}.");
    }

    private void ResolveMainCamera()
    {
        if (_mainCamera != null && _mainCamera.isActiveAndEnabled && _mainCamera.gameObject.activeInHierarchy)
        {
            return;
        }

        if (Camera.main != null && Camera.main.isActiveAndEnabled)
        {
            _mainCamera = Camera.main;
            return;
        }

        Camera[] cameras = FindObjectsByType<Camera>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (Camera camera in cameras)
        {
            if (camera.isActiveAndEnabled)
            {
                _mainCamera = camera;
                return;
            }
        }

        _mainCamera = null;
    }

    private static bool TryGetPointerPosition(out Vector2 pointerPosition)
    {
        if (Pointer.current != null)
        {
            pointerPosition = Pointer.current.position.ReadValue();
            return true;
        }

        if (Mouse.current != null)
        {
            pointerPosition = Mouse.current.position.ReadValue();
            return true;
        }

        if (Touchscreen.current != null)
        {
            pointerPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            return true;
        }

        pointerPosition = default;
        return false;
    }
}
