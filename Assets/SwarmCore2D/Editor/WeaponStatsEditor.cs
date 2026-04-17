using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WeaponStats))]
public class WeaponStatsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var stats = (WeaponStats)target;
        var attackType = (WeaponStats.AttackType)serializedObject.FindProperty("attackType").enumValueIndex;

        bool isMelee      = attackType == WeaponStats.AttackType.Melee;
        bool isProjectile = attackType == WeaponStats.AttackType.Projectile;
        bool isArea       = attackType == WeaponStats.AttackType.Area;
        bool isOrbit      = attackType == WeaponStats.AttackType.Orbit;

        // ── Attack Type ──────────────────────────────────────────────────
        EditorGUILayout.PropertyField(serializedObject.FindProperty("attackType"));
        EditorGUILayout.Space(4);

        // ── Base Stats (always) ──────────────────────────────────────────
        DrawHeader("Base Stats");
        EditorGUILayout.PropertyField(serializedObject.FindProperty("damage"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("cooldown"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("amount"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("knockback"));

        // ── Projectile ───────────────────────────────────────────────────
        if (isProjectile)
        {
            DrawHeader("Projectile");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("projectileSpeed"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("projectileSize"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("maxRange"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("spreadAngle"));
        }

        // ── Area / Orbit visual size ─────────────────────────────────────
        if (isOrbit)
        {
            DrawHeader("Orbit");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("projectileSize"),
                new GUIContent("Orb Visual Size"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("hitRadius"),
                new GUIContent("Orb Hit Radius"));
        }

        if (isMelee || isArea)
        {
            DrawHeader(isMelee ? "Melee" : "Area");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("hitRadius"));
            if (isArea || isMelee)
                EditorGUILayout.PropertyField(serializedObject.FindProperty("attackAngle"));
        }

        // ── Duration ─────────────────────────────────────────────────────
        if (isProjectile || isArea || isOrbit)
        {
            DrawHeader("Duration");
            string label = isProjectile ? "Projectile Lifetime" : isOrbit ? "Active Duration" : "Effect Duration";
            EditorGUILayout.PropertyField(serializedObject.FindProperty("effectDuration"),
                new GUIContent(label));
        }

        // ── Pierce ───────────────────────────────────────────────────────
        if (isMelee || isProjectile)
        {
            DrawHeader("Pierce");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("pierceCount"));
        }

        // ── Directions ───────────────────────────────────────────────────
        if (isMelee || isProjectile)
        {
            DrawHeader("Directions");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("attackUp"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("attackDown"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("attackLeft"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("attackRight"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("allowDiagonals"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("directionMode"));
        }

        // ── Scaling Flags (always) ───────────────────────────────────────
        DrawHeader("Scaling Flags");
        EditorGUILayout.PropertyField(serializedObject.FindProperty("scaledByMight"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("scaledByArea"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("scaledBySpeed"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("scaledByDuration"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("scaledByAmount"));

        // ── Visual (always) ──────────────────────────────────────────────
        DrawHeader("Visual");
        EditorGUILayout.PropertyField(serializedObject.FindProperty("weaponIcon"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("attackVisualPrefab"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("frames"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("frameRate"));

        // ── Upgrades ─────────────────────────────────────────────────────
        DrawHeader("Upgrades");
        EditorGUILayout.PropertyField(serializedObject.FindProperty("allowedWeaponUpgrades"),
            new GUIContent("Allowed Upgrade Types"), true);

        EditorGUILayout.Space(4);
        if (GUILayout.Button("Set Defaults for " + attackType))
            SetDefaultUpgrades(stats, attackType);

        serializedObject.ApplyModifiedProperties();
    }

    static void DrawHeader(string label)
    {
        EditorGUILayout.Space(6);
        EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
    }

    static void SetDefaultUpgrades(WeaponStats stats, WeaponStats.AttackType type)
    {
        Undo.RecordObject(stats, "Set Default Upgrades");

        stats.allowedWeaponUpgrades = type switch
        {
            WeaponStats.AttackType.Melee => new List<UpgradeData.UpgradeType>
            {
                UpgradeData.UpgradeType.Damage,
                UpgradeData.UpgradeType.AttackSpeed,
                UpgradeData.UpgradeType.Size,
                UpgradeData.UpgradeType.Knockback,
                UpgradeData.UpgradeType.Pierce,
                UpgradeData.UpgradeType.AddDirectionRandom,
            },
            WeaponStats.AttackType.Projectile => new List<UpgradeData.UpgradeType>
            {
                UpgradeData.UpgradeType.Damage,
                UpgradeData.UpgradeType.AttackSpeed,
                UpgradeData.UpgradeType.ProjectileAmount,
                UpgradeData.UpgradeType.Size,
                UpgradeData.UpgradeType.Knockback,
                UpgradeData.UpgradeType.Pierce,
                UpgradeData.UpgradeType.AddDirectionRandom,
            },
            WeaponStats.AttackType.Area => new List<UpgradeData.UpgradeType>
            {
                UpgradeData.UpgradeType.Damage,
                UpgradeData.UpgradeType.AttackSpeed,
                UpgradeData.UpgradeType.Size,
                UpgradeData.UpgradeType.Knockback,
            },
            WeaponStats.AttackType.Orbit => new List<UpgradeData.UpgradeType>
            {
                UpgradeData.UpgradeType.Damage,
                UpgradeData.UpgradeType.AttackSpeed,
                UpgradeData.UpgradeType.ProjectileAmount,
                UpgradeData.UpgradeType.Size,
                UpgradeData.UpgradeType.Knockback,
            },
            _ => new List<UpgradeData.UpgradeType>()
        };

        EditorUtility.SetDirty(stats);
    }
}
