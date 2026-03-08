using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
public class ConstrainedGrab : MonoBehaviour
{
    public enum Axis { X, Y, Z }

    private Vector3 _lockedPosition;
    private Axis _currentAxis;
    private bool _isGrabbed;

    private XRGrabInteractable _grab;

    private void Awake()
    {
        _grab = GetComponent<XRGrabInteractable>();
        _grab.selectEntered.AddListener(OnSelectEntered);
        _grab.selectExited.AddListener(OnSelectExited);
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        var vrPlayer = args.interactorObject.transform
                           .GetComponentInParent<VRPlayerController>();
        var arPlayer = args.interactorObject.transform
                           .GetComponentInParent<ARPlayerController>();

        if (vrPlayer != null)
            _currentAxis = vrPlayer.AllowedAxis;
        else if (arPlayer != null)
            _currentAxis = arPlayer.AllowedAxis;

        _lockedPosition = transform.position;
        _isGrabbed = true;
    }

    private void OnSelectExited(SelectExitEventArgs args) => _isGrabbed = false;

    private void FixedUpdate()
    {
        if (!_isGrabbed) return;

        var pos = transform.position;

        switch (_currentAxis)
        {
            case Axis.X:
                pos.y = _lockedPosition.y;
                pos.z = _lockedPosition.z;
                break;
            case Axis.Z:
                pos.x = _lockedPosition.x;
                pos.y = _lockedPosition.y;
                break;
            case Axis.Y:
                pos.x = _lockedPosition.x;
                pos.z = _lockedPosition.z;
                break;
        }

        transform.position = pos;
    }
}