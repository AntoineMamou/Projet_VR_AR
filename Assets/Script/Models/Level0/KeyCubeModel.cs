using System;
using UnityEngine;

/// <summary>
/// MVC — Model
/// Contient l'état du KeyCube. N'a aucune dépendance Unity (pas de MonoBehaviour).
/// </summary>
[Serializable]
public class KeyCubeModel
{
    // ── État ───────────────────────────────────────────────────────────────
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

    // ── Événements ─────────────────────────────────────────────────────────
    /// <summary>Déclenché quand IsInZone change. bool = nouvel état.</summary>
    public event Action<bool> OnStateChanged;
}