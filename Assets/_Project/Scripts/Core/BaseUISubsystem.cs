using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseUISubsystem<T> where T : BaseUIElement
{
    protected Dictionary<string, T> elements = new Dictionary<string, T>();
    protected T currentElement;
    protected UIManager manager;
    protected UIType uiType;

    public T CurrentElement => currentElement;

    public BaseUISubsystem(UIManager manager, UIType uiType)
    {
        this.manager = manager;
        this.uiType = uiType;
    }

    public virtual void HideAll()
    {
        manager.HideAllUI(elements);
    }

    public virtual T GetExist<TElement>() where TElement : T
    {
        return manager.GetExistUI<TElement, T>(elements);
    }

    public virtual T GetNew<TElement>() where TElement : T
    {
        return manager.GetNewUI<TElement>(uiType);
    }

    public virtual void Remove(string elementName)
    {
        manager.RemoveUI(elementName, elements);
    }

    public virtual void Show<TElement>(object data = null, bool forceShowData = true) where TElement : T
    {
        currentElement = manager.ShowUIElementLogic<TElement, T>(
            data,
            forceShowData,
            elements,
            currentElement,
            () => GetNew<TElement>(),
            Remove
        );
    }
}