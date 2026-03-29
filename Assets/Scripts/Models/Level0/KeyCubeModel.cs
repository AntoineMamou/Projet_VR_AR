using System;
using UnityEngine;

/// <summary>
/// MVC — Model
/// Contient l'état du KeyCube : zone, couleur, et contrôle AR.
/// </summary>
[Serializable]
public class KeyCubeModel
{
    // ── État zone ──────────────────────────────────────────────────────────
    private bool _isInZone;
    public bool IsInZone
    {
        get => _isInZone;
        set
        {
            if (_isInZone == value) return;
            _isInZone = value;
            OnStateChanged?.Invoke(_isInZone);
        }
    }

    // ── État contrôle AR ───────────────────────────────────────────────────
    private bool _isARControlling;
    public bool IsARControlling
    {
        get => _isARControlling;
        set
        {
            if (_isARControlling == value) return;
            _isARControlling = value;
            OnARControllingChanged?.Invoke(_isARControlling);
        }
    }

    // ── État grab VR ───────────────────────────────────────────────────────
    private bool _isGrabbedByVR;
    public bool IsGrabbedByVR
    {
        get => _isGrabbedByVR;
        set
        {
            if (_isGrabbedByVR == value) return;
            _isGrabbedByVR = value;
            OnVRGrabChanged?.Invoke(_isGrabbedByVR);
        }
    }

    public float LockedZ { get; private set; }

    public void SetLockedZ(float z) => LockedZ = z;

    // ── Événements ─────────────────────────────────────────────────────────
    public event Action<bool> OnStateChanged;
    public event Action<bool> OnARControllingChanged;
    public event Action<bool> OnVRGrabChanged;
}