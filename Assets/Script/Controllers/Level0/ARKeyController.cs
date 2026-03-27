using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

public class ARKeyController : MonoBehaviour
{
    [SerializeField] private RevealObjectView revealView;

    private KeyModel _model;

    private void Awake()
    {
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
        if (LevelRunStats.AreInteractionsLocked)
        {
            return;
        }

        var touches = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches;
        if (touches.Count == 0)
        {
            return;
        }

        var touch = touches[0];
        if (touch.phase != UnityEngine.InputSystem.TouchPhase.Began)
        {
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(touch.screenPosition);
        if (!Physics.Raycast(ray, out RaycastHit hit))
        {
            return;
        }

        if (hit.collider.gameObject == gameObject)
        {
            _model.SetTouched();
        }
    }

    private void HandleKeyTouched()
    {
        if (revealView != null)
        {
            revealView.ShowObject();
        }

        LevelRunStats.Instance?.RegisterRedKeyActivated();
    }
}
