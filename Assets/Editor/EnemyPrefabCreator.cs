using System.IO;
using UnityEditor;
using UnityEngine;

public static class EnemyPrefabCreator
{
    private const string EnemyPrefabDirectory = "Assets/Prefebs/Enemy";
    private const string EnemySpriteDirectory = "Assets/Resources/Enemy";

    [MenuItem("Tools/Potato Enemies/Create Basic Enemy Prefabs")]
    public static void CreateBasicEnemyPrefabs()
    {
        Directory.CreateDirectory(EnemyPrefabDirectory);
        Directory.CreateDirectory(EnemySpriteDirectory);

        CreateEnemyPrefab(
            "BabyAlien",
            "enemy.baby_alien",
            new Color(0.95f, 0.28f, 0.2f, 1f),
            0.55f,
            0.42f);

        CreateEnemyPrefab(
            "Chaser",
            "enemy.chaser",
            new Color(0.95f, 0.55f, 0.1f, 1f),
            0.48f,
            0.36f);

        CreateEnemyPrefab(
            "Spitter",
            "enemy.spitter",
            new Color(0.25f, 0.85f, 0.35f, 1f),
            0.68f,
            0.48f);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Potato Enemies", "Created 3 basic enemy prefabs.", "OK");
    }

    private static void CreateEnemyPrefab(
        string prefabName,
        string enemyId,
        Color color,
        float visualScale,
        float colliderRadius)
    {
        GameObject enemyObject = new GameObject(prefabName);
        try
        {
            Sprite sprite = CreateSpriteAsset(prefabName, color);

            enemyObject.layer = 0;
            enemyObject.transform.localScale = Vector3.one * visualScale;

            SpriteRenderer spriteRenderer = enemyObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;
            spriteRenderer.color = color;
            spriteRenderer.sortingOrder = 5;

            Rigidbody2D rb = enemyObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.freezeRotation = true;

            CircleCollider2D collider = enemyObject.AddComponent<CircleCollider2D>();
            collider.radius = colliderRadius;

            enemyObject.AddComponent<EnemyBase>();
            enemyObject.AddComponent<EnemyChaseAI>();

            string prefabPath = $"{EnemyPrefabDirectory}/{prefabName}.prefab";
            PrefabUtility.SaveAsPrefabAsset(enemyObject, prefabPath);
            Debug.Log($"Created enemy prefab for {enemyId}: {prefabPath}");
        }
        finally
        {
            Object.DestroyImmediate(enemyObject);
        }
    }

    private static Sprite CreateSpriteAsset(string assetName, Color color)
    {
        string texturePath = $"{EnemySpriteDirectory}/{assetName}.png";
        var texture = new Texture2D(16, 16, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[16 * 16];
        for (int index = 0; index < pixels.Length; index++)
        {
            pixels[index] = Color.white;
        }

        texture.SetPixels(pixels);
        texture.Apply();
        File.WriteAllBytes(texturePath, texture.EncodeToPNG());
        Object.DestroyImmediate(texture);

        AssetDatabase.ImportAsset(texturePath, ImportAssetOptions.ForceUpdate);
        TextureImporter importer = AssetImporter.GetAtPath(texturePath) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaIsTransparency = true;
            importer.filterMode = FilterMode.Point;
            importer.SaveAndReimport();
        }

        return AssetDatabase.LoadAssetAtPath<Sprite>(texturePath);
    }
}
