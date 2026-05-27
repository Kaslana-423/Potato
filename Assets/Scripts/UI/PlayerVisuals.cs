using UnityEngine;

/// <summary>
/// 挂载在 playerskin 子物体上
/// </summary>
public class PlayerVisuals : MonoBehaviour
{
    [Header("动画设置")]
    public float wobbleSpeed = 1.5f;
    public float wobbleAngle = 15f;
    public float squashAmount = 0.15f;

    private SpriteRenderer spriteRenderer;
    private PlayerController playerController;

    private float wobbleTimer;
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

    void Update()
    {
        if (spriteRenderer == null || playerController == null) return;

        HandleSpriteFlip();
        UpdateWobbleAnimation();
    }

    private void HandleSpriteFlip()
    {
        float moveX = playerController.InputDirection.x;
        if (moveX > 0)
            spriteRenderer.flipX = false;
        else if (moveX < 0)
            spriteRenderer.flipX = true;
    }

    private void UpdateWobbleAnimation()
    {
        float currentSpeed = playerController.CurrentVelocity.magnitude;

        if (currentSpeed > 0.1f)
        {
            wobbleTimer += Time.deltaTime * currentSpeed * wobbleSpeed;

            float zAngle = Mathf.Sin(wobbleTimer) * wobbleAngle;
            // 晃动和缩放只作用于视觉子级，不影响父级 Collider
            spriteRenderer.transform.localRotation = Quaternion.Euler(0, 0, zAngle);

            float scaleY = 1f + Mathf.Cos(wobbleTimer * 2f) * squashAmount;
            float scaleX = 1f - Mathf.Cos(wobbleTimer * 2f) * squashAmount;

            spriteRenderer.transform.localScale = new Vector3(baseScale.x * scaleX, baseScale.y * scaleY, baseScale.z);
        }
        else
        {
            wobbleTimer = 0f;
            spriteRenderer.transform.localRotation = Quaternion.Lerp(spriteRenderer.transform.localRotation, Quaternion.identity, Time.deltaTime * 15f);
            spriteRenderer.transform.localScale = Vector3.Lerp(spriteRenderer.transform.localScale, baseScale, Time.deltaTime * 15f);
        }
    }
}