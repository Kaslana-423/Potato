using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 5f;

    [Header("地图边界")]
    [SerializeField] private BoxCollider2D movementBounds;
    [SerializeField, Min(0f)] private float boundaryPadding = 0.02f;

    private Rigidbody2D rb;
    private Collider2D bodyCollider;
    private Vector2 movement;
    private bool controlEnabled = true;
    private bool boundaryEnabled = true;

    // 暴露物理速度，用于驱动动画的播放频率（例如停下时速度为0，动画自然停止）
    public Vector2 CurrentVelocity => rb != null ? rb.velocity : Vector2.zero;

    // 暴露输入方向，用于立即翻转贴图朝向，比读 velocity 响应更干脆
    public Vector2 InputDirection => movement;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bodyCollider = GetComponent<Collider2D>();
        rb.gravityScale = 0f;
        rb.constraints |= RigidbodyConstraints2D.FreezeRotation;
        rb.rotation = 0f;
        rb.angularVelocity = 0f;
        RestorePosition(transform.position);
    }

    void Update()
    {
        if (!controlEnabled)
        {
            movement = Vector2.zero;
            return;
        }

        // 纯逻辑更新，绝对不在这里调 Transform 和 Sprite
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        if (!controlEnabled)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        // 把碰撞体整体留在墙内，也防止旧存档或碰撞修正把玩家留在场外。
        Vector2 position = ClampPosition(rb.position);
        Vector2 nextPosition = ClampPosition(position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
        if (position != rb.position)
        {
            rb.position = position;
        }
        rb.velocity = (nextPosition - position) / Time.fixedDeltaTime;
    }

    public void RestorePosition(Vector3 position)
    {
        // 读档发生在物理步之外，先同步新地图和玩家的碰撞体尺寸。
        Physics2D.SyncTransforms();
        Vector2 safePosition = ClampPosition(position);
        transform.position = new Vector3(safePosition.x, safePosition.y, position.z);
        rb.position = safePosition;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    public void SetControlEnabled(bool value)
    {
        controlEnabled = value;
        if (!value)
        {
            movement = Vector2.zero;
            rb.velocity = Vector2.zero;
        }
    }

    public void SetBoundaryEnabled(bool value)
    {
        boundaryEnabled = value;
    }

    public void SetTransitionPosition(Vector3 position)
    {
        transform.position = position;
        rb.position = position;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    private Vector2 ClampPosition(Vector2 position)
    {
        if (!boundaryEnabled || movementBounds == null || bodyCollider == null)
        {
            return position;
        }

        Bounds arena = movementBounds.bounds;
        Bounds body = bodyCollider.bounds;
        Vector2 offset = (Vector2)body.center - rb.position;
        Vector2 min = (Vector2)arena.min + (Vector2)body.extents - offset + Vector2.one * boundaryPadding;
        Vector2 max = (Vector2)arena.max - (Vector2)body.extents - offset - Vector2.one * boundaryPadding;
        position.x = min.x <= max.x ? Mathf.Clamp(position.x, min.x, max.x) : arena.center.x - offset.x;
        position.y = min.y <= max.y ? Mathf.Clamp(position.y, min.y, max.y) : arena.center.y - offset.y;
        return position;
    }
}
