using UnityEngine;

[DisallowMultipleComponent]
public class LevelGoalTrigger : MonoBehaviour
{
    [SerializeField] private string _requiredTag = "Player";
    [SerializeField] private bool _completeOnlyOnce = true;
    [SerializeField] private GameObject[] _activateOnComplete;
    [SerializeField] private GameObject[] _deactivateOnComplete;

    private bool _hasCompleted;

    private void OnTriggerEnter(Collider other)
    {
        TryComplete(other, "OnTriggerEnter");
    }

    private void TryComplete(Collider other, string eventName)
    {
        if (_completeOnlyOnce && _hasCompleted)
        {
            return;
        }

        if (!string.IsNullOrWhiteSpace(_requiredTag) && !other.CompareTag(_requiredTag))
        {
            Debug.Log($"[LevelGoalTrigger] Ignored {other.name} via {eventName}. Required tag: {_requiredTag}, actual tag: {other.tag}.");
            return;
        }

        Debug.Log($"[LevelGoalTrigger] Goal triggered by {other.name} via {eventName}.");
        _hasCompleted = true;
        SetObjectsActive(_activateOnComplete, true);
        SetObjectsActive(_deactivateOnComplete, false);

        if (LevelRunStats.Instance == null)
        {
            Debug.LogWarning("[LevelGoalTrigger] No LevelRunStats instance found in the scene.");
            return;
        }

        LevelRunStats.Instance.CompleteLevel();
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
