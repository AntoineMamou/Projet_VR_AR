using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using Unity.Netcode;

/// <summary>
/// MVC — Controller
/// Gère l'état du KeyCube : zone, couleur, déplacement AR et verrouillage grab.
/// À placer sur le GameObject KeyCube avec KeyCubeView et XRGrabInteractable.
/// </summary>
[RequireComponent(typeof(KeyCubeView))]
[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(Rigidbody))]
public class KeyCubeController : NetworkBehaviour
{
    [Header("Déplacement AR")]
    [SerializeField] private float _moveSpeed = 3f;

    private KeyCubeModel _model;
    private KeyCubeView _view;
    private XRGrabInteractable _grabInteractable;
    private Rigidbody _rb;

    private float _lockedZForVR;
    private float _lockedYForVR;

    private float _arMoveDirection = 0f;
    private float _lastSentDirection = -999f; // Filtre anti-spam

    private void Awake()
    {
        _model = new KeyCubeModel();
        _view = GetComponent<KeyCubeView>();
        _grabInteractable = GetComponent<XRGrabInteractable>();
        _rb = GetComponent<Rigidbody>();

        _model.OnStateChanged += OnModelStateChanged;
        _model.OnARControllingChanged += OnARControllingChanged;
        _model.OnVRGrabChanged += OnVRGrabChanged;

        _grabInteractable.selectEntered.AddListener(OnGrabbed);
        _grabInteractable.selectExited.AddListener(OnReleased);
    }

    private void OnDestroy()
    {
        _model.OnStateChanged -= OnModelStateChanged;
        _model.OnARControllingChanged -= OnARControllingChanged;
        _model.OnVRGrabChanged -= OnVRGrabChanged;

        _grabInteractable.selectEntered.RemoveListener(OnGrabbed);
        _grabInteractable.selectExited.RemoveListener(OnReleased);
    }

    private void LateUpdate()
    {
        if (_model.IsGrabbedByVR)
        {
            transform.position = new Vector3(transform.position.x, _lockedYForVR, _lockedZForVR);
            transform.rotation = Quaternion.identity;
        }
    }

    private void FixedUpdate()
    {
        if (IsServer && !_model.IsGrabbedByVR)
        {
            if (_arMoveDirection != 0f)
            {
                Vector3 delta = Vector3.forward * _arMoveDirection * _moveSpeed * Time.fixedDeltaTime;
                _rb.MovePosition(_rb.position + delta);
            }
            else
            {
                _rb.linearVelocity = Vector3.zero;
                _rb.angularVelocity = Vector3.zero;
            }
        }
    }

    public void EnterZone() => _model.IsInZone = true;
    public void ExitZone() => _model.IsInZone = false;

    public void MoveAlongZ(float direction)
    {
        if (direction == _lastSentDirection) return;
        _lastSentDirection = direction;

        if (IsServer)
        {
            _arMoveDirection = direction;
        }
        else
        {
            MoveAlongZServerRpc(direction);
        }
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void MoveAlongZServerRpc(float direction)
    {
        _arMoveDirection = direction;
    }

    public void SetARControlling(bool isControlling)
    {
        if (IsServer)
        {
            _model.IsARControlling = isControlling;
        }
        else
        {
            SetARControllingServerRpc(isControlling);
        }
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void SetARControllingServerRpc(bool isControlling)
    {
        _model.IsARControlling = isControlling;
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        _model.IsGrabbedByVR = true;

        _lockedZForVR = transform.position.z;
        _lockedYForVR = transform.position.y;
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        _model.IsGrabbedByVR = false;

        _view.ApplyZConstraint(_lockedZForVR);
    }

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