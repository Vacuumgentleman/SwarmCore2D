using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(TierDefinition))]
public class TierDefinitionDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Replace "Element 0" → "Tier 0"
        string text = label.text;
        if (text.StartsWith("Element "))
            label.text = "Tier " + text.Substring("Element ".Length);

        EditorGUI.PropertyField(position, property, label, true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }
}
