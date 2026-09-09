using UnityEngine;

/// <summary>
/// 挂载在 playerskin 子物体上
/// </summary>
public class PlayerVisuals : MonoBehaviour
{
    [Header("Paper Hop")]
    [SerializeField, Min(0f)] private float hopHeight = 0.1f;
    [SerializeField, Min(0.1f)] private float hopsPerSecond = 5.5f;
    [SerializeField, Range(0f, 20f)] private float tiltAngle = 6f;
    [SerializeField, Range(0f, 0.25f)] private float landingSquash = 0.09f;
    [SerializeField, Range(0f, 0.25f)] private float airStretch = 0.04f;
    [SerializeField, Min(0.1f)] private float settleSpeed = 18f;

    private SpriteRenderer spriteRenderer;
    private PlayerController playerController;
    private Transform visualTransform;

    private Vector3 baseScale;
    private Vector3 baseLocalPosition;
    private Quaternion baseLocalRotation;
    private float hopCycle;
    private bool wasMoving;

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
            visualTransform = spriteRenderer.transform;
            baseScale = visualTransform.localScale;
            baseLocalPosition = visualTransform.localPosition;
            baseLocalRotation = visualTransform.localRotation;
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

        bool moving = playerController.InputDirection.sqrMagnitude > 0.01f;
        if (moving)
        {
            UpdatePaperHop();
        }
        else
        {
            SettleToBasePose();
        }

        wasMoving = moving;
    }

    private void UpdatePaperHop()
    {
        if (!wasMoving)
        {
            hopCycle = 0f;
        }

        hopCycle += Time.deltaTime * hopsPerSecond;
        float cyclePosition = Mathf.Repeat(hopCycle, 1f);
        float arc = Mathf.Sin(cyclePosition * Mathf.PI);
        float contact = Mathf.Pow(Mathf.Abs(Mathf.Cos(cyclePosition * Mathf.PI)), 12f);
        float alternatingSide = (Mathf.FloorToInt(hopCycle) & 1) == 0 ? -1f : 1f;

        visualTransform.localPosition = baseLocalPosition + Vector3.up * (hopHeight * arc);
        visualTransform.localRotation = baseLocalRotation * Quaternion.Euler(0f, 0f, alternatingSide * tiltAngle * arc);

        float horizontalScale = 1f + landingSquash * contact - airStretch * 0.35f * arc;
        float verticalScale = 1f - landingSquash * contact + airStretch * arc;
        visualTransform.localScale = new Vector3(
            baseScale.x * horizontalScale,
            baseScale.y * verticalScale,
            baseScale.z);
    }

    private void SettleToBasePose()
    {
        hopCycle = 0f;
        float blend = 1f - Mathf.Exp(-settleSpeed * Time.deltaTime);
        visualTransform.localPosition = Vector3.Lerp(visualTransform.localPosition, baseLocalPosition, blend);
        visualTransform.localRotation = Quaternion.Slerp(visualTransform.localRotation, baseLocalRotation, blend);
        visualTransform.localScale = Vector3.Lerp(visualTransform.localScale, baseScale, blend);
    }

    private void HandleSpriteFlip()
    {
        float moveX = playerController.InputDirection.x;
        if (moveX > 0)
            spriteRenderer.flipX = false;
        else if (moveX < 0)
            spriteRenderer.flipX = true;
    }

    private void OnDisable()
    {
        if (visualTransform == null)
        {
            return;
        }

        visualTransform.localPosition = baseLocalPosition;
        visualTransform.localRotation = baseLocalRotation;
        visualTransform.localScale = baseScale;
        hopCycle = 0f;
        wasMoving = false;
    }

}
