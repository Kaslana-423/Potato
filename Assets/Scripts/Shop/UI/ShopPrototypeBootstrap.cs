#if UNITY_EDITOR
using UnityEngine;

public static class ShopPrototypeBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void EnsurePrototypeShop()
    {
        if (Object.FindObjectOfType<ShopManager>() != null)
        {
            return;
        }

        var prototype = new GameObject("Shop Prototype");
        prototype.AddComponent<ShopManager>();
    }
}
#endif
