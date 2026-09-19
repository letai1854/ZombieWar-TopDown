using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum UIType
{
    Unknow,
    Screen,
    Popup,
    Notify,
    Overlap
}

public class UIManager : BaseManager<UIManager>
{
    public GameObject cScreen, cPopup, cNotify, cOverlap;
    private Dictionary<string, List<GameObject>> inactiveUIElements = new Dictionary<string, List<GameObject>>();
    public GameObject Portait;
    public CanvasScaler canvas;

    private ScreenSubsystem screenSubsystem;
    private PopupSubsystem popupSubsystem;
    private NotifySubsystem notifySubsystem;
    private OverlapSubsystem overlapSubsystem;

    public ScreenSubsystem Screens => screenSubsystem;
    public PopupSubsystem Popups => popupSubsystem;
    public NotifySubsystem Notifies => notifySubsystem;
    public OverlapSubsystem Overlaps => overlapSubsystem;

    public BaseScreen CurScreen => screenSubsystem?.CurrentElement;
    public BasePopup CurPopup => popupSubsystem?.CurrentElement;
    public BaseNotify CurNotify => notifySubsystem?.CurrentElement;
    public BaseOverlap CurOverlap => overlapSubsystem?.CurrentElement;

    private const string SCREEN_RESOURCES_PATH = "Prefabs/UI/Screen/";
    private const string POPUP_RESOURCES_PATH = "Prefabs/UI/Popup/";
    private const string NOTIFY_RESOURCES_PATH = "Prefabs/UI/Notify/";
    private const string OVERLAP_RESOURCES_PATH = "Prefabs/UI/Overlap/";

