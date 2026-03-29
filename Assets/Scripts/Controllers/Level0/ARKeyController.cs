using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

public class ARKeyController : MonoBehaviour
{
    [SerializeField] private RevealObjectView revealView;

    private KeyModel _model;

    private void Awake()
    {
        Debug.Log("[ARKey] Awake : Clé initialisée, en attente de touché");
        _model = new KeyModel();
        EnhancedTouchSupport.Enable();
    }

    private void OnEnable()
    {
        _model.OnKeyTouched += HandleKeyTouched;
    }

    private void OnDisable()
    {
        _model.OnKeyTouched -= HandleKeyTouched;
    }

    private void Update()
    {
        var touches = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches;

        if (touches.Count == 0) return;

        var touch = touches[0];
        if (touch.phase != UnityEngine.InputSystem.TouchPhase.Began) return;

        Debug.Log("[ARKey] Touch détecté à : " + touch.screenPosition);

        Ray ray = Camera.main.ScreenPointToRay(touch.screenPosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.Log("[ARKey] Raycast touche : " + hit.collider.gameObject.name);

            if (hit.collider.gameObject == gameObject)
            {
                Debug.Log("[ARKey] ✓ Clé touchée !");
                _model.SetTouched();
            }
        }
        else
        {
            Debug.Log("[ARKey] ✗ Raycast ne touche rien");
        }
    }

    private void HandleKeyTouched()
    {
        revealView.ShowObject();
    }
}