using UnityEngine;

/// <summary>
/// MVC — Controller (AR)
/// Reçoit la direction depuis ARInputView, met à jour ARInputModel,
/// puis pilote le KeyCubeController (déplacement + verrouillage du grab).
/// À placer sur le même GameObject que ARInputView.
/// </summary>
[RequireComponent(typeof(ARInputView))]
public class ARInputController : MonoBehaviour
{
    // ── Sérialisation ──────────────────────────────────────────────────────
    [Header("Cible")]
    [SerializeField] private KeyCubeController _keyCubeController;

    // ── Références ─────────────────────────────────────────────────────────
    private ARInputModel _model;
    private ARInputView  _view;

    // ── Cycle Unity ────────────────────────────────────────────────────────
    private void Awake()
    {
        _model = new ARInputModel();
        _view  = GetComponent<ARInputView>();

        Debug.Log($"[ARInput] Awake — _view={_view != null}, _keyCubeController={_keyCubeController != null}");

        _view.OnDirectionChanged      += OnDirectionChanged;
        _model.OnControllingChanged   += OnControllingChanged;
    }

    private void OnDestroy()
    {
        _view.OnDirectionChanged    -= OnDirectionChanged;
        _model.OnControllingChanged -= OnControllingChanged;
    }

    private void Update()
    {
        Debug.Log($"[ARInput] Update — IsControlling={_model.IsControlling}, Direction={_model.Direction}");
        if (!_model.IsControlling) return;
        _keyCubeController.MoveAlongZ(_model.Direction);
    }

    // ── Réactions ──────────────────────────────────────────────────────────
    private void OnDirectionChanged(float direction)
    {
        Debug.Log($"[ARInput] OnDirectionChanged reçu — direction={direction}");
        _model.Direction = direction;
    }

    private void OnControllingChanged(bool isControlling)
    {
        _keyCubeController.SetARControlling(isControlling);
    }
}