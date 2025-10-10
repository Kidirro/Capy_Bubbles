using UnityEditor;
using TMPro;
using UnityEngine;


[InitializeOnLoad]
public static class TMPFixer
{
    [MenuItem("Tools/Fix TMP Materials")]
    static void FixTMP()
    {
        var texts = GameObject.FindObjectsOfType<TextMeshProUGUI>(true);
        foreach (var t in texts)
        {
            if (t.font != null)
            {
                t.fontMaterial = t.font.material;
                EditorUtility.SetDirty(t);
            }
        }
        Debug.Log("TMP Materials restored");
    }
}