    protected override void Awake()
    {
        base.Awake();
        InitializeSubsystems();
        SetupCanvas();
    }
    public void SetupCanvas()
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        if (screenWidth <= 1500)
        {
            canvas.referenceResolution = new Vector2(1500, 1920);
        }
        if (screenWidth >= 2000)
        {
            canvas.referenceResolution = new Vector2(2048, 1920);
        }
        if (screenWidth > 1500 && screenWidth <= 1800)
        {
            canvas.referenceResolution = new Vector2(1800, 1920);
        }
        if (screenWidth > 1800 && screenWidth < 2000)
        {
            canvas.referenceResolution = new Vector2(1900, 1920);
        }
    }

    private void InitializeSubsystems()
    {
        screenSubsystem = new ScreenSubsystem(this);
        popupSubsystem = new PopupSubsystem(this);
        notifySubsystem = new NotifySubsystem(this);
        overlapSubsystem = new OverlapSubsystem(this);
    }

    #region Generic Helpers (Internal Implementation for Subsystems)

    internal Transform GetParentTransform(UIType type)
    {
        switch (type)
        {
            case UIType.Screen: return cScreen.transform;
            case UIType.Popup: return cPopup.transform;
            case UIType.Notify: return cNotify.transform;
            case UIType.Overlap: return cOverlap.transform;
            default:
                Debug.LogError($"Unsupported UIType for parent: {type}");
                return null;
        }
    }

    internal T GetNewUI<T>(UIType uiType) where T : MonoBehaviour
    {
        string uiName = typeof(T).Name;

        string poolKey = $"{uiType}_{uiName}";
        if (inactiveUIElements.TryGetValue(poolKey, out var pooledElements) && pooledElements.Count > 0)
        {
            GameObject existingElement = pooledElements[pooledElements.Count - 1];
            pooledElements.RemoveAt(pooledElements.Count - 1);

            if (existingElement != null)
            {
                existingElement.SetActive(false);
                Transform parentTransform = GetParentTransform(uiType);
                existingElement.transform.SetParent(parentTransform);
                existingElement.transform.localScale = Vector3.one;
                existingElement.transform.localPosition = Vector3.zero;
                existingElement.transform.localRotation = Quaternion.identity;

                T componentInstance = existingElement.GetComponent<T>();
                return componentInstance;
            }
        }

        GameObject pfUI = GetUIPrefab(uiType, uiName);
        if (pfUI == null)
        {
            throw new MissingReferenceException($"Cannot find prefab for {uiName} ({uiType}). Path: {GetUIPrefabPath(uiType, uiName)}");
        }

        T uiComponent = pfUI.GetComponent<T>();
        if (uiComponent == null)
        {
            throw new MissingComponentException($"Prefab for {uiName} ({uiType}) does not have the required component {typeof(T).FullName}.");
        }

        Transform parent = GetParentTransform(uiType);
        if (parent == null) return null;

        GameObject ob = Instantiate(pfUI, parent) as GameObject;
        ob.transform.localScale = Vector3.one;
        ob.transform.localPosition = Vector3.zero;
        ob.transform.localRotation = Quaternion.identity;
        ob.SetActive(false);

#if UNITY_EDITOR
        ob.name = $"{uiType.ToString().ToUpper()}_{uiName}";
#endif

        T instance = ob.GetComponent<T>();
        BaseUIElement uiElementInstance = instance as BaseUIElement;
        if (uiElementInstance != null)
        {
            uiElementInstance.Init();
        }
        else
        {
            Debug.LogError($"Instantiated UI {typeof(T).Name} does not inherit from BaseUIElement. Cannot call Init().", instance);
        }
        return instance;
    }

    internal void RemoveUI<T>(string name, Dictionary<string, T> collection) where T : MonoBehaviour
    {
        if (collection.TryGetValue(name, out T uiElement))
        {
            if (uiElement != null && uiElement.gameObject != null)
            {
                GameObject element = uiElement.gameObject;
                element.SetActive(false);
                string poolKey = $"{(uiElement as BaseUIElement)?.UIType ?? UIType.Unknow}_{name}";
                if (!inactiveUIElements.TryGetValue(poolKey, out var pooledElements))
                {
                    pooledElements = new List<GameObject>();
                    inactiveUIElements[poolKey] = pooledElements;
                }
                pooledElements.Add(element);
            }
            collection.Remove(name);
        }
    }

    internal void HideAllUI<T>(Dictionary<string, T> collection) where T : MonoBehaviour
    {
        List<string> keys = new List<string>(collection.Keys);
        foreach (string key in keys)
        {
            if (collection.TryGetValue(key, out T uiElementComponent))
            {
                if (uiElementComponent != null)
                {
                    BaseUIElement element = uiElementComponent as BaseUIElement;
                    if (element != null && !element.IsHide)
                    {
                        element.Hide();
                    }
                }
            }
        }
    }

    internal U GetExistUI<T, U>(Dictionary<string, U> collection) where T : U where U : MonoBehaviour
    {
        string uiName = typeof(T).Name;
        if (collection.TryGetValue(uiName, out U uiElement))
        {
            return uiElement;
        }
        return null;
    }

    internal U ShowUIElementLogic<T, U>(
        object data,
        bool forceShowData,
        Dictionary<string, U> collection,
        U currentElement,
        Func<U> createNewElement,
        Action<string> removeElementAction
        ) where T : U where U : MonoBehaviour
    {
        string elementName = typeof(T).Name;
        U result = null;
        U newCurrentElement = currentElement;

        if (currentElement != null)
        {
            var currentName = currentElement.GetType().Name;
            if (currentName.Equals(elementName))
            {
                result = currentElement;
            }
            else
            {
                (currentElement as BaseUIElement)?.Hide();
                removeElementAction(currentName);
                newCurrentElement = null;
            }
        }

        if (result == null)
        {
            if (!collection.TryGetValue(elementName, out result))
            {
                result = createNewElement();
                if (result != null)
                {
                    collection.Add(elementName, result);
                }
                else
                {
                    Debug.LogError($"Failed to create new UI element: {elementName}");
                    return newCurrentElement;
                }
            }
        }

        bool isShow = false;
        if (result != null)
        {
            BaseUIElement resultElement = result as BaseUIElement;
            if (resultElement != null && (forceShowData || resultElement.IsHide))
            {
                isShow = true;
            }
        }

        if (isShow && result != null)
        {
            newCurrentElement = result;
            result.transform.SetAsLastSibling();
            (result as BaseUIElement)?.Show(data);
            result.gameObject.SetActive(true);
        }
        else if (result != null && !isShow)
        {
            if (currentElement != null && currentElement.GetType().Name.Equals(elementName))
            {
                newCurrentElement = currentElement;
            }
        }

        return newCurrentElement;
    }

    private string GetUIPrefabPath(UIType t, string uiName)
    {
        switch (t)
        {
            case UIType.Screen: return SCREEN_RESOURCES_PATH + uiName;
            case UIType.Popup: return POPUP_RESOURCES_PATH + uiName;
            case UIType.Notify: return NOTIFY_RESOURCES_PATH + uiName;
            case UIType.Overlap: return OVERLAP_RESOURCES_PATH + uiName;
            default: return string.Empty;
        }
    }

    private GameObject GetUIPrefab(UIType t, string uiName)
    {
        string defaultPath = GetUIPrefabPath(t, uiName);
        if (string.IsNullOrEmpty(defaultPath))
        {
            Debug.LogError($"Invalid UIType specified: {t}");
            return null;
        }

        GameObject result = Resources.Load(defaultPath) as GameObject;
        return result;
    }


    public void HideSpecificOverlap<T>() where T : BaseOverlap =>
        overlapSubsystem?.HideSpecific<T>();

    public void NotifyOverlapHidden(BaseOverlap overlap) =>
        overlapSubsystem?.NotifyOverlapHidden(overlap);
    #endregion

    #region Facade Methods (Delegating to Subsystems)

    public void HideAllScreens() => screenSubsystem?.HideAll();
    public T GetExistScreen<T>() where T : BaseScreen => (T)screenSubsystem?.GetExist<T>();
    public void ShowScreen<T>(object data = null, bool forceShowData = true) where T : BaseScreen => screenSubsystem?.Show<T>(data, forceShowData);

    public void HideAllPopups() => popupSubsystem?.HideAll();
    public T GetExistPopup<T>() where T : BasePopup => (T)popupSubsystem?.GetExist<T>();
    public void ShowPopup<T>(object data = null, bool forceShowData = true) where T : BasePopup => popupSubsystem?.Show<T>(data, forceShowData);

    public void HideAllNotifies() => notifySubsystem?.HideAll();
    public T GetExistNotify<T>() where T : BaseNotify => (T)notifySubsystem?.GetExist<T>();
    public void ShowNotify<T>(object data = null, bool forceShowData = false) where T : BaseNotify => notifySubsystem?.Show<T>(data, forceShowData);

    public void HideAllOverlaps() => overlapSubsystem?.HideAll();
    public T GetExistOverlap<T>() where T : BaseOverlap => (T)overlapSubsystem?.GetExist<T>();
    public void ShowOverlap<T>(object data = null, bool forceShowData = false) where T : BaseOverlap => overlapSubsystem?.Show<T>(data, forceShowData);

    public void HideAll(UIType type)
    {
        switch (type)
        {
            case UIType.Screen: HideAllScreens(); break;
            case UIType.Popup: HideAllPopups(); break;
            case UIType.Notify: HideAllNotifies(); break;
            case UIType.Overlap: HideAllOverlaps(); break;
            default: Debug.LogWarning($"HideAll called with unsupported UIType: {type}"); break;
        }
    }

    public void HideAll()
    {
        HideAllScreens();
        HideAllPopups();
        HideAllNotifies();
        HideAllOverlaps();
    }

    #endregion

}