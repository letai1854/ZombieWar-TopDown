using UnityEngine;

public class PopupSubsystem : BaseUISubsystem<BasePopup>
{
    public PopupSubsystem(UIManager manager) : base(manager, UIType.Popup) { }
}