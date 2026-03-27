using UnityEngine;

[DisallowMultipleComponent]
public class BuzzerTrigger : MonoBehaviour
{
    [SerializeField] private string _requiredTag = "Player";
    [SerializeField] private bool _countOnlyOnce = true;
    [SerializeField] private GameObject[] _activateOnTouch;
    [SerializeField] private GameObject[] _deactivateOnTouch;

    private bool _hasBeenTouched;

    private void OnTriggerEnter(Collider other)
    {
        if (_countOnlyOnce && _hasBeenTouched)
        {
            return;
        }

        if (!string.IsNullOrWhiteSpace(_requiredTag) && !other.CompareTag(_requiredTag))
        {
            return;
        }

        _hasBeenTouched = true;
        LevelRunStats.Instance?.RegisterBuzzerTouched();
        SetObjectsActive(_activateOnTouch, true);
        SetObjectsActive(_deactivateOnTouch, false);
    }

    private static void SetObjectsActive(GameObject[] objects, bool isActive)
    {
        if (objects == null)
        {
            return;
        }

        foreach (GameObject target in objects)
        {
            if (target != null)
            {
                target.SetActive(isActive);
            }
        }
    }
}
