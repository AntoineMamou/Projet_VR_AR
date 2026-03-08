using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class HoverGrabColor : MonoBehaviour
{
    private Color _colorDefault;
    private readonly Color _colorHover = Color.cyan;
    private readonly Color _colorGrab  = Color.yellow;

    private MeshRenderer _renderer;
    private XRGrabInteractable _grab;

    private int _hoverCount = 0;
    private int _grabCount  = 0;

    private void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
        _colorDefault = _renderer.material.color;

        _grab = GetComponent<XRGrabInteractable>();

        _grab.hoverEntered.AddListener(_ => { _hoverCount++; UpdateColor(); });
        _grab.hoverExited.AddListener(_  => { _hoverCount--; UpdateColor(); });
        _grab.selectEntered.AddListener(_ => { _grabCount++; UpdateColor(); });
        _grab.selectExited.AddListener(_  => { _grabCount--; UpdateColor(); });
    }

    private void UpdateColor()
    {
        if (_grabCount > 0)
            _renderer.material.color = _colorGrab;
        else if (_hoverCount > 0)
            _renderer.material.color = _colorHover;
        else
            _renderer.material.color = _colorDefault;
    }
}