using UnityEngine;

public class MiniatureWorldViewService : MonoBehaviour, IViewService
{
    [Header("Settings")]
    [SerializeField] private float godViewScale = 0.1f;
    [SerializeField] private float fullScale = 1.0f;

    private bool _isGodView = true;

    private void Awake()
    {
        AppBootstrapper.RegisterViewService(this);
        ApplyScale();
    }

    public Transform GetWorldAnchor()
    {
        return this.transform;
    }

    public void SetViewMode(bool isGodView, Vector3 focusLocalPoint = default)
    {
        _isGodView = isGodView;

        if (isGodView)
        {
            transform.localScale = Vector3.one * godViewScale;
            transform.localPosition = new Vector3(0, -0.5f, 1f);
        }
        else
        {
            transform.localScale = Vector3.one * fullScale;
            transform.position = -focusLocalPoint;
        }
    }
    public float GetCurrentScale() => _isGodView ? godViewScale : fullScale;

    private void ApplyScale()
    {
        float scale = GetCurrentScale();
        transform.localScale = new Vector3(scale, scale, scale);
    }
}