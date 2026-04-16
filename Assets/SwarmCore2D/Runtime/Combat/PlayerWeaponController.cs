using UnityEngine;
using SwarmCore2D.Simulation;
using SwarmCore2D.Core;
using SwarmCore2D.Combat;
using System.Collections.Generic;
using System;

public class PlayerWeaponController : MonoBehaviour
{
    [Header("Weapon Slots")]
    public int maxWeaponSlots = 6;
    public List<WeaponStats> weapons = new List<WeaponStats>();

    [Header("References")]
    public SwarmSimulationController simulation;

    public event Action OnWeaponsChanged;

    List<WeaponRuntimeStats> runtimes = new List<WeaponRuntimeStats>();
    List<float> timers = new List<float>();
    List<List<Vector2>> directionLists = new List<List<Vector2>>();
    List<int> lastDirIndexes = new List<int>();
    List<float[]> orbitAngles = new List<float[]>();

    PlayerMeleeAttackSystem meleeSystem;
    ProjectileSystem projectileSystem;

    void Start()
    {
        if (simulation == null)
            simulation = FindFirstObjectByType<SwarmSimulationController>();

        if (simulation == null)
        {
            Debug.LogError("SwarmSimulationController not found");
            enabled = false;
            return;
        }

        meleeSystem = new PlayerMeleeAttackSystem(simulation.WorldState);
        projectileSystem = simulation.ProjectileSystem;

        InitializeWeapons();
    }

    void InitializeWeapons()
    {
        runtimes.Clear();
        timers.Clear();
        directionLists.Clear();
        lastDirIndexes.Clear();
        orbitAngles.Clear();

        for (int i = 0; i < weapons.Count && i < maxWeaponSlots; i++)
        {
            if (weapons[i] == null)
                continue;

            var runtime = new WeaponRuntimeStats();
            runtime.LoadFrom(weapons[i]);

            runtimes.Add(runtime);
            timers.Add(0f);
            directionLists.Add(BuildDirectionList(runtime));
            lastDirIndexes.Add(0);
            orbitAngles.Add(BuildOrbitAngles(runtime.amount));
        }

        OnWeaponsChanged?.Invoke();
    }

    float[] BuildOrbitAngles(int amount)
    {
        int count = Mathf.Max(1, amount);
        float[] angles = new float[count];
        float step = 360f / count;

        for (int i = 0; i < count; i++)
            angles[i] = i * step;

        return angles;
    }

    List<Vector2> BuildDirectionList(WeaponRuntimeStats runtime)
    {
        var directions = new List<Vector2>();

        if (runtime.attackUp) directions.Add(Vector2.up);

        if (runtime.allowDiagonals && runtime.attackUp && runtime.attackRight)
            directions.Add((Vector2.up + Vector2.right).normalized);

        if (runtime.attackRight) directions.Add(Vector2.right);

        if (runtime.allowDiagonals && runtime.attackDown && runtime.attackRight)
            directions.Add((Vector2.down + Vector2.right).normalized);

        if (runtime.attackDown) directions.Add(Vector2.down);

        if (runtime.allowDiagonals && runtime.attackDown && runtime.attackLeft)
            directions.Add((Vector2.down + Vector2.left).normalized);

        if (runtime.attackLeft) directions.Add(Vector2.left);

        if (runtime.allowDiagonals && runtime.attackUp && runtime.attackLeft)
            directions.Add((Vector2.up + Vector2.left).normalized);

        if (directions.Count == 0)
            directions.Add(Vector2.right);

        return directions;
    }

    void Update()
    {
        if (SwarmTime.Paused)
            return;

        var global = PlayerStats.Instance != null ? PlayerStats.Instance.stats : null;

        for (int i = 0; i < runtimes.Count; i++)
        {
            var runtime = runtimes[i];

            if (runtime.attackType == WeaponStats.AttackType.Orbit)
            {
                UpdateOrbit(i, runtime, global);
                continue;
            }

            timers[i] -= Time.deltaTime;

            if (timers[i] > 0f)
                continue;

            float cooldown = runtime.cooldown;
            if (global != null)
                cooldown *= global.cooldownMultiplier;

            timers[i] = cooldown;

            FireWeapon(i, global);
        }
    }

    void FireWeapon(int index, PlayerStatsRuntime global)
    {
        var runtime = runtimes[index];

        switch (runtime.attackType)
        {
            case WeaponStats.AttackType.Melee:
                FireMelee(index, runtime, global);
                break;

            case WeaponStats.AttackType.Projectile:
                FireProjectile(index, runtime, global);
                break;

            case WeaponStats.AttackType.Area:
                FireArea(index, runtime, global);
                break;
        }
    }

    void FireMelee(int index, WeaponRuntimeStats runtime, PlayerStatsRuntime global)
    {
        Vector2 playerPos = transform.position;

        int total = runtime.amount;
        if (global != null && weapons[index].scaledByAmount)
            total += global.extraProjectiles;

        var directions = directionLists[index];
        int dirCount = directions.Count;

        for (int p = 0; p < total; p++)
        {
            int dirIndex = GetDirIndex(index, runtime, p, dirCount);
            Vector2 dir = directions[dirIndex];

            float dmg = runtime.damage;
            float radius = runtime.hitRadius;
            float knockback = runtime.knockback;

            if (global != null)
            {
                if (weapons[index].scaledByMight) dmg *= global.damageMultiplier;
                if (weapons[index].scaledByArea) radius *= global.areaMultiplier;
            }

            meleeSystem.Attack(playerPos, dir, radius, runtime.attackAngle, dmg, knockback);

            SpawnAttackVisual(playerPos, dir, runtime);
        }
    }

