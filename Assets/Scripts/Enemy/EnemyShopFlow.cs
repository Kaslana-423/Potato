using System;
using UnityEngine;
using UnityEngine.UI;

public sealed class EnemyShopFlow
{
    private readonly Action exitShopAction;
    private Button boundShopExitButton;

    public EnemyShopFlow(Action exitShopAction)
    {
        this.exitShopAction = exitShopAction;
    }

    public void Open(
        ref ShopManager shopManager,
        GameObject shopRoot,
        ref Button shopExitButton,
        bool refreshShopWhenOpened)
    {
        AutoBind(ref shopManager, shopRoot, ref shopExitButton);
        SetVisible(ref shopManager, shopRoot, ref shopExitButton, true);

        if (refreshShopWhenOpened && shopManager != null)
        {
            shopManager.RefreshShop();
        }

        BindExitButton(shopExitButton);
    }

    public void SetVisible(
        ref ShopManager shopManager,
        GameObject shopRoot,
        ref Button shopExitButton,
        bool visible)
    {
        AutoBind(ref shopManager, shopRoot, ref shopExitButton);
        if (shopManager != null)
        {
            shopManager.SetShopOpen(visible);
            return;
        }

        if (shopRoot != null && shopRoot.activeSelf != visible)
        {
            shopRoot.SetActive(visible);
        }
    }

    public void AutoBind(
        ref ShopManager shopManager,
        GameObject shopRoot,
        ref Button shopExitButton)
    {
        if (shopManager == null)
        {
            shopManager = UnityEngine.Object.FindObjectOfType<ShopManager>(true);
        }

        if (shopExitButton == null && shopRoot != null)
        {
            shopExitButton = FindButtonInChildren(
                shopRoot.transform,
                "NextWaveButton",
                "Next Wave Button",
                "StartNextWaveButton",
                "Start Next Wave Button",
                "ExitShopButton",
                "Exit Shop Button");
        }

        if (shopExitButton == null && shopManager != null)
        {
            shopExitButton = FindButtonInChildren(
                shopManager.transform,
                "NextWaveButton",
                "Next Wave Button",
                "StartNextWaveButton",
                "Start Next Wave Button",
                "ExitShopButton",
                "Exit Shop Button");
        }
    }

    public void BindExitButton(Button shopExitButton)
    {
        if (boundShopExitButton == shopExitButton)
        {
            return;
        }

        UnbindExitButton();
        boundShopExitButton = shopExitButton;
        if (boundShopExitButton != null)
        {
            boundShopExitButton.onClick.AddListener(HandleExitShopClicked);
        }
    }

    public void UnbindExitButton()
    {
        if (boundShopExitButton != null)
        {
            boundShopExitButton.onClick.RemoveListener(HandleExitShopClicked);
            boundShopExitButton = null;
        }
    }

    private void HandleExitShopClicked()
    {
        exitShopAction?.Invoke();
    }

    private static Button FindButtonInChildren(Transform root, params string[] names)
    {
        if (root == null)
        {
            return null;
        }

        foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
        {
            foreach (string objectName in names)
            {
                if (child.name == objectName)
                {
                    return child.GetComponent<Button>();
                }
            }
        }

        return null;
    }
}
