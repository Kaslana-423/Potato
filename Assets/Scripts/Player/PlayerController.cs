using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 5f;

    [Header("动画设置")]
    public float wobbleSpeed = 1.5f; // 扭曲动画的播放速度系数
    public float wobbleAngle = 15f;  // 最大左右倾斜角度
    public float squashAmount = 0.15f; // 最大挤压/拉伸的形变程度

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private Vector2 movement;
    private float wobbleTimer;
    private Vector3 originalScale;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // 获取身上的或者子物体身上的 SpriteRenderer
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            // 记录初始缩放大小，方便后续在此基础上进行变形
            originalScale = spriteRenderer.transform.localScale;
        }
    }

    void Update()
    {
        // 1. 获取输入 (WASD)
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if (spriteRenderer != null)
        {
            // 2. 根据移动方向翻转贴图
            if (movement.x > 0)
                spriteRenderer.flipX = false;
            else if (movement.x < 0)
                spriteRenderer.flipX = true;

            // 3. 处理程序化扭曲动画
            UpdateWobbleAnimation();
        }
    }

    void FixedUpdate()
    {
        // 4. 物理移动 (在FixedUpdate中修改物理属性更稳定)
        rb.velocity = movement.normalized * moveSpeed;
    }

    private void UpdateWobbleAnimation()
    {
        float currentSpeed = rb.velocity.magnitude;

        if (currentSpeed > 0.1f)
        {
            // 计时器累加：速度越快，累加速度越快（与人物速度成正比）
            wobbleTimer += Time.deltaTime * currentSpeed * wobbleSpeed;

            // 计算旋转：使用 Sin 函数实现左右来回摇摆
            float zAngle = Mathf.Sin(wobbleTimer) * wobbleAngle;
            spriteRenderer.transform.localRotation = Quaternion.Euler(0, 0, zAngle);

            // 计算缩放：使用 Cos 函数实现拉伸与挤压 (也就是果冻弹跳感)
            // 频率乘以2，保证在向左摇和向右摇的时候，都会发生一次完整的弹跳
            float scaleY = 1f + Mathf.Cos(wobbleTimer * 2f) * squashAmount;
            float scaleX = 1f - Mathf.Cos(wobbleTimer * 2f) * squashAmount;

            spriteRenderer.transform.localScale = new Vector3(originalScale.x * scaleX, originalScale.y * scaleY, originalScale.z);
        }
        else
        {
            // 停止移动时，平滑地将角度和缩放恢复到初始状态
            wobbleTimer = 0f;
            spriteRenderer.transform.localRotation = Quaternion.Lerp(spriteRenderer.transform.localRotation, Quaternion.identity, Time.deltaTime * 15f);
            spriteRenderer.transform.localScale = Vector3.Lerp(spriteRenderer.transform.localScale, originalScale, Time.deltaTime * 15f);
        }
    }
}
