using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[DisallowMultipleComponent]
public class KeyController : MonoBehaviour
{
    [Header("Optional Visual Feedback")]
    [SerializeField] private RevealObjectView revealView;

    [Header("Victory")]
    [SerializeField] private bool _completeLevelOnGrab = true;

    private KeyModel _model;
    private XRGrabInteractable _grabInteractable;

    private void Awake()
    {
        EnsureInitialized();
    }

    private void EnsureInitialized()
    {
        _model ??= new KeyModel();
        _grabInteractable ??= GetComponent<XRGrabInteractable>();

        if (_grabInteractable == null)
        {
            Debug.LogError("[KeyController] XRGrabInteractable is missing on the key object.", this);
        }
    }

    private void OnEnable()
    {
        EnsureInitialized();

        if (_grabInteractable != null)
        {
            _grabInteractable.selectEntered.AddListener(OnKeyGrabbed);
        }

        _model.OnKeyGrabbed += HandleKeyGrabbed;
    }

    private void OnDisable()
    {
        if (_grabInteractable != null)
        {
            _grabInteractable.selectEntered.RemoveListener(OnKeyGrabbed);
        }

        _model.OnKeyGrabbed -= HandleKeyGrabbed;
    }

    private void OnKeyGrabbed(SelectEnterEventArgs args)
    {
        Debug.Log($"[KeyController] Key grabbed by {args.interactorObject?.transform?.name ?? "unknown interactor"}.");
        _model.SetGrabbed();
    }

    private void HandleKeyGrabbed()
    {
        if (revealView != null)
        {
            revealView.ShowObject();
        }

        if (_completeLevelOnGrab)
        {
            if (LevelRunStats.Instance == null)
            {
                Debug.LogWarning("[KeyController] No LevelRunStats instance found. Key grab will not complete the level.");
                return;
            }

            LevelRunStats.Instance.CompleteLevel();
        }
    }

    [ContextMenu("Debug/Simulate Key Grab")]
    public void DebugSimulateKeyGrab()
    {
        EnsureInitialized();
        Debug.Log("[KeyController] Debug simulate key grab.");
        _model.SetGrabbed();
    }
}
