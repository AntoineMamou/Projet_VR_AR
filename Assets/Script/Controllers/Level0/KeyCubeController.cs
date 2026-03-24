using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

/// <summary>
/// MVC — Controller
/// Gère l'état du KeyCube : zone, couleur, déplacement AR et verrouillage grab.
/// À placer sur le GameObject KeyCube avec KeyCubeView et XRGrabInteractable.
/// </summary>
[RequireComponent(typeof(KeyCubeView))]
[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(Rigidbody))]
public class KeyCubeController : MonoBehaviour
{
    // ── Sérialisation ──────────────────────────────────────────────────────
    [Header("Déplacement AR")]
    [SerializeField] private float _moveSpeed = 3f;

    // ── Références ─────────────────────────────────────────────────────────
    private KeyCubeModel       _model;
    private KeyCubeView        _view;
    private XRGrabInteractable _grabInteractable;
    private Rigidbody          _rb;

    // ── Cycle Unity ────────────────────────────────────────────────────────
    private void Awake()
    {
        _model            = new KeyCubeModel();
        _view             = GetComponent<KeyCubeView>();
        _grabInteractable = GetComponent<XRGrabInteractable>();
        _rb               = GetComponent<Rigidbody>();

        _model.OnStateChanged         += OnModelStateChanged;
        _model.OnARControllingChanged += OnARControllingChanged;
        _model.OnVRGrabChanged        += OnVRGrabChanged;

        _grabInteractable.selectEntered.AddListener(OnGrabbed);
        _grabInteractable.selectExited.AddListener(OnReleased);
    }

    private void OnDestroy()
    {
        _model.OnStateChanged         -= OnModelStateChanged;
        _model.OnARControllingChanged -= OnARControllingChanged;
        _model.OnVRGrabChanged        -= OnVRGrabChanged;

        _grabInteractable.selectEntered.RemoveListener(OnGrabbed);
        _grabInteractable.selectExited.RemoveListener(OnReleased);
    }

    private void LateUpdate()
    {
        if (_model.IsGrabbedByVR)
            _view.ApplyZConstraint(_model.LockedZ);
    }

    // ── API publique — Zone ────────────────────────────────────────────────
    public void EnterZone() => _model.IsInZone = true;
    public void ExitZone()  => _model.IsInZone = false;

    // ── API publique — AR ──────────────────────────────────────────────────
    /// <summary>
    /// Appelé chaque Update par ARInputController tant que le joueur AR appuie.
    /// direction : -1 = -Z, +1 = +Z
    /// </summary>
    public void MoveAlongZ(float direction)
    {
        Debug.Log($"[KeyCube] MoveAlongZ appelé, direction={direction}, rb={_rb != null}");
        Vector3 delta  = Vector3.forward * direction * _moveSpeed * Time.deltaTime;
        Vector3 target = _rb.position + delta;
        _rb.MovePosition(target);
    }

    /// <summary>
    /// Active ou désactive le grab XRI selon que le joueur AR contrôle le cube.
    /// </summary>
    public void SetARControlling(bool isControlling)
    {
        _model.IsARControlling = isControlling;
    }

    // ── Événements XR ─────────────────────────────────────────────────────
    private void OnGrabbed(SelectEnterEventArgs args)
    {
        _model.SetLockedZ(transform.position.z);
        _model.IsGrabbedByVR = true;
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        _model.IsGrabbedByVR = false;
        _view.ApplyZConstraint(_model.LockedZ);
    }

    // ── Réactions au Model ─────────────────────────────────────────────────
    private void OnModelStateChanged(bool isInZone)
    {
        _view.SetInZoneState(isInZone);
    }

    private void OnARControllingChanged(bool isControlling)
    {
        _grabInteractable.enabled = !isControlling;
    }

    private void OnVRGrabChanged(bool isGrabbed) { }
}