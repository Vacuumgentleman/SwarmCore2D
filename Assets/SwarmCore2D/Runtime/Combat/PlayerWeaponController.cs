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

    public AttackVisualSystem AttackVisualSystem => attackVisualSystem;

    List<WeaponRuntimeStats> runtimes          = new List<WeaponRuntimeStats>();
    List<float>              timers             = new List<float>();
    List<List<Vector2>>      directionLists     = new List<List<Vector2>>();
    List<int>                lastDirIndexes     = new List<int>();
    List<float[]>            orbitAngles        = new List<float[]>();
    List<int[]>              orbitVisualIds     = new List<int[]>();
    List<bool>               orbitIsActive      = new List<bool>();
    List<float>              orbitPhaseTimers   = new List<float>();
    List<float>              orbitDamageTimers  = new List<float>();
    List<int>                activeAreaVisualIds = new List<int>();

    PlayerMeleeAttackSystem meleeSystem;
    ProjectileSystem        projectileSystem;
    AttackVisualSystem      attackVisualSystem;

    public static PlayerWeaponController Instance { get; private set; }

    void Awake()
    {
        Instance = this;
        attackVisualSystem = new AttackVisualSystem();
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

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

        meleeSystem      = new PlayerMeleeAttackSystem(simulation.WorldState);
        projectileSystem = simulation.ProjectileSystem;

        InitializeWeapons();
    }

    void InitializeWeapons()
    {
        // Deactivate any existing orbit visuals (explicit removal, safe)
        if (attackVisualSystem != null)
            foreach (var ids in orbitVisualIds)
                foreach (var id in ids)
                    attackVisualSystem.Deactivate(id);

        runtimes.Clear();
        timers.Clear();
        directionLists.Clear();
        lastDirIndexes.Clear();
        orbitAngles.Clear();
        orbitVisualIds.Clear();
        orbitIsActive.Clear();
        orbitPhaseTimers.Clear();
        orbitDamageTimers.Clear();
        activeAreaVisualIds.Clear();

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
            orbitVisualIds.Add(SpawnOrbitVisualIds(runtime));
            orbitIsActive.Add(runtime.attackType == WeaponStats.AttackType.Orbit);
            orbitPhaseTimers.Add(runtime.effectDuration);
            orbitDamageTimers.Add(0f);
            activeAreaVisualIds.Add(-1);
        }

        OnWeaponsChanged?.Invoke();
    }

    // Returns attackVisualScale from the asset, with a safe fallback for existing assets
    // that have the field serialized as 0 (new field defaults to 0 in serialized data).
    static float EffectiveVisualScale(WeaponRuntimeStats runtime)
        => runtime.attackVisualScale > 0f ? runtime.attackVisualScale : 1f;

    int[] SpawnOrbitVisualIds(WeaponRuntimeStats runtime)
    {
        int count = Mathf.Max(1, runtime.amount);
        var ids = new int[count];
        for (int i = 0; i < count; i++)
            ids[i] = SpawnLoopVisual(runtime, transform.position, 0f);
        return ids;
    }

    int SpawnLoopVisual(WeaponRuntimeStats runtime, Vector2 pos, float scale)
    {
        if (runtime.attackVisualMaterial == null) return -1;
        return attackVisualSystem.Spawn(pos, 0f, scale,
            runtime.attackVisualMaterial,
            runtime.attackVisualFrameCount,
            runtime.attackVisualFrameRate,
            loop: true);
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

        Vector2 playerPos = transform.position;

        for (int i = 0; i < runtimes.Count; i++)
        {
            var runtime = runtimes[i];

            if (runtime.attackType == WeaponStats.AttackType.Orbit)
            {
                UpdateOrbit(i, runtime, global);
                continue;
            }

            // Area: keep visual centered on player while it plays
            if (runtime.attackType == WeaponStats.AttackType.Area)
            {
                int areaId = activeAreaVisualIds[i];
                if (areaId >= 0 && areaId < AttackVisualState.Capacity && attackVisualSystem.state.active[areaId])
                    attackVisualSystem.UpdatePosition(areaId, playerPos);
            }

            timers[i] -= Time.deltaTime;
            if (timers[i] > 0f) continue;

            float cooldown = runtime.cooldown;
            if (global != null) cooldown *= global.cooldownMultiplier;
            timers[i] = cooldown;

            FireWeapon(i, global);
        }

        attackVisualSystem.Update(Time.deltaTime);
    }

    void FireWeapon(int index, PlayerStatsRuntime global)
    {
        var runtime = runtimes[index];
        switch (runtime.attackType)
        {
            case WeaponStats.AttackType.Melee:      FireMelee(index, runtime, global);      break;
            case WeaponStats.AttackType.Projectile: FireProjectile(index, runtime, global); break;
            case WeaponStats.AttackType.Area:       FireArea(index, runtime, global);       break;
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

            float dmg    = runtime.damage;
            float radius = runtime.hitRadius;

            if (global != null)
            {
                if (weapons[index].scaledByMight) dmg    *= global.damageMultiplier;
                if (weapons[index].scaledByArea)  radius *= global.areaMultiplier;
            }

            meleeSystem.Attack(playerPos, dir, radius, runtime.attackAngle, dmg, runtime.knockback);
            SpawnAttackVisual(playerPos, dir, EffectiveVisualScale(runtime), runtime);
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

        float dmg      = runtime.damage;
        float size     = runtime.projectileSize;
        float speed    = runtime.projectileSpeed;
        float duration = runtime.effectDuration;
        float range    = runtime.maxRange;
        int   pierce   = runtime.pierceCount;

        if (global != null)
        {
            if (weapons[index].scaledByMight)    dmg      *= global.damageMultiplier;
            if (weapons[index].scaledByArea)     size     *= global.areaMultiplier;
            if (weapons[index].scaledBySpeed)    speed    *= global.speedMultiplier;
            if (weapons[index].scaledByDuration) duration *= global.durationMultiplier;
            if (global.pierceBonus > 0)          pierce   += global.pierceBonus;
        }

        int[] slotCount   = new int[dirCount];
        int[] slotOf      = new int[total];
        int[] indexInSlot = new int[total];

        if (runtime.directionMode == WeaponStats.AttackDirectionMode.Volley)
        {
            int volleySlot = lastDirIndexes[index] % dirCount;
            for (int p = 0; p < total; p++)
            {
                slotOf[p]      = volleySlot;
                indexInSlot[p] = slotCount[volleySlot]++;
            }
            lastDirIndexes[index] += 1;
        }
        else
        {
            int startDir = runtime.directionMode == WeaponStats.AttackDirectionMode.Alternating
                ? lastDirIndexes[index] : 0;

            for (int p = 0; p < total; p++)
            {
                int slot = (startDir + p) % dirCount;
                slotOf[p]      = slot;
                indexInSlot[p] = slotCount[slot]++;
            }

            if (runtime.directionMode == WeaponStats.AttackDirectionMode.Alternating)
                lastDirIndexes[index] += total;
        }

        for (int p = 0; p < total; p++)
        {
            int    slot = slotOf[p];
            Vector2 dir = directions[slot];

            int n = slotCount[slot];
            if (n > 1 && runtime.spreadAngle > 0f)
            {
                float totalArc = runtime.spreadAngle * (n - 1);
                float angle    = -totalArc * 0.5f + indexInSlot[p] * runtime.spreadAngle;
                dir = RotateDirection(dir, angle);
            }

            projectileSystem.Spawn(playerPos, dir, speed, dmg, duration, range, pierce, size,
                runtime.attackVisualMaterial, runtime.attackVisualFrameCount, runtime.attackVisualFrameRate,
                runtime.rotateProjectile);
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

        float dmg    = runtime.damage;
        float radius = runtime.hitRadius;

        if (global != null)
        {
            if (weapons[index].scaledByMight) dmg    *= global.damageMultiplier;
            if (weapons[index].scaledByArea)  radius *= global.areaMultiplier;
        }

        meleeSystem.Attack(playerPos, Vector2.up, radius, 360f, dmg, runtime.knockback);

        // Scale area visual with the actual hit radius so it matches gameplay
        float visualScale = EffectiveVisualScale(runtime) * (radius / Mathf.Max(0.01f, runtime.hitRadius));
        activeAreaVisualIds[index] = SpawnAttackVisual(playerPos, Vector2.zero, visualScale, runtime);
    }

    void UpdateOrbit(int index, WeaponRuntimeStats runtime, PlayerStatsRuntime global)
    {
        Vector2 playerPos = transform.position;

        float orbitRadius    = runtime.hitRadius;
        float dmg            = runtime.damage;
        float hitSize        = runtime.projectileSize;
        float orbitSpeed     = runtime.projectileSpeed;
        float activeDuration = runtime.effectDuration;
        float cooldownDur    = runtime.cooldown;

        if (global != null)
        {
            if (weapons[index].scaledByMight)   dmg *= global.damageMultiplier;
            if (weapons[index].scaledByArea)    { orbitRadius *= global.areaMultiplier; hitSize *= global.areaMultiplier; }
            if (weapons[index].scaledBySpeed)   orbitSpeed    *= global.speedMultiplier;
            if (weapons[index].scaledByDuration) activeDuration *= global.durationMultiplier;
            cooldownDur *= global.cooldownMultiplier;
        }

        SyncOrbitState(index, runtime);

        // Phase timer — toggle active/inactive using scale=0 (never deactivate IDs)
        orbitPhaseTimers[index] -= Time.deltaTime;
        if (orbitPhaseTimers[index] <= 0f)
        {
            orbitIsActive[index]    = !orbitIsActive[index];
            orbitPhaseTimers[index] = orbitIsActive[index] ? activeDuration : cooldownDur;
        }

        bool  isActive    = orbitIsActive[index];
        float visualScale = isActive ? hitSize * EffectiveVisualScale(runtime) : 0f;

        var   angles = orbitAngles[index];
        var   ids    = orbitVisualIds[index];
        int   count  = angles.Length;

        for (int p = 0; p < count; p++)
        {
            angles[p] += orbitSpeed * Time.deltaTime;
            if (angles[p] >= 360f) angles[p] -= 360f;

            float   rad      = angles[p] * Mathf.Deg2Rad;
            Vector2 orbitPos = playerPos + new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * orbitRadius;

            if (p < ids.Length && ids[p] >= 0)
            {
                attackVisualSystem.UpdatePosition(ids[p], orbitPos);
                attackVisualSystem.UpdateScale(ids[p], visualScale);
            }
        }

        if (!isActive) return;

        orbitDamageTimers[index] -= Time.deltaTime;
        if (orbitDamageTimers[index] > 0f) return;

        orbitDamageTimers[index] = 0.3f;

        for (int p = 0; p < count; p++)
        {
            float   rad      = angles[p] * Mathf.Deg2Rad;
            Vector2 orbitPos = playerPos + new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * orbitRadius;
            meleeSystem.Attack(orbitPos, Vector2.up, hitSize, 360f, dmg, 0f);
        }
    }

    void SyncOrbitState(int index, WeaponRuntimeStats runtime)
    {
        int    needed = Mathf.Max(1, runtime.amount);
        var    ids    = orbitVisualIds[index];
        float[] angles = orbitAngles[index];

        if (angles.Length != needed)
        {
            float  baseAngle = angles.Length > 0 ? angles[0] : 0f;
            float  step      = 360f / needed;
            float[] newAngles = new float[needed];
            for (int i = 0; i < needed; i++)
                newAngles[i] = baseAngle + i * step;
            orbitAngles[index] = newAngles;
        }

        if (ids.Length == needed) return;

        // Deactivate surplus (explicit removal — not a recycling hazard)
        for (int i = needed; i < ids.Length; i++)
            attackVisualSystem.Deactivate(ids[i]);

        var newIds = new int[needed];
        int copy   = Mathf.Min(ids.Length, needed);
        for (int i = 0; i < copy; i++)
            newIds[i] = ids[i];

        // Spawn missing — scale=0 so UpdateOrbit sets correct scale next frame
        for (int i = copy; i < needed; i++)
            newIds[i] = SpawnLoopVisual(runtime, transform.position, 0f);

        orbitVisualIds[index] = newIds;
    }

    int GetDirIndex(int weaponIndex, WeaponRuntimeStats runtime, int projectileIndex, int dirCount)
    {
        if (runtime.directionMode == WeaponStats.AttackDirectionMode.Clockwise)
            return projectileIndex % dirCount;

        if (runtime.directionMode == WeaponStats.AttackDirectionMode.Volley)
            return lastDirIndexes[weaponIndex]++ / Mathf.Max(1, runtime.amount) % dirCount;

        return lastDirIndexes[weaponIndex]++ % dirCount;
    }

    // Returns the spawned visual ID (-1 if no material configured)
    int SpawnAttackVisual(Vector2 playerPos, Vector2 dir, float visualScale, WeaponRuntimeStats runtime)
    {
        if (runtime.attackVisualMaterial == null) return -1;

        float   offset   = dir == Vector2.zero ? 0f : runtime.hitRadius * 0.5f;
        Vector2 spawnPos = playerPos + dir * offset;
        float   angle    = dir == Vector2.zero ? 0f : Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        return attackVisualSystem.Spawn(spawnPos, angle, visualScale,
            runtime.attackVisualMaterial,
            runtime.attackVisualFrameCount,
            runtime.attackVisualFrameRate);
    }

    public bool AddWeapon(WeaponStats weapon)
    {
        if (weapon == null || runtimes.Count >= maxWeaponSlots)
            return false;

        weapons.Add(weapon);

        var runtime = new WeaponRuntimeStats();
        runtime.LoadFrom(weapon);

        runtimes.Add(runtime);
        timers.Add(0f);
        directionLists.Add(BuildDirectionList(runtime));
        lastDirIndexes.Add(0);
        orbitAngles.Add(BuildOrbitAngles(runtime.amount));
        orbitVisualIds.Add(SpawnOrbitVisualIds(runtime));
        orbitIsActive.Add(runtime.attackType == WeaponStats.AttackType.Orbit);
        orbitPhaseTimers.Add(runtime.effectDuration);
        orbitDamageTimers.Add(0f);
        activeAreaVisualIds.Add(-1);

        OnWeaponsChanged?.Invoke();
        return true;
    }

    public WeaponRuntimeStats GetRuntime(int index)
    {
        if (index < 0 || index >= runtimes.Count) return null;
        return runtimes[index];
    }

    public int WeaponCount => runtimes.Count;

    public void RebuildDirections(int index)
    {
        if (index < 0 || index >= runtimes.Count) return;
        directionLists[index] = BuildDirectionList(runtimes[index]);
    }

    public void RebuildOrbitAngles(int index)
    {
        if (index < 0 || index >= runtimes.Count) return;
        orbitAngles[index] = BuildOrbitAngles(runtimes[index].amount);
    }
}
