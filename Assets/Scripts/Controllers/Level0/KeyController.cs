using UnityEngine ;
using UnityEngine.XR.Interaction.Toolkit;
public class KeyController : MonoBehaviour
{
    [SerializeField] private RevealObjectView revealView;

    private KeyModel _model;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable _grabInteractable;

    private void Awake()
    {
        _model = new KeyModel();
        _grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
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
        _model.SetGrabbed();
    }

    private void HandleKeyGrabbed()
    {
        revealView.ShowObject();
    }
}