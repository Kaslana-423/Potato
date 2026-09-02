using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class UIRouter : MonoBehaviour
{
    [SerializeField] private bool handleCancelInput = true;

    private readonly Dictionary<UIRoute, UIScreen> screens = new Dictionary<UIRoute, UIScreen>();
    private readonly UINavigationStack navigationStack = new UINavigationStack();
    private Func<bool> backInterceptor;

    public event Action<UIRoute> RouteChanged;

    public UIRoute CurrentRoute => navigationStack.Current != null
        ? navigationStack.Current.Route
        : UIRoute.None;

    public bool CanGoBack => navigationStack.Count > 1;

    private void Update()
    {
        if (handleCancelInput && Input.GetButtonDown("Cancel"))
        {
            Back();
        }
    }

    public void Register(UIScreen screen)
    {
        if (screen == null || screen.Route == UIRoute.None)
        {
            return;
        }

        screens[screen.Route] = screen;
    }

    public void SetBackInterceptor(Func<bool> interceptor)
    {
        backInterceptor = interceptor;
    }

    public bool Initialize(UIRoute rootRoute)
    {
        if (!screens.TryGetValue(rootRoute, out UIScreen rootScreen))
        {
            Debug.LogError($"Cannot initialize UI router. Route is not registered: {rootRoute}", this);
            return false;
        }

        foreach (KeyValuePair<UIRoute, UIScreen> entry in screens)
        {
            if (entry.Key != rootRoute)
            {
                entry.Value.Exit();
            }
        }

        navigationStack.Clear();
        navigationStack.Push(rootScreen);
        rootScreen.Enter();
        RouteChanged?.Invoke(rootRoute);
        return true;
    }

    public bool Navigate(UIRoute route)
    {
        if (!screens.TryGetValue(route, out UIScreen nextScreen))
        {
            Debug.LogWarning($"Cannot navigate to unregistered UI route: {route}", this);
            return false;
        }

        if (CurrentRoute == route)
        {
            return true;
        }

        if (navigationStack.Current != null)
        {
            navigationStack.Current.Pause();
        }

        navigationStack.Push(nextScreen);
        nextScreen.Enter();
        RouteChanged?.Invoke(route);
        return true;
    }

    public bool Back()
    {
        if (backInterceptor != null && backInterceptor())
        {
            return true;
        }

        if (!CanGoBack)
        {
            return false;
        }

        UIScreen currentScreen = navigationStack.Pop();
        currentScreen.Exit();
        navigationStack.Current.Resume();
        RouteChanged?.Invoke(navigationStack.Current.Route);
        return true;
    }
}
