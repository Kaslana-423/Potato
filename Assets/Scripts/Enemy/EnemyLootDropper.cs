using UnityEngine;

public sealed class EnemyLootDropper : MonoBehaviour
{
    [Header("Optional Drop Prefabs")]
    [SerializeField] private FruitPickup fruitPrefab;
    [SerializeField] private LootCratePickup lootCratePrefab;

    [Header("Drop")]
    [SerializeField, Min(0f)] private float scatterRadius = 0.45f;

    private static Sprite whiteSprite;
    private bool droppedThisLife;

    private void OnEnable()
    {
        droppedThisLife = false;
    }

    public void ConfigureDefaults(
        FruitPickup defaultFruitPrefab,
        LootCratePickup defaultLootCratePrefab,
        float defaultScatterRadius,
        bool overwriteDropSettings)
    {
        if (fruitPrefab == null)
        {
            fruitPrefab = defaultFruitPrefab;
        }

        if (lootCratePrefab == null)
        {
            lootCratePrefab = defaultLootCratePrefab;
        }

        if (overwriteDropSettings)
        {
            scatterRadius = Mathf.Max(0f, defaultScatterRadius);
        }
    }

    public void DropLoot(float fruitChance, float lootCrateChance)
    {
        if (droppedThisLife)
        {
            return;
        }

        droppedThisLife = true;
        if (Random.value < Mathf.Clamp01(fruitChance))
        {
            SpawnFruit();
        }

        if (Random.value < Mathf.Clamp01(lootCrateChance))
        {
            SpawnLootCrate();
        }
    }

    private void SpawnFruit()
    {
        Vector3 position = GetDropPosition();
        if (fruitPrefab != null)
        {
            Instantiate(fruitPrefab, position, Quaternion.identity);
            return;
        }

        GameObject fruitObject = CreateRuntimeDrop(
            "Fruit",
            position,
            new Color(0.92f, 0.18f, 0.22f, 1f),
            0.42f);
        fruitObject.AddComponent<FruitPickup>();
    }

    private void SpawnLootCrate()
    {
        Vector3 position = GetDropPosition();
        if (lootCratePrefab != null)
        {
            Instantiate(lootCratePrefab, position, Quaternion.identity);
            return;
        }

        GameObject crateObject = CreateRuntimeDrop(
            "Loot Crate",
            position,
            new Color(0.68f, 0.39f, 0.13f, 1f),
            0.58f);
        crateObject.AddComponent<LootCratePickup>();

        GameObject band = CreateRuntimeDrop(
            "Band",
            position,
            new Color(0.96f, 0.75f, 0.2f, 1f),
            0.16f);
        band.transform.SetParent(crateObject.transform, false);
        band.transform.localPosition = Vector3.zero;
    }

    private Vector3 GetDropPosition()
    {
        Vector2 offset = scatterRadius > 0f ? Random.insideUnitCircle * scatterRadius : Vector2.zero;
        return transform.position + (Vector3)offset;
    }

    private static GameObject CreateRuntimeDrop(string objectName, Vector3 position, Color color, float scale)
    {
        GameObject dropObject = new GameObject(objectName);
        dropObject.transform.position = position;
        dropObject.transform.localScale = Vector3.one * scale;

        SpriteRenderer renderer = dropObject.AddComponent<SpriteRenderer>();
        renderer.sprite = GetWhiteSprite();
        renderer.color = color;
        renderer.sortingLayerName = "Bullet";
        renderer.sortingOrder = 1;
        return dropObject;
    }

    private static Sprite GetWhiteSprite()
    {
        if (whiteSprite == null)
        {
            Texture2D texture = Texture2D.whiteTexture;
            whiteSprite = Sprite.Create(
                texture,
                new Rect(0f, 0f, texture.width, texture.height),
                new Vector2(0.5f, 0.5f),
                texture.width);
        }

        return whiteSprite;
    }
}
