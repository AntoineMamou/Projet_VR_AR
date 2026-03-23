using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class TouchController : MonoBehaviour
{
    [SerializeField] private InputActionReference spawnAction;
    [SerializeField] private Camera _mainCamera;

    private string _selectedItemID;

    private void OnEnable()
    {
        spawnAction.action.performed += OnTouchPerformed;
    }

    private void OnDisable()
    {
        spawnAction.action.performed -= OnTouchPerformed;
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

        if (!context.performed || _mainCamera == null)
        {
            return;
        }

        Vector2 touchPosition = Pointer.current.position.ReadValue();
        Ray ray = _mainCamera.ScreenPointToRay(touchPosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GameObject spawnedObject = AppBootstrapper.PlacableService.SpawnObject(itemID, hit.point, Quaternion.identity);

            if (spawnedObject != null)
            {
                LevelRunStats.Instance?.RegisterCubeUsed();
            }

            return;
        }

        Debug.Log($"[TouchController] Pas de sol pour spawner l'objet a la position {touchPosition}.");
    }
}
