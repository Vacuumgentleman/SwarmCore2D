using UnityEngine;
using UnityEditor;

namespace GabrielBissonnette.Primo
{
    [InitializeOnLoad]
    public static class HierarchySectionHeader
    {
        static HierarchySectionHeader()
        {
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
        }

        static void HierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {
            var obj = EditorUtility.InstanceIDToObject(instanceID);

            if (obj == null)
                return;

            GameObject gameObject = obj as GameObject;

            if (gameObject == null)
                return;

            if (gameObject.name.StartsWith("//", System.StringComparison.Ordinal))
            {
                EditorGUI.DrawRect(selectionRect, Color.black);
                EditorGUI.DropShadowLabel(
                    selectionRect,
                    gameObject.name.Replace("/", "").ToUpperInvariant()
                );
            }
        }
    }
}