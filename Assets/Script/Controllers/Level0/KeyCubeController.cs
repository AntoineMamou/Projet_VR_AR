using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(KeyCubeView))]
[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(Rigidbody))]
public class KeyCubeController : MonoBehaviour
{
    [Header("Deplacement AR")]
    [SerializeField] private float _moveSpeed = 3f;

    private KeyCubeModel _model;
    private KeyCubeView _view;
    private XRGrabInteractable _grabInteractable;
    private Rigidbody _rb;

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
            _view.ApplyZConstraint(_model.LockedZ);
        }
    }

    public void EnterZone()
    {
        _model.IsInZone = true;
    }

    public void ExitZone()
    {
        _model.IsInZone = false;
    }

    public void MoveAlongZ(float direction)
    {
        if (LevelRunStats.AreInteractionsLocked)
        {
            return;
        }

        Vector3 delta = Vector3.forward * direction * _moveSpeed * Time.deltaTime;
        Vector3 target = _rb.position + delta;
        _rb.MovePosition(target);
    }

    public void SetARControlling(bool isControlling)
    {
        if (LevelRunStats.AreInteractionsLocked)
        {
            isControlling = false;
        }

        _model.IsARControlling = isControlling;
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        if (LevelRunStats.AreInteractionsLocked)
        {
            return;
        }

        _model.SetLockedZ(transform.position.z);
        _model.IsGrabbedByVR = true;
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        _model.IsGrabbedByVR = false;
        _view.ApplyZConstraint(_model.LockedZ);
    }

    private void OnModelStateChanged(bool isInZone)
    {
        _view.SetInZoneState(isInZone);
    }

    private void OnARControllingChanged(bool isControlling)
    {
        _grabInteractable.enabled = !isControlling;
    }

    private void OnVRGrabChanged(bool isGrabbed)
    {
    }
}
