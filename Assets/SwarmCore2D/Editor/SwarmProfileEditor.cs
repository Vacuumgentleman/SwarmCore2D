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
            EditorGUILayout.LabelField("SwarmCore2D — Simulation Profile", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Fuente única de tick rate y capacidad de entidades.\n" +
                "Asigna este SO en FixedTickBehaviour y SwarmSimulationController.",
                MessageType.Info
            );
            EditorGUILayout.Space();

            DrawDefaultInspector();

            EditorGUILayout.Space();
            DrawPerformanceInfo(profile);
            EditorGUILayout.Space();

            if (GUILayout.Button("Open Documentation"))
                DocumentationLink.OpenDocs();
        }

        void DrawPerformanceInfo(SwarmProfile profile)
        {
            EditorGUILayout.LabelField("Estimated Performance Tier", EditorStyles.boldLabel);

            int entities = profile.maxEntities;
            int tickRate = profile.tickRate;

            string tier;
            MessageType msgType;

            if (entities <= 500)
            {
                tier = "Bajo — apto para móvil y WebGL";
                msgType = MessageType.Info;
            }
            else if (entities <= 1500)
            {
                tier = "Medio — Desktop / WebGL potente";
                msgType = MessageType.Info;
            }
            else
            {
                tier = "Alto — Desktop recomendado";
                msgType = MessageType.Warning;
            }

            EditorGUILayout.HelpBox(
                $"Entidades: {entities}    Tick Rate: {tickRate} Hz\nRendimiento estimado: {tier}",
                msgType
            );
        }
    }
}
