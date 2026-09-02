using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class UIScreen : MonoBehaviour
{
    [SerializeField] private UIRoute route;
    [SerializeField] private GameObject contentRoot;
    [SerializeField] private Selectable initialSelection;

    private GameObject lastSelectedObject;

    public UIRoute Route => route;
    public bool IsVisible => contentRoot != null && contentRoot.activeSelf;

    public void Configure(UIRoute newRoute, GameObject newContentRoot, Selectable newInitialSelection)
    {
        route = newRoute;
        contentRoot = newContentRoot != null ? newContentRoot : gameObject;
        initialSelection = newInitialSelection;
    }

    public void Enter()
    {
        SetVisible(true);
        OnEnter();
        Select(initialSelection != null ? initialSelection.gameObject : null);
    }

    public void Exit()
    {
        OnExit();
        lastSelectedObject = null;
        SetVisible(false);
    }

    public void Pause()
    {
        RememberSelection();
        OnPause();
        SetVisible(false);
    }

    public void Resume()
    {
        SetVisible(true);
        OnResume();

        GameObject target = IsSelectable(lastSelectedObject)
            ? lastSelectedObject
            : initialSelection != null ? initialSelection.gameObject : null;
        Select(target);
    }

    protected virtual void OnEnter()
    {
    }

    protected virtual void OnExit()
    {
    }

    protected virtual void OnPause()
    {
    }

    protected virtual void OnResume()
    {
    }

    private void RememberSelection()
    {
        EventSystem eventSystem = EventSystem.current;
        GameObject selected = eventSystem != null ? eventSystem.currentSelectedGameObject : null;
        if (selected != null && contentRoot != null && selected.transform.IsChildOf(contentRoot.transform))
        {
            lastSelectedObject = selected;
        }
    }

    private void SetVisible(bool visible)
    {
        GameObject root = contentRoot != null ? contentRoot : gameObject;
        if (root.activeSelf != visible)
        {
            root.SetActive(visible);
        }
    }

    private static bool IsSelectable(GameObject target)
    {
        if (target == null || !target.activeInHierarchy)
        {
            return false;
        }

        Selectable selectable = target.GetComponent<Selectable>();
        return selectable != null && selectable.IsInteractable();
    }

    private static void Select(GameObject target)
    {
        EventSystem eventSystem = EventSystem.current;
        if (eventSystem == null || !IsSelectable(target))
        {
            return;
        }

        eventSystem.SetSelectedGameObject(null);
        eventSystem.SetSelectedGameObject(target);
    }
}
