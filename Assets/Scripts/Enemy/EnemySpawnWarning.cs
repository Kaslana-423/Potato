using UnityEngine;

public sealed class EnemySpawnWarning : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color warningColor = new Color(1f, 0.2f, 0.08f, 0.45f);
    [SerializeField] private Color finalColor = new Color(1f, 0.05f, 0.02f, 0.8f);
    [SerializeField, Min(0.1f)] private float pulseScale = 1.25f;

    private static Sprite defaultSprite;
    private float duration;
    private float timer;
    private Vector3 baseScale;

    private void Awake()
    {
        EnsureRenderer();
    }

    public void Play(Vector3 position, float warningDuration, float radius)
    {
        transform.position = position;
        duration = Mathf.Max(0.01f, warningDuration);
        timer = 0f;
        baseScale = Vector3.one * Mathf.Max(0.1f, radius * 2f);

        EnsureRenderer();
        transform.localScale = baseScale;
        spriteRenderer.color = warningColor;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        float progress = Mathf.Clamp01(timer / duration);
        float pulse = 1f + Mathf.Sin(progress * Mathf.PI * 6f) * 0.08f * pulseScale;
        transform.localScale = baseScale * pulse;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.Lerp(warningColor, finalColor, progress);
        }

        if (timer >= duration)
        {
            Destroy(gameObject);
        }
    }

    private void EnsureRenderer()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

        if (spriteRenderer.sprite == null)
        {
            spriteRenderer.sprite = GetDefaultSprite();
        }

        spriteRenderer.sortingOrder = 20;
    }

    private static Sprite GetDefaultSprite()
    {
        if (defaultSprite == null)
        {
            Texture2D texture = Texture2D.whiteTexture;
            defaultSprite = Sprite.Create(
                texture,
                new Rect(0f, 0f, texture.width, texture.height),
                new Vector2(0.5f, 0.5f),
                texture.width);
        }

        return defaultSprite;
    }
}
