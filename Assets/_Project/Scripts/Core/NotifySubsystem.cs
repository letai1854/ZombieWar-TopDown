using UnityEngine;

public class NotifySubsystem : BaseUISubsystem<BaseNotify>
{
    public NotifySubsystem(UIManager manager) : base(manager, UIType.Notify) { }
}