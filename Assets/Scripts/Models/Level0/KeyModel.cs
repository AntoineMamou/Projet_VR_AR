using System;
using UnityEngine;
public class KeyModel
{
    public bool IsGrabbed { get; private set; } = false;

    public event Action OnKeyGrabbed;

    public event Action OnKeyTouched;

    public void SetGrabbed()
    {
        IsGrabbed = true;
        OnKeyGrabbed?.Invoke();
    }

    public void SetTouched() => OnKeyTouched?.Invoke();
}