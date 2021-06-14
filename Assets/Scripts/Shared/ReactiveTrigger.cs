using System;

public class ReactiveTrigger
{
    public event Action OnTriggered;

    public void Fire()
    {
        OnTriggered?.Invoke();
    }
}
    