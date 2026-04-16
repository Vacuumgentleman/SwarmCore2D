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
    List<List<GameObject>> orbitVisuals = new List<List<GameObject>>();
    List<bool> orbitIsActive = new List<bool>();
    List<float> orbitPhaseTimers = new List<float>();
    List<float> orbitDamageTimers = new List<float>();

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
        foreach (var list in orbitVisuals)
            foreach (var obj in list)
                if (obj != null) Destroy(obj);

        runtimes.Clear();
        timers.Clear();
        directionLists.Clear();
        lastDirIndexes.Clear();
        orbitAngles.Clear();
        orbitVisuals.Clear();
        orbitIsActive.Clear();
        orbitPhaseTimers.Clear();
        orbitDamageTimers.Clear();

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
            orbitVisuals.Add(SpawnOrbitVisuals(runtime));
            orbitIsActive.Add(runtime.attackType == WeaponStats.AttackType.Orbit);
            orbitPhaseTimers.Add(runtime.effectDuration);
            orbitDamageTimers.Add(0f);
        }

        OnWeaponsChanged?.Invoke();
    }

    List<GameObject> SpawnOrbitVisuals(WeaponRuntimeStats runtime)
    {
        var list = new List<GameObject>();
        if (runtime.attackType != WeaponStats.AttackType.Orbit || runtime.attackVisualPrefab == null)
            return list;

        int count = Mathf.Max(1, runtime.amount);
        for (int i = 0; i < count; i++)
        {
            var obj = Instantiate(runtime.attackVisualPrefab, transform.position, Quaternion.identity);
            if (runtime.frames != null && runtime.frames.Length > 0)
            {
                var visual = obj.GetComponent<AttackVisual>();
                if (visual != null)
                    visual.Init(runtime.frames, runtime.frameRate, loop: true);
            }
            list.Add(obj);
        }

        return list;
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

        // Calcular cuántos proyectiles van a cada dirección para aplicar spread
        int startDir = runtime.directionMode == WeaponStats.AttackDirectionMode.Alternating
            ? lastDirIndexes[index]
            : 0;

        int[] slotCount = new int[dirCount];
        int[] slotOf = new int[total];
        int[] indexInSlot = new int[total];

        for (int p = 0; p < total; p++)
        {
            int slot = (startDir + p) % dirCount;
            slotOf[p] = slot;
            indexInSlot[p] = slotCount[slot]++;
        }

        // Avanzar lastDirIndexes para Alternating
        if (runtime.directionMode == WeaponStats.AttackDirectionMode.Alternating)
            lastDirIndexes[index] += total;

        for (int p = 0; p < total; p++)
        {
            int slot = slotOf[p];
            Vector2 dir = directions[slot];

            int n = slotCount[slot];
            if (n > 1 && runtime.spreadAngle > 0f)
            {
                float totalArc = runtime.spreadAngle * (n - 1);
                float angle = -totalArc * 0.5f + indexInSlot[p] * runtime.spreadAngle;
                dir = RotateDirection(dir, angle);
            }

            projectileSystem.Spawn(playerPos, dir, speed, dmg, duration, range, pierce, size);
        }
    }

    static Vector2 RotateDirection(Vector2 dir, float angleDeg)
    {
        float rad = angleDeg * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);
        return new Vector2(dir.x * cos - dir.y * sin, dir.x * sin + dir.y * cos);
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
        float activeDuration = runtime.effectDuration;
        float cooldownDuration = runtime.cooldown;

        if (global != null)
        {
            if (weapons[index].scaledByMight) dmg *= global.damageMultiplier;
            if (weapons[index].scaledByArea) { orbitRadius *= global.areaMultiplier; hitSize *= global.areaMultiplier; }
            if (weapons[index].scaledBySpeed) orbitSpeed *= global.speedMultiplier;
            if (weapons[index].scaledByDuration) activeDuration *= global.durationMultiplier;
            cooldownDuration *= global.cooldownMultiplier;
        }

        // --- Sincronizar conteo de orbs con runtime.amount ---
        SyncOrbitState(index, runtime);

        // --- Ciclo activo / cooldown ---
        orbitPhaseTimers[index] -= Time.deltaTime;
        if (orbitPhaseTimers[index] <= 0f)
        {
            orbitIsActive[index] = !orbitIsActive[index];
            orbitPhaseTimers[index] = orbitIsActive[index] ? activeDuration : cooldownDuration;
            SetOrbitVisualsVisible(index, orbitIsActive[index]);
        }

        bool isActive = orbitIsActive[index];

        // --- Posiciones orbitales (siempre se actualizan) ---
        var angles = orbitAngles[index];
        var visuals = orbitVisuals[index];
        int count = angles.Length;

        for (int p = 0; p < count; p++)
        {
            angles[p] += orbitSpeed * Time.deltaTime;
            if (angles[p] >= 360f) angles[p] -= 360f;

            float rad = angles[p] * Mathf.Deg2Rad;
            Vector2 orbitPos = playerPos + new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * orbitRadius;

            if (p < visuals.Count && visuals[p] != null)
            {
                visuals[p].transform.position = orbitPos;
                visuals[p].transform.localScale = Vector3.one * hitSize;
            }
        }

        // --- Daño: solo en fase activa, por ticks ---
        if (!isActive) return;

        orbitDamageTimers[index] -= Time.deltaTime;
        if (orbitDamageTimers[index] > 0f) return;

        orbitDamageTimers[index] = 0.3f;

        for (int p = 0; p < count; p++)
        {
            float rad = angles[p] * Mathf.Deg2Rad;
            Vector2 orbitPos = playerPos + new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * orbitRadius;
            meleeSystem.Attack(orbitPos, Vector2.up, hitSize, 360f, dmg, 0f);
        }
    }

    void SyncOrbitState(int index, WeaponRuntimeStats runtime)
    {
        int needed = Mathf.Max(1, runtime.amount);
        var visuals = orbitVisuals[index];
        float[] angles = orbitAngles[index];

        // Sincronizar array de ángulos
        if (angles.Length != needed)
        {
            float baseAngle = angles.Length > 0 ? angles[0] : 0f;
            float step = 360f / needed;
            float[] newAngles = new float[needed];
            for (int i = 0; i < needed; i++)
                newAngles[i] = baseAngle + i * step;
            orbitAngles[index] = newAngles;
        }

        // Spawnear visuals que faltan
        while (visuals.Count < needed)
        {
            if (runtime.attackVisualPrefab == null) { visuals.Add(null); continue; }

            var obj = Instantiate(runtime.attackVisualPrefab, transform.position, Quaternion.identity);

            if (runtime.frames != null && runtime.frames.Length > 0)
            {
                var visual = obj.GetComponent<AttackVisual>();
                if (visual != null)
                    visual.Init(runtime.frames, runtime.frameRate, loop: true);
            }

            if (!orbitIsActive[index])
            {
                var sr = obj.GetComponent<SpriteRenderer>();
                if (sr != null) sr.enabled = false;
            }

            visuals.Add(obj);
        }

        // Destruir visuals sobrantes
        while (visuals.Count > needed)
        {
            int last = visuals.Count - 1;
            if (visuals[last] != null) Destroy(visuals[last]);
            visuals.RemoveAt(last);
        }
    }

    void SetOrbitVisualsVisible(int index, bool visible)
    {
        if (index >= orbitVisuals.Count) return;
        foreach (var obj in orbitVisuals[index])
        {
            if (obj == null) continue;
            var sr = obj.GetComponent<SpriteRenderer>();
            if (sr != null) sr.enabled = visible;
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
        orbitVisuals.Add(SpawnOrbitVisuals(runtime));
        orbitIsActive.Add(runtime.attackType == WeaponStats.AttackType.Orbit);
        orbitPhaseTimers.Add(runtime.effectDuration);
        orbitDamageTimers.Add(0f);

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