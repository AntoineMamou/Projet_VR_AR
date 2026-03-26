using UnityEngine;

public class StandardViewService : MonoBehaviour, IViewService
{
    private void Awake()
    {
        AppBootstrapper.RegisterViewService(this);
    }

    public Transform GetWorldAnchor() => this.transform;

    public float GetCurrentScale() => 1.0f;

    public void SetViewMode(bool isGodView, Vector3 focusLocalPoint = default)
    {
        Debug.Log("[StandardView] Mode switch ignored in VR.");
    }
}