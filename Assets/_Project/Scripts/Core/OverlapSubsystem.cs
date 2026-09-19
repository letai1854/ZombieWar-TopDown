using System.Collections.Generic;
using UnityEngine;

public class OverlapSubsystem : BaseUISubsystem<BaseOverlap>
{
    protected Dictionary<string, BaseOverlap> activeOverlaps = new Dictionary<string, BaseOverlap>();

    public OverlapSubsystem(UIManager manager) : base(manager, UIType.Overlap) { }

    public override void Show<TElement>(object data = null, bool forceShowData = true)
    {
        string elementName = typeof(TElement).Name;
        BaseOverlap result;

        if (!elements.TryGetValue(elementName, out result))
        {
            result = GetNew<TElement>();
            if (result != null)
                elements.Add(elementName, result);
            else
            {
                Debug.LogError($"Failed to create Overlap UI: {elementName}");
                return;
            }
        }

        bool shouldShow = forceShowData || result.IsHide;
        if (shouldShow)
        {
            result.transform.SetAsLastSibling();
            result.Show(data);

            if (!activeOverlaps.ContainsKey(elementName))
                activeOverlaps[elementName] = result;

            currentElement = result;
        }
    }

    public  void Hide(string elementName)
    {
        if (elements.TryGetValue(elementName, out BaseOverlap element))
        {
            if (!element.IsHide)
            {
                element.Hide(); 

                if (activeOverlaps.ContainsKey(elementName))
                    activeOverlaps.Remove(elementName);
            }
        }
    }

    public override void Remove(string elementName)
    {
        if (elements.TryGetValue(elementName, out BaseOverlap element))
        {
            if (element.gameObject != null)
                element.gameObject.SetActive(false); 
        }

        base.Remove(elementName);

        if (activeOverlaps.ContainsKey(elementName))
            activeOverlaps.Remove(elementName);
    }

    public void HideSpecific<TElement>() where TElement : BaseOverlap
    {
        string elementName = typeof(TElement).Name;
        Hide(elementName);
    }

    public override void HideAll()
    {
        List<string> keys = new List<string>(activeOverlaps.Keys);

        foreach (string key in keys)
        {
            BaseOverlap overlap = activeOverlaps[key];
            if (overlap != null && !overlap.IsHide)
                overlap.Hide(); 
        }

        activeOverlaps.Clear();
    }

    public void NotifyOverlapHidden(BaseOverlap overlap)
    {
        if (overlap != null)
        {
            string key = null;
            foreach (var pair in activeOverlaps)
            {
                if (pair.Value == overlap)
                {
                    key = pair.Key;
                    break;
                }
            }

            if (key != null)
                activeOverlaps.Remove(key);
        }
    }
}