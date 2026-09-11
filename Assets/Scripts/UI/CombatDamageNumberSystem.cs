using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class CombatDamageNumberSystem : MonoBehaviour
{
    private const int MaxRetainedNumbers = 128;

    private static readonly Color PlayerDamageColor = new Color32(255, 70, 70, 255);
    private static readonly Color EnemyDamageColor = new Color32(255, 255, 255, 255);
    private static readonly Color CriticalDamageColor = new Color32(255, 214, 48, 255);

    private static CombatDamageNumberSystem instance;

    private readonly Queue<CombatDamageNumberView> inactiveNumbers =
        new Queue<CombatDamageNumberView>();

    private RectTransform numberRoot;
    private Camera worldCamera;
    private TMP_FontAsset damageNumberFont;
    private Material damageNumberMaterial;

    public static void ShowPlayerDamage(Vector3 worldPosition, int damage)
    {
        if (damage <= 0 || !Application.isPlaying)
        {
            return;
        }

        EnsureInstance()?.Show(worldPosition, damage, PlayerDamageColor, false);
    }

    public static void ShowEnemyDamage(Vector3 worldPosition, float damage, bool isCritical)
    {
        if (damage <= 0f || !Application.isPlaying)
        {
            return;
        }

        int displayedDamage = Mathf.Max(1, Mathf.RoundToInt(damage));
        EnsureInstance()?.Show(
            worldPosition,
            displayedDamage,
            isCritical ? CriticalDamageColor : EnemyDamageColor,
            isCritical);
    }

    private static CombatDamageNumberSystem EnsureInstance()
    {
        if (instance != null)
        {
            return instance;
        }

        instance = FindObjectOfType<CombatDamageNumberSystem>();
        if (instance != null)
        {
            return instance;
        }

        var root = new GameObject("[Combat Damage Numbers]");
        instance = root.AddComponent<CombatDamageNumberSystem>();
        return instance;
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        BuildCanvas();
    }

    private void OnEnable()
    {
        if (instance == null)
        {
            instance = this;
        }

        if (instance != this)
        {
            return;
        }

        BuildCanvas();
        ClearExistingNumberViews();
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }

        if (damageNumberMaterial != null)
        {
            Destroy(damageNumberMaterial);
            damageNumberMaterial = null;
        }

        inactiveNumbers.Clear();
    }

    private void BuildCanvas()
    {
        if (numberRoot != null)
        {
            return;
        }

        Canvas canvas = GetComponentInChildren<Canvas>(true);
        GameObject canvasObject;
        if (canvas == null)
        {
            canvasObject = new GameObject(
                "Damage Number Canvas",
                typeof(RectTransform),
                typeof(Canvas),
                typeof(CanvasScaler));
            canvasObject.transform.SetParent(transform, false);
            canvas = canvasObject.GetComponent<Canvas>();
        }
        else
        {
            canvasObject = canvas.gameObject;
        }

        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        // 保持在世界画面之上、暂停和商店等正式 UI 之下。
        canvas.sortingOrder = -100;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        if (scaler == null)
        {
            scaler = canvasObject.AddComponent<CanvasScaler>();
        }

        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        numberRoot = canvasObject.GetComponent<RectTransform>();
        BuildTextResources();
    }

    private void ClearExistingNumberViews()
    {
        CombatDamageNumberView[] existingViews =
            GetComponentsInChildren<CombatDamageNumberView>(true);
        foreach (CombatDamageNumberView view in existingViews)
        {
            if (view != null)
            {
                Destroy(view.gameObject);
            }
        }

        inactiveNumbers.Clear();
    }

    private void BuildTextResources()
    {
        damageNumberFont = Resources.Load<TMP_FontAsset>(
            "Fonts & Materials/ShanHaiNiuNaiBoBoW-2 SDF");
        if (damageNumberFont == null)
        {
            damageNumberFont = TMP_Settings.defaultFontAsset;
        }

        Material sourceMaterial = damageNumberFont != null ? damageNumberFont.material : null;
        if (sourceMaterial == null)
        {
            return;
        }

        damageNumberMaterial = new Material(sourceMaterial)
        {
            name = "Combat Damage Number Material"
        };
        if (damageNumberMaterial.HasProperty("_OutlineColor"))
        {
            damageNumberMaterial.SetColor("_OutlineColor", new Color32(0, 0, 0, 220));
        }

        if (damageNumberMaterial.HasProperty("_OutlineWidth"))
        {
            damageNumberMaterial.SetFloat("_OutlineWidth", 0.18f);
        }
    }

    private void Show(Vector3 worldPosition, int damage, Color color, bool isCritical)
    {
        Camera camera = ResolveWorldCamera();
        if (camera == null)
        {
            return;
        }

        CombatDamageNumberView view = GetNumberView();
        view.Show(camera, worldPosition, damage, color, isCritical);
    }

    private Camera ResolveWorldCamera()
    {
        if (worldCamera == null)
        {
            worldCamera = Camera.main;
            if (worldCamera == null)
            {
                worldCamera = FindObjectOfType<Camera>();
            }
        }

        return worldCamera;
    }

    private CombatDamageNumberView GetNumberView()
    {
        while (inactiveNumbers.Count > 0)
        {
            CombatDamageNumberView pooledView = inactiveNumbers.Dequeue();
            if (pooledView != null)
            {
                return pooledView;
            }
        }

        var numberObject = new GameObject("Damage Number", typeof(RectTransform));
        numberObject.SetActive(false);
        numberObject.transform.SetParent(numberRoot, false);

        RectTransform rectTransform = numberObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(220f, 90f);

        TextMeshProUGUI label = numberObject.AddComponent<TextMeshProUGUI>();
        if (damageNumberFont != null)
        {
            label.font = damageNumberFont;
        }

        if (damageNumberMaterial != null)
        {
            label.fontSharedMaterial = damageNumberMaterial;
        }

        label.alignment = TextAlignmentOptions.Center;
        label.enableAutoSizing = false;
        label.enableWordWrapping = false;
        label.fontStyle = FontStyles.Bold;
        label.overflowMode = TextOverflowModes.Overflow;
        label.raycastTarget = false;

        CombatDamageNumberView view = numberObject.AddComponent<CombatDamageNumberView>();
        view.Configure(this, rectTransform, label);
        return view;
    }

    internal void Release(CombatDamageNumberView view)
    {
        if (view == null)
        {
            return;
        }

        view.PrepareForPool(numberRoot);
        if (inactiveNumbers.Count < MaxRetainedNumbers)
        {
            inactiveNumbers.Enqueue(view);
        }
        else
        {
            Destroy(view.gameObject);
        }
    }
}