    void FireProjectile(int index, WeaponRuntimeStats runtime, PlayerStatsRuntime global)
    {
        Vector2 playerPos = transform.position;

        int total = runtime.amount;
        if (global != null && weapons[index].scaledByAmount)
            total += global.extraProjectiles;

        var directions = directionLists[index];
        int dirCount = directions.Count;

        for (int p = 0; p < total; p++)
        {
            int dirIndex = GetDirIndex(index, runtime, p, dirCount);
            Vector2 dir = directions[dirIndex];

            float dmg = runtime.damage;
            float size = runtime.projectileSize;
            float speed = runtime.projectileSpeed;
            float duration = runtime.effectDuration;
            float range = runtime.maxRange;
            bool pierce = runtime.pierceCount > 0;

            if (global != null)
            {
                if (weapons[index].scaledByMight) dmg *= global.damageMultiplier;
                if (weapons[index].scaledByArea) size *= global.areaMultiplier;
                if (weapons[index].scaledBySpeed) speed *= global.speedMultiplier;
                if (weapons[index].scaledByDuration) duration *= global.durationMultiplier;
                if (global.pierceBonus > 0) pierce = true;
            }

            projectileSystem.Spawn(playerPos, dir, speed, dmg, duration, range, pierce, size);
        }
    }

    void FireArea(int index, WeaponRuntimeStats runtime, PlayerStatsRuntime global)
    {
        Vector2 playerPos = transform.position;

        float dmg = runtime.damage;
        float radius = runtime.hitRadius;

        if (global != null)
        {
            if (weapons[index].scaledByMight) dmg *= global.damageMultiplier;
            if (weapons[index].scaledByArea) radius *= global.areaMultiplier;
        }

        meleeSystem.Attack(playerPos, Vector2.up, radius, 360f, dmg, runtime.knockback);

        SpawnAttackVisual(playerPos, Vector2.zero, runtime);
    }

    void UpdateOrbit(int index, WeaponRuntimeStats runtime, PlayerStatsRuntime global)
    {
        Vector2 playerPos = transform.position;

        float orbitRadius = runtime.hitRadius;
        float dmg = runtime.damage;
        float hitSize = runtime.projectileSize;
        float orbitSpeed = runtime.projectileSpeed;

        if (global != null)
        {
            if (weapons[index].scaledByMight) dmg *= global.damageMultiplier;
            if (weapons[index].scaledByArea) orbitRadius *= global.areaMultiplier;
            if (weapons[index].scaledBySpeed) orbitSpeed *= global.speedMultiplier;
        }

        var angles = orbitAngles[index];
        int count = Mathf.Min(angles.Length, runtime.amount);

        for (int p = 0; p < count; p++)
        {
            angles[p] += orbitSpeed * Time.deltaTime;
            if (angles[p] >= 360f) angles[p] -= 360f;

            float rad = angles[p] * Mathf.Deg2Rad;
            Vector2 orbitPos = playerPos + new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * orbitRadius;

            meleeSystem.Attack(orbitPos, Vector2.up, hitSize, 360f, dmg, 0f);
        }
    }

    int GetDirIndex(int weaponIndex, WeaponRuntimeStats runtime, int projectileIndex, int dirCount)
    {
        if (runtime.directionMode == WeaponStats.AttackDirectionMode.Clockwise)
            return projectileIndex % dirCount;

        int idx = lastDirIndexes[weaponIndex]++ % dirCount;
        return idx;
    }

    void SpawnAttackVisual(Vector2 playerPos, Vector2 dir, WeaponRuntimeStats runtime)
    {
        if (runtime.attackVisualPrefab == null)
            return;

        float offset = dir == Vector2.zero ? 0f : runtime.hitRadius * 0.5f;
        Vector3 spawnPos = playerPos + dir * offset;
        float angle = dir == Vector2.zero ? 0f : Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        GameObject obj = Instantiate(
            runtime.attackVisualPrefab,
            spawnPos,
            Quaternion.Euler(0, 0, angle)
        );

        AttackVisual visual = obj.GetComponent<AttackVisual>();
        if (visual != null)
            visual.Init(runtime.frames, runtime.frameRate);
    }

    public bool AddWeapon(WeaponStats weapon)
    {
        if (weapon == null)
            return false;

        if (runtimes.Count >= maxWeaponSlots)
            return false;

        weapons.Add(weapon);

        var runtime = new WeaponRuntimeStats();
        runtime.LoadFrom(weapon);

        runtimes.Add(runtime);
        timers.Add(0f);
        directionLists.Add(BuildDirectionList(runtime));
        lastDirIndexes.Add(0);
        orbitAngles.Add(BuildOrbitAngles(runtime.amount));

        OnWeaponsChanged?.Invoke();

        return true;
    }

    public WeaponRuntimeStats GetRuntime(int index)
    {
        if (index < 0 || index >= runtimes.Count)
            return null;

        return runtimes[index];
    }

    public int WeaponCount => runtimes.Count;

    public void RebuildDirections(int index)
    {
        if (index < 0 || index >= runtimes.Count)
            return;

        directionLists[index] = BuildDirectionList(runtimes[index]);
    }

    public void RebuildOrbitAngles(int index)
    {
        if (index < 0 || index >= runtimes.Count)
            return;

        orbitAngles[index] = BuildOrbitAngles(runtimes[index].amount);
    }
}