using UnityEngine;

[DisallowMultipleComponent]
[DefaultExecutionOrder(1000)]
public class PlayerTriggerFollower : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private Vector3 _worldOffset = new(0f, -0.5f, 0f);
    [SerializeField] private bool _matchTargetYaw;

    private void Awake()
    {
        SnapToTarget();
    }

    private void LateUpdate()
    {
        SnapToTarget();
    }

    private void SnapToTarget()
    {
        ResolveTarget();
        if (_target == null)
        {
            return;
        }

        transform.position = _target.position + _worldOffset;

        float yaw = _matchTargetYaw ? _target.eulerAngles.y : 0f;
        transform.rotation = Quaternion.Euler(0f, yaw, 0f);
    }

    private void ResolveTarget()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            Transform mainCameraTransform = mainCamera.transform;
            if (_target != mainCameraTransform)
            {
                _target = mainCameraTransform;
            }

            return;
        }

        if (_target != null && !_target.gameObject.activeInHierarchy)
        {
            _target = null;
        }
    }
}
