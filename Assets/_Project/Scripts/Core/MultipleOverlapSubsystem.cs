using System;
using System.Collections.Generic;
using UnityEngine;

public class MultipleOverlapSubsystem
{
    protected Dictionary<string, BaseOverlap> elements = new Dictionary<string, BaseOverlap>();
    protected List<BaseOverlap> activeElements = new List<BaseOverlap>(); 
    protected UIManager manager;

    public MultipleOverlapSubsystem(UIManager manager)
    {
        this.manager = manager;
    }

    public void HideAll()
    {
        foreach (var element in activeElements)
        {
            if (element != null && !element.IsHide)
            {
                element.Hide();
            }
        }
        activeElements.Clear();
    }

    public T GetExist<T>() where T : BaseOverlap
    {
        string elementName = typeof(T).Name;
        if (elements.TryGetValue(elementName, out BaseOverlap element))
        {
            return element as T;
        }
        return null;
    }

    public T GetNew<T>() where T : BaseOverlap
    {
        return manager.GetNewUI<T>(UIType.Overlap);
    }

    public void Remove(string elementName)
    {
        if (elements.TryGetValue(elementName, out BaseOverlap element))
        {
            if (element != null)
            {
                activeElements.Remove(element);
                if (element.gameObject != null)
                {
                    element.gameObject.SetActive(false);
                }
            }
            elements.Remove(elementName);
        }
    }

    public void Show<T>(object data = null, bool forceShowData = false) where T : BaseOverlap
    {
        string elementName = typeof(T).Name;
        BaseOverlap result;

        if (!elements.TryGetValue(elementName, out result))
        {
            result = GetNew<T>();
            if (result != null)
            {
                elements.Add(elementName, result);
            }
            else
            {
                Debug.LogError($"Failed to create UI element: {elementName}");
                return;
            }
        }

        bool isShow = false;
        if (result != null && (forceShowData || result.IsHide))
        {
            isShow = true;
        }

        if (isShow && result != null)
        {
            result.transform.SetAsLastSibling();

            result.Show(data);
            result.gameObject.SetActive(true);

            if (!activeElements.Contains(result))
                activeElements.Add(result);
        }
    }

    public void Hide<T>() where T : BaseOverlap
    {
        string elementName = typeof(T).Name;
        if (elements.TryGetValue(elementName, out BaseOverlap element))
        {
            if (element != null && !element.IsHide)
            {
                element.Hide();
                activeElements.Remove(element);
            }
        }
    }
}