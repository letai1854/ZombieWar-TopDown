public class BaseOverlap : BaseUIElement
{
    public override void Hide()
    {
        base.Hide();
        uiType = UIType.Overlap;

        if (UIManager.HasInstance)
            UIManager.Instance.NotifyOverlapHidden(this);


        gameObject.SetActive(false);
    }

    public override void Init()
    {
        base.Init();
    }

    public override void Show(object data)
    {
        base.Show(data);

        gameObject.SetActive(true);
    }
}