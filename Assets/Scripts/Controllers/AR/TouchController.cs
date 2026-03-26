using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class TouchController : MonoBehaviour
{
    [SerializeField] private InputActionReference spawnAction;
    [SerializeField] private Camera _mainCamera;
    private string _selectedItemID = null;

    private void OnEnable() => spawnAction.action.performed += OnTouchPerformed;
    private void OnDisable() => spawnAction.action.performed -= OnTouchPerformed;

    public void SelectItem(string id)
    {
        _selectedItemID = (string.IsNullOrEmpty(id) || _selectedItemID == id) ? null : id;
        Debug.Log($"[Selection] Item set to: {_selectedItemID ?? "None"}");
    }
    private void OnTouchPerformed(InputAction.CallbackContext context)
    {
        if (string.IsNullOrEmpty(_selectedItemID)) return;
        if (EventSystem.current.IsPointerOverGameObject()) return;

        Vector2 touchPosition = Pointer.current.position.ReadValue();
        Ray ray = _mainCamera.ScreenPointToRay(touchPosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            HandleNetworkSpawn(_selectedItemID, hit.point);
        }
    }

    private void HandleNetworkSpawn(string id, Vector3 pos)
    {
        if (NetworkManager.Singleton.IsHost)
        {
            AppBootstrapper.SpawningService.ExecuteSpawnByID(id, pos, Quaternion.identity);
        }
        else
        {
            AppBootstrapper.SpawningBridge.RequestSpawnRpc(id, pos, Quaternion.identity);
        }
    }
}