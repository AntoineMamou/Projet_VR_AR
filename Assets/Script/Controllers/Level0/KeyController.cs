using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class KeyController : MonoBehaviour
{
    [SerializeField] private RevealObjectView revealView;
    [SerializeField] private bool _completeLevelOnGrab;

    private KeyModel _model;
    private XRGrabInteractable _grabInteractable;

    private void Awake()
    {
        _model = new KeyModel();
        _grabInteractable = GetComponent<XRGrabInteractable>();
    }

    private void OnEnable()
    {
        _grabInteractable.selectEntered.AddListener(OnKeyGrabbed);
        _model.OnKeyGrabbed += HandleKeyGrabbed;
    }

    private void OnDisable()
    {
        _grabInteractable.selectEntered.RemoveListener(OnKeyGrabbed);
        _model.OnKeyGrabbed -= HandleKeyGrabbed;
    }

    private void OnKeyGrabbed(SelectEnterEventArgs args)
    {
        if (LevelRunStats.AreInteractionsLocked)
        {
            return;
        }

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
            LevelRunStats.Instance?.RegisterGreenKeyActivatedAndComplete();
        }
    }
}
