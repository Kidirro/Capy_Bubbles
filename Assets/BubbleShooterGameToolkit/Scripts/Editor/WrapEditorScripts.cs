#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class WrapEditorScripts
{
    [MenuItem("Tools/Editor Utils/Wrap Editor Scripts with UNITY_EDITOR (Deep Scan)")]
    public static void WrapAllEditorScripts()
    {
        string projectPath = Application.dataPath;
        string[] allCsFiles = Directory.GetFiles(projectPath, "*.cs", SearchOption.AllDirectories);

        int modifiedCount = 0;
        int skippedCount = 0;

        foreach (string file in allCsFiles)
        {
            string text = File.ReadAllText(file);

            // Условие: файл находится в Editor или использует признаки Editor API
            bool isEditorFile =
                file.Contains("/Editor/") ||
                text.Contains("UnityEditor") ||
                text.Contains("EditorGUI") ||
                text.Contains("EditorGUILayout") ||
                text.Contains("EditorWindow") ||
                text.Contains("MenuItem") ||
                text.Contains("GUILayout") || // для твоего случая
                text.Contains("EditorUtility") ||
                text.Contains("EditorScr");

            if (!isEditorFile)
            {
                skippedCount++;
                continue;
            }

            // Уже защищён?
            if (text.Contains("#if UNITY_EDITOR"))
            {
                skippedCount++;
                continue;
            }

            // Создаём бэкап
            // string backup = file + ".bak";
            // if (!File.Exists(backup))
            //     File.WriteAllText(backup, text);

            // Оборачиваем
            string wrapped = "#if UNITY_EDITOR\n" + text + "\n#endif\n";
            File.WriteAllText(file, wrapped);

            modifiedCount++;
        }

        AssetDatabase.Refresh();
        Debug.Log($"✅ Обёрнуто файлов: {modifiedCount}, пропущено: {skippedCount}");
    }
}
#endif