public sealed class CombatDamageNumberView : MonoBehaviour
{
    private CombatDamageNumberSystem owner;
    private RectTransform rectTransform;
    private TextMeshProUGUI label;
    private Camera worldCamera;
    private Vector3 worldPosition;
    private Vector3 worldVelocity;
    private Color baseColor;
    private float elapsed;
    private float lifetime;
    private bool isCritical;
    private bool playing;

    internal void Configure(
        CombatDamageNumberSystem newOwner,
        RectTransform newRectTransform,
        TextMeshProUGUI newLabel)
    {
        owner = newOwner;
        rectTransform = newRectTransform;
        label = newLabel;
    }

    internal void Show(
        Camera camera,
        Vector3 origin,
        int damage,
        Color color,
        bool critical)
    {
        worldCamera = camera;
        worldPosition = origin + new Vector3(Random.Range(-0.18f, 0.18f), 0.7f, 0f);
        worldVelocity = new Vector3(Random.Range(-0.12f, 0.12f), critical ? 1.45f : 1.15f, 0f);
        baseColor = color;
        elapsed = 0f;
        lifetime = critical ? 1f : 0.85f;
        isCritical = critical;
        playing = true;

        label.text = damage.ToString();
        label.fontSize = critical ? 44f : 36f;
        label.color = color;
        label.enabled = true;
        rectTransform.localScale = Vector3.one * (critical ? 1.35f : 1f);
        gameObject.SetActive(true);
        RefreshScreenPosition();
    }

    private void Update()
    {
        if (!playing)
        {
            return;
        }

        float deltaTime = Time.deltaTime;
        elapsed += deltaTime;
        worldPosition += worldVelocity * deltaTime;
        RefreshScreenPosition();

        float normalizedTime = Mathf.Clamp01(elapsed / lifetime);
        float fade = normalizedTime <= 0.55f
            ? 1f
            : 1f - (normalizedTime - 0.55f) / 0.45f;
        Color fadedColor = baseColor;
        fadedColor.a *= fade;
        label.color = fadedColor;

        if (isCritical)
        {
            float settle = Mathf.Clamp01(normalizedTime / 0.25f);
            rectTransform.localScale = Vector3.one * Mathf.Lerp(1.35f, 1f, settle);
        }

        if (elapsed >= lifetime)
        {
            playing = false;
            owner.Release(this);
        }
    }

    private void RefreshScreenPosition()
    {
        if (worldCamera == null)
        {
            label.enabled = false;
            return;
        }

        Vector3 screenPosition = worldCamera.WorldToScreenPoint(worldPosition);
        label.enabled = screenPosition.z > 0f;
        rectTransform.position = screenPosition;
    }

    internal void PrepareForPool(Transform poolRoot)
    {
        playing = false;
        worldCamera = null;
        worldPosition = Vector3.zero;
        worldVelocity = Vector3.zero;
        elapsed = 0f;
        lifetime = 0f;
        isCritical = false;
        label.text = string.Empty;
        label.color = Color.white;
        label.enabled = true;
        rectTransform.localScale = Vector3.one;
        transform.SetParent(poolRoot, false);
        gameObject.SetActive(false);
    }
}
