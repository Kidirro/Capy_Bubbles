// Assets/Editor/TMPFontAssetFixer.cs
using UnityEngine;
using UnityEditor;
using TMPro;
using System.IO;

public class TMPFontAssetFixer
{
    [MenuItem("Tools/TMP - Fix missing font materials")]
    public static void FixAllTmpFontAssets()
    {
        if (!EditorUtility.DisplayDialog(
            "Fix TMP Font Assets",
            "This will search your project for TMP_FontAsset assets and create/assign missing materials. Make a backup before continuing. Proceed?",
            "Yes", "Cancel"))
            return;

        string[] guids = AssetDatabase.FindAssets("t:TMP_FontAsset");
        int fontsFixed = 0, materialsCreated = 0;

        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            TMP_FontAsset font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);
            if (font == null) continue;

            EditorUtility.DisplayProgressBar("Fixing TMP font assets", $"Processing {font.name} ({i+1}/{guids.Length})", (float)i / guids.Length);

            if (font.material == null)
            {
                Texture2D atlas = GetAtlasTexture(font);
                Shader shader = FindTMPShader();
                if (shader == null) shader = Shader.Find("Sprites/Default");

                Material mat = new Material(shader)
                {
                    name = font.name + "_TMP_Material"
                };
                if (atlas != null) mat.mainTexture = atlas;

                string folder = Path.GetDirectoryName(path);
                if (string.IsNullOrEmpty(folder)) folder = "Assets";
                string matPath = AssetDatabase.GenerateUniqueAssetPath(folder + "/" + mat.name + ".mat");
                AssetDatabase.CreateAsset(mat, matPath);

                // assign and mark dirty
                font.material = mat;
                EditorUtility.SetDirty(font);

                fontsFixed++;
                materialsCreated++;
                Debug.Log($"[TMPFixer] Created material for {font.name} -> {matPath}");
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();

        // Fix components in open scenes
        int updatedComponents = 0;
        var sceneTexts = Object.FindObjectsOfType<TextMeshProUGUI>(true);
        foreach (var t in sceneTexts)
        {
            if (t.font != null && t.font.material != null)
            {
                if (t.fontMaterial != t.font.material)
                {
                    t.fontMaterial = t.font.material;
                    EditorUtility.SetDirty(t);
                    updatedComponents++;
                }
            }
        }
        var sceneTexts3D = Object.FindObjectsOfType<TextMeshPro>(true);
        foreach (var t in sceneTexts3D)
        {
            if (t.font != null && t.font.material != null)
            {
                if (t.fontMaterial != t.font.material)
                {
                    t.fontMaterial = t.font.material;
                    EditorUtility.SetDirty(t);
                    updatedComponents++;
                }
            }
        }

        // Fix prefabs in project (safe load / save)
        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");
        int prefabsChanged = 0;
        for (int i = 0; i < prefabGuids.Length; i++)
        {
            string prefabPath = AssetDatabase.GUIDToAssetPath(prefabGuids[i]);
            EditorUtility.DisplayProgressBar("Fixing TMP in prefabs", $"Processing prefab {i+1}/{prefabGuids.Length}", (float)i / prefabGuids.Length);
            GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
            bool changed = false;

            var compsUI = root.GetComponentsInChildren<TextMeshProUGUI>(true);
            foreach (var c in compsUI)
            {
                if (c.font != null && c.font.material != null && c.fontMaterial != c.font.material)
                {
                    c.fontMaterial = c.font.material;
                    changed = true;
                }
            }

            var comps3 = root.GetComponentsInChildren<TextMeshPro>(true);
            foreach (var c in comps3)
            {
                if (c.font != null && c.font.material != null && c.fontMaterial != c.font.material)
                {
                    c.fontMaterial = c.font.material;
                    changed = true;
                }
            }

            if (changed)
            {
                PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
                prefabsChanged++;
            }
            PrefabUtility.UnloadPrefabContents(root);
        }
        EditorUtility.ClearProgressBar();

        Debug.Log($"[TMPFixer] Done. Fonts fixed: {fontsFixed}, Materials created: {materialsCreated}, Scene components updated: {updatedComponents}, Prefabs changed: {prefabsChanged}");
        EditorUtility.DisplayDialog("TMP Fixer", $"Finished.\nFonts fixed: {fontsFixed}\nMaterials created: {materialsCreated}\nScene components updated: {updatedComponents}\nPrefabs changed: {prefabsChanged}", "OK");
    }

    static Texture2D GetAtlasTexture(TMP_FontAsset font)
    {
        // Try several serialized property names to safely get atlas texture across TMP versions
        var so = new SerializedObject(font);
        SerializedProperty prop = so.FindProperty("atlasTexture");
        if (prop == null) prop = so.FindProperty("atlas");
        if (prop == null) prop = so.FindProperty("m_AtlasTexture");
        if (prop != null && prop.objectReferenceValue != null)
            return prop.objectReferenceValue as Texture2D;

        var arr = so.FindProperty("atlasTextures");
        if (arr != null && arr.isArray && arr.arraySize > 0)
        {
            var el = arr.GetArrayElementAtIndex(0);
            if (el != null && el.objectReferenceValue != null)
                return el.objectReferenceValue as Texture2D;
        }

        // fallback: if there is already a material with mainTexture
        if (font.material != null && font.material.mainTexture is Texture2D)
            return (Texture2D)font.material.mainTexture;

        return null;
    }

    static Shader FindTMPShader()
    {
        string[] names = new string[] {
            "TextMeshPro/Distance Field",
            "TextMeshPro/Distance Field (Surface)",
            "TextMeshPro/Distance Field (Mobile)",
            "TextMesh Pro/Distance Field",
            "UI/Default"
        };
        foreach (var n in names)
        {
            Shader s = Shader.Find(n);
            if (s != null) return s;
        }
        return null;
    }
}
