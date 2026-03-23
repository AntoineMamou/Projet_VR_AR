using System;

public class KeyModel
{
    public event Action OnKeyGrabbed;

    public bool IsGrabbed { get; private set; }

    public void SetGrabbed()
    {
        if (IsGrabbed)
        {
            return;
        }

        IsGrabbed = true;
        OnKeyGrabbed?.Invoke();
    }
}
