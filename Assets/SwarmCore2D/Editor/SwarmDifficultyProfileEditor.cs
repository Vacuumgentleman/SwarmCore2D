using UnityEditor;
using UnityEngine;
using SwarmCore2D.ScriptableObjects;

namespace SwarmCore2D.Editor
{
    [CustomEditor(typeof(SwarmDifficultyProfile))]
    public class SwarmDifficultyProfileEditor : UnityEditor.Editor
    {
        SerializedProperty phasesProp;
        SerializedProperty healthCurveProp;
        SerializedProperty speedCurveProp;

        bool[] foldouts = new bool[0];

        void OnEnable()
        {
            phasesProp     = serializedObject.FindProperty("phases");
            healthCurveProp = serializedObject.FindProperty("healthMultiplier");
            speedCurveProp  = serializedObject.FindProperty("speedMultiplier");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // ── Spawn Phases ─────────────────────────────────────────────
            EditorGUILayout.Space(4);
            EditorGUILayout.LabelField("Spawn Phases", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Cada fase se activa cuando el tiempo de partida supera su startTime.\n" +
                "Si enemyPool está vacío se usan todos los enemigos de EnemyDatabase.",
                MessageType.None
            );
            EditorGUILayout.Space(2);

            // Sync foldout array size
            if (foldouts.Length != phasesProp.arraySize)
            {
                var prev = foldouts;
                foldouts = new bool[phasesProp.arraySize];
                for (int i = 0; i < Mathf.Min(prev.Length, foldouts.Length); i++)
                    foldouts[i] = prev[i];
            }

            for (int i = 0; i < phasesProp.arraySize; i++)
            {
                SerializedProperty phase = phasesProp.GetArrayElementAtIndex(i);

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                EditorGUILayout.BeginHorizontal();
                foldouts[i] = EditorGUILayout.Foldout(foldouts[i], $"Phase {i}", true, EditorStyles.foldoutHeader);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("✕", GUILayout.Width(22)))
                {
                    phasesProp.DeleteArrayElementAtIndex(i);
                    serializedObject.ApplyModifiedProperties();
                    return;
                }
                EditorGUILayout.EndHorizontal();

                if (foldouts[i])
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(phase.FindPropertyRelative("startTime"));
                    EditorGUILayout.PropertyField(phase.FindPropertyRelative("spawnInterval"));
                    EditorGUILayout.PropertyField(phase.FindPropertyRelative("enemyPool"), true);
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(2);
            }

            if (GUILayout.Button("+ Add Phase"))
            {
                phasesProp.InsertArrayElementAtIndex(phasesProp.arraySize);

                // Initialize new phase with sensible defaults
                var newPhase = phasesProp.GetArrayElementAtIndex(phasesProp.arraySize - 1);
                newPhase.FindPropertyRelative("startTime").floatValue =
                    phasesProp.arraySize > 1
                        ? phasesProp.GetArrayElementAtIndex(phasesProp.arraySize - 2)
                               .FindPropertyRelative("startTime").floatValue + 60f
                        : 0f;
                newPhase.FindPropertyRelative("spawnInterval").floatValue = 2f;
            }

            // ── Stat Scaling Curves ───────────────────────────────────────
            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("Stat Scaling Over Time", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Eje X = segundos de partida.  Eje Y = multiplicador (1 = sin cambio).\n" +
                "Se aplica a todos los enemigos activos en tiempo real.",
                MessageType.None
            );
            EditorGUILayout.Space(2);
            EditorGUILayout.PropertyField(healthCurveProp, new GUIContent("Health Multiplier"));
            EditorGUILayout.PropertyField(speedCurveProp,  new GUIContent("Speed Multiplier"));

            serializedObject.ApplyModifiedProperties();
        }
    }
}
