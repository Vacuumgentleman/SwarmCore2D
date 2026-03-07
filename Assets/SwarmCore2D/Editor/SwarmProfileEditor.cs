using UnityEditor;
using UnityEngine;
using SwarmCore2D.ScriptableObjects;

namespace SwarmCore2D.Editor
{
    [CustomEditor(typeof(SwarmProfile))]
    public class SwarmProfileEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            SwarmProfile profile = (SwarmProfile)target;

            EditorGUILayout.Space();

            EditorGUILayout.LabelField(
                "SwarmCore2D Simulation Profile",
                EditorStyles.boldLabel
            );

            EditorGUILayout.HelpBox(
                "This profile controls the core simulation parameters of the swarm system.",
                MessageType.Info
            );

            EditorGUILayout.Space();

            DrawDefaultInspector();

            EditorGUILayout.Space();

            DrawPerformanceInfo(profile);

            EditorGUILayout.Space();

            if (GUILayout.Button("Open Documentation"))
            {
                DocumentationLink.OpenDocs();
            }
        }

        void DrawPerformanceInfo(SwarmProfile profile)
        {
            EditorGUILayout.LabelField(
                "Estimated Performance",
                EditorStyles.boldLabel
            );

            int entities = profile.maxEntities;
            float tickRate = profile.tickRate;

            string estimate;

            if (entities <= 500)
                estimate = "Low load – Mobile/WebGL friendly";
            else if (entities <= 1500)
                estimate = "Medium load – Desktop/WebGL";
            else
                estimate = "High load – Desktop recommended";

            EditorGUILayout.HelpBox(
                $"Entities: {entities}\nTick Rate: {tickRate}\nPerformance Tier: {estimate}",
                MessageType.None
            );
        }
    }
}