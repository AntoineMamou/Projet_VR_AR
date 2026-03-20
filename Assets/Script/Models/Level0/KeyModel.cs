using System;
using UnityEngine;
public class KeyModel
{
    public bool IsGrabbed { get; private set; } = false;

    public event Action OnKeyGrabbed;

    public void SetGrabbed()
    {
        IsGrabbed = true;
        OnKeyGrabbed?.Invoke();
    }
}