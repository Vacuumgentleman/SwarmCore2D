using UnityEngine;
using SwarmCore2D.Simulation;
using SwarmCore2D.Core;
using SwarmCore2D.Combat;
using System.Collections.Generic;

public class PlayerWeaponController : MonoBehaviour
{
    public WeaponStats weapon;

    public WeaponRuntimeStats runtime;

    public SwarmSimulationController simulation;

    PlayerMeleeAttackSystem meleeSystem;
    ProjectileSystem projectileSystem;

    float timer;

    List<Vector2> directions = new List<Vector2>();
    int lastDirIndex = 0;

    void Start()
    {
        if (runtime == null)
            runtime = GetComponent<WeaponRuntimeStats>();

        runtime.LoadFrom(weapon);

        if (simulation == null)
            simulation = FindFirstObjectByType<SwarmSimulationController>();

        if (simulation == null)
        {
            Debug.LogError("SwarmSimulationController not found");
            enabled = false;
            return;
        }

        if (runtime.attackType == WeaponStats.AttackType.Melee)
            meleeSystem = new PlayerMeleeAttackSystem(simulation.WorldState);

        if (runtime.attackType == WeaponStats.AttackType.Ranged)
            projectileSystem = simulation.ProjectileSystem;

        BuildDirectionList();
    }

    void BuildDirectionList()
    {
        directions.Clear();

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
    }

    void Update()
    {
        if (SwarmTime.Paused)
            return;

        if (runtime == null)
            return;

        timer -= Time.deltaTime;

        if (timer > 0)
            return;

        timer = runtime.cooldown;

        FireAttack();
    }

    void FireAttack()
    {
        if (runtime.attackType == WeaponStats.AttackType.Melee)
            FireMelee();
        else
            FireRanged();
    }

    void FireMelee()
    {
        if (meleeSystem == null)
            return;

        Vector2 playerPos = transform.position;

        int dirCount = directions.Count;

        for (int p = 0; p < runtime.amount; p++)
        {
            int dirIndex =
                runtime.directionMode == WeaponStats.AttackDirectionMode.Clockwise
                ? p % dirCount
                : lastDirIndex++ % dirCount;

            Vector2 dir = directions[dirIndex];

            meleeSystem.Attack(playerPos, ConvertToWeaponStats(), dir);

            SpawnAttackVisual(playerPos, dir);
        }
    }

    void FireRanged()
    {
        if (projectileSystem == null)
            return;

        Vector2 playerPos = transform.position;

        int dirCount = directions.Count;

        for (int p = 0; p < runtime.amount; p++)
        {
            int dirIndex =
                runtime.directionMode == WeaponStats.AttackDirectionMode.Clockwise
                ? p % dirCount
                : lastDirIndex++ % dirCount;

            Vector2 dir = directions[dirIndex];

            projectileSystem.Spawn(
                playerPos,
                dir,
                ConvertToWeaponStats()
            );
        }
    }

    WeaponStats ConvertToWeaponStats()
    {
        weapon.damage = runtime.damage;
        weapon.radius = runtime.radius;
        weapon.cooldown = runtime.cooldown;

        weapon.amount = runtime.amount;

        weapon.knockback = runtime.knockback;
        weapon.attackAngle = runtime.attackAngle;

        weapon.maxDistance = runtime.maxDistance;
        weapon.maxLifetime = runtime.maxLifetime;
        weapon.projectileSize = runtime.projectileSize;
        weapon.pierceEnemies = runtime.pierceEnemies;

        return weapon;
    }

    void SpawnAttackVisual(Vector2 playerPos, Vector2 dir)
    {
        if (runtime.attackVisualPrefab == null)
            return;

        float offset = runtime.radius * 0.5f;

        Vector3 spawnPos = playerPos + dir * offset;

        float angle =
            Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        GameObject obj = Instantiate(
            runtime.attackVisualPrefab,
            spawnPos,
            Quaternion.Euler(0, 0, angle)
        );

        AttackVisual visual = obj.GetComponent<AttackVisual>();

        if (visual != null)
        {
            visual.Init(runtime.frames, runtime.frameRate);
        }
    }

    public void RebuildDirections()
    {
        BuildDirectionList();
    }
}