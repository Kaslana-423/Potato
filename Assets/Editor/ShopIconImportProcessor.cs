using UnityEditor;
using UnityEngine;

public sealed class ShopIconImportProcessor : AssetPostprocessor
{
    private const string ShopIconDirectory = "Assets/Resources/IconImage/Items/";

    private void OnPreprocessTexture()
    {
        if (!assetPath.StartsWith(ShopIconDirectory))
        {
            return;
        }

        var importer = (TextureImporter)assetImporter;
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = false;
        importer.wrapMode = TextureWrapMode.Clamp;
        importer.filterMode = FilterMode.Bilinear;
        importer.textureCompression = TextureImporterCompression.CompressedHQ;
    }
}
