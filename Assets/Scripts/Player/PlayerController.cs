using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 movement;

    // 暴露物理速度，用于驱动动画的播放频率（例如停下时速度为0，动画自然停止）
    public Vector2 CurrentVelocity => rb.velocity;

    // 暴露输入方向，用于立即翻转贴图朝向，比读 velocity 响应更干脆
    public Vector2 InputDirection => movement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // 纯逻辑更新，绝对不在这里调 Transform 和 Sprite
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        // 物理更新
        rb.velocity = movement.normalized * moveSpeed;
    }
}