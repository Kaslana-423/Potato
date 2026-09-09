using UnityEngine;

/// <summary>
/// 挂载在 playerskin 子物体上
/// </summary>
public class PlayerVisuals : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private PlayerController playerController;

    private Vector3 baseScale;

    void Start()
    {
        // 核心改动：向上在父物体（Player本体）上寻找控制脚本
        playerController = GetComponentInParent<PlayerController>();

        // SpriteRenderer 通常就在 skin 本身或者它的子级上
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (spriteRenderer != null)
        {
            // 注意：现在直接记录 skin 物体或者 sprite 的本地缩放
            baseScale = spriteRenderer.transform.localScale;
        }

        if (playerController == null)
        {
            Debug.LogError($"严重错误：在 {gameObject.name} 的父级找不到 PlayerController 组件！");
        }
    }

    public void UpdateBaseScale(Vector3 newScale)
    {
        baseScale = newScale;
    }

    void LateUpdate()
    {
        if (spriteRenderer == null || playerController == null) return;

        HandleSpriteFlip();
        spriteRenderer.transform.localRotation = Quaternion.identity;
        spriteRenderer.transform.localScale = baseScale;
    }

    private void HandleSpriteFlip()
    {
        float moveX = playerController.InputDirection.x;
        if (moveX > 0)
            spriteRenderer.flipX = false;
        else if (moveX < 0)
            spriteRenderer.flipX = true;
    }

}
