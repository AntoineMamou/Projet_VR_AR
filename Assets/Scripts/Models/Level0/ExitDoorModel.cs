using System;

/// <summary>
/// MVC — Model
/// Contient l'état de l'ExitDoor. Pur C#, pas de MonoBehaviour.
/// </summary>
public class ExitDoorModel
{
    private bool _isVisible;

    public bool IsVisible
    {
        get => _isVisible;
        set
        {
            if (_isVisible == value) return;
            _isVisible = value;
            OnVisibilityChanged?.Invoke(_isVisible);
        }
    }

    public event Action<bool> OnVisibilityChanged;
}