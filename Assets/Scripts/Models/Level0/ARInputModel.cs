using System;
using UnityEngine;

/// <summary>
/// MVC — Model (AR)
/// Contient la direction courante et le flag IsControlling
/// qui bloque le grab XRI sur le KeyCube.
/// </summary>
public class ARInputModel
{
    // ── État ───────────────────────────────────────────────────────────────
    private float _direction; // -1, 0, +1

    public float Direction
    {
        get => _direction;
        set
        {
            if (Mathf.Approximately(_direction, value)) return;
            _direction = value;
            OnDirectionChanged?.Invoke(_direction);
            OnControllingChanged?.Invoke(IsControlling);
        }
    }

    /// <summary>True si le joueur AR est en train d'appuyer.</summary>
    public bool IsControlling => !Mathf.Approximately(_direction, 0f);

    // ── Événements ─────────────────────────────────────────────────────────
    public event Action<float> OnDirectionChanged;
    public event Action<bool>  OnControllingChanged;
}