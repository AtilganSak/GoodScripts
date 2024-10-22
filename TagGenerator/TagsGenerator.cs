using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace _GameAssets.Scripts.Editor
{
    public class TagsGenerator : EditorWindow
    {
        [MenuItem("Tools/Generate Tags")]
        private static void ShowWindow()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("public static class Tags");
            stringBuilder.AppendLine("{");

            // Get all tags in the project
            string[] tags = UnityEditorInternal.InternalEditorUtility.tags;

            // Add each tag as a public static string variable
            foreach (string tag in tags)
            {
                // Ensure tag name is a valid C# identifier
                string cleanedTag = CleanTagName(tag);
                stringBuilder.AppendLine($"    public static string {cleanedTag} = \"{tag}\";");
            }

            stringBuilder.AppendLine("}");

            // Write to a C# file under Assets folder
            string filePath = "Assets/_GameAssets/Scripts/Tags.cs";
            File.WriteAllText(filePath, stringBuilder.ToString());

            // Refresh Unity to compile the new script
            AssetDatabase.Refresh();

        }

        static string CleanTagName(string tagName)
        {
            // Ensure tag name is a valid C# identifier
            string cleanedName = tagName.Replace(" ", "_").Replace("-", "_");
            // Check if first character is a digit, prepend underscore if true
            if (char.IsDigit(cleanedName[0]))
            {
                cleanedName = "_" + cleanedName;
            }
            return cleanedName;
        }
    }
}