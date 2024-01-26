using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(NamedArrayAttribute))]
public class NamedArrayDrawer : PropertyDrawer
{
    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
        try
        {
            int pos = int.Parse(property.propertyPath.Split('[', ']')[1]);
            string name = (Necessary.ScoreNames[pos]);
            if (name == "")
                name = "First";
            EditorGUI.PropertyField(rect, property, new GUIContent(name));
        }
        catch
        {
            EditorGUI.PropertyField(rect, property, label);
        }
    }
}