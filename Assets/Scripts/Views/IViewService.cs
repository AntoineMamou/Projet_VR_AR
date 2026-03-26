using UnityEngine;

public interface IViewService
{
    Transform GetWorldAnchor();
    void SetViewMode(bool isGodView, Vector3 focusLocalPoint = default);
    float GetCurrentScale();
}