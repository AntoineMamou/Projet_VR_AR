using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class TouchController : MonoBehaviour
{
    [SerializeField] private InputActionReference spawnAction;

    private void OnEnable() => spawnAction.action.performed += OnTouchPerformed;
    private void OnDisable() => spawnAction.action.performed -= OnTouchPerformed;

    private string _selectedItemID = null;

    public void SelectItem(string id)
    {
        _selectedItemID = (_selectedItemID == id) ? null : id;
        Debug.Log($"[Selection] Objet actuel : {_selectedItemID ?? "Aucun"}");
    }
    private void OnTouchPerformed(InputAction.CallbackContext context)
    {
        if (!string.IsNullOrEmpty(_selectedItemID))
        {
            HandleSpawn(context, ItemIDs.TestCube);
        }
    }

    [SerializeField] private Camera _mainCamera;

    public void HandleSpawn(InputAction.CallbackContext context, string itemID = ItemIDs.TestCube)
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        if (context.performed)
        {
            Vector2 touchPosition = Pointer.current.position.ReadValue();
            Ray ray = _mainCamera.ScreenPointToRay(touchPosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                AppBootstrapper.PlacableService.SpawnObject(itemID, hit.point, Quaternion.identity);
            }
            else
            {
                Debug.Log($"[TouchController] Pas de sol où spawner l'objet à la position {touchPosition}.");
            }


        }
    }
}