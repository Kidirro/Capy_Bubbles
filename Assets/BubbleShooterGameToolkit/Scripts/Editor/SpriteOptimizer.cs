using UnityEngine;
using UnityEditor;
using System.IO;

public class SpriteResizerEditor : EditorWindow
{
    private int reductionFactor = 2; // Во сколько раз уменьшаем (N)
    private bool makePowerOfTwo = true; // Делать ли размер кратным 2

    [MenuItem("Tools/Resize Sprites (Reduce xN)")]
    public static void ShowWindow()
    {
        GetWindow<SpriteResizerEditor>("Sprite Resizer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Уменьшение спрайтов в N раз", EditorStyles.boldLabel);
        
        reductionFactor = EditorGUILayout.IntField("Уменьшить в раз:", reductionFactor);
        makePowerOfTwo = EditorGUILayout.Toggle("Кратно 2", makePowerOfTwo);
        
        if (GUILayout.Button("Применить ко всем спрайтам"))
        {
            ResizeAllSprites();
        }
    }

    private void ResizeAllSprites()
    {
        if (reductionFactor <= 0)
        {
            Debug.LogError("Коэффициент уменьшения должен быть > 0!");
            return;
        }

        string[] allSpritePaths = Directory.GetFiles("Assets", "*.png", SearchOption.AllDirectories);
        int processedCount = 0;

        foreach (string spritePath in allSpritePaths)
        {
            TextureImporter importer = AssetImporter.GetAtPath(spritePath) as TextureImporter;
            if (importer == null || importer.textureType != TextureImporterType.Sprite) continue;

            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(spritePath);
            if (texture == null) continue;

            // Рассчитываем новый размер
            int newWidth = texture.width / reductionFactor;
            int newHeight = texture.height / reductionFactor;

            // Приводим к степени двойки (если включено)
            if (makePowerOfTwo)
            {
                newWidth = ToNearestPowerOfTwo(newWidth);
                newHeight = ToNearestPowerOfTwo(newHeight);
            }

            // Устанавливаем максимальный размер
            importer.maxTextureSize = Mathf.Max(newWidth, newHeight);
            
            // Форсируем пересжатие
            importer.textureCompression = TextureImporterCompression.Compressed;
            importer.compressionQuality = 50;

            EditorUtility.SetDirty(importer);
            importer.SaveAndReimport();

            processedCount++;
            Debug.Log($"Уменьшен: {spritePath} -> {newWidth}x{newHeight}");
        }

        AssetDatabase.Refresh();
        Debug.Log($"Готово! Обработано спрайтов: {processedCount}");
    }

    // Приведение к ближайшей степени двойки (в меньшую сторону)
    private int ToNearestPowerOfTwo(int value)
    {
        int po2 = 1;
        while (po2 * 2 <= value)
        {
            po2 *= 2;
        }
        return po2;
    }
}