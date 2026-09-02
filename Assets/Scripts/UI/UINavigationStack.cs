using System.Collections.Generic;

public sealed class UINavigationStack
{
    private readonly List<UIScreen> screens = new List<UIScreen>();

    public int Count => screens.Count;
    public UIScreen Current => screens.Count > 0 ? screens[screens.Count - 1] : null;

    public void Clear()
    {
        screens.Clear();
    }

    public void Push(UIScreen screen)
    {
        screens.Add(screen);
    }

    public UIScreen Pop()
    {
        if (screens.Count == 0)
        {
            return null;
        }

        int currentIndex = screens.Count - 1;
        UIScreen screen = screens[currentIndex];
        screens.RemoveAt(currentIndex);
        return screen;
    }
}
