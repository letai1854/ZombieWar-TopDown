using UnityEngine;

public class ScreenSubsystem : BaseUISubsystem<BaseScreen>
{
    public ScreenSubsystem(UIManager manager) : base(manager, UIType.Screen) { }
}