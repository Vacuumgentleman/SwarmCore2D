using UnityEngine;
using SwarmCore2D.Simulation;
using SwarmCore2D.Core;
using System.Collections.Generic;

public class PlayerWeaponController : MonoBehaviour
{
    public WeaponStats weapon;
    public SwarmSimulationController simulation;

    PlayerMeleeAttackSystem attackSystem;

    float timer;
    List<Vector2> directions = new List<Vector2>();
    int lastDirIndex = 0;

    void Start()
    {
        if (simulation == null)
            simulation = FindFirstObjectByType<SwarmSimulationController>();

        if (simulation != null && weapon.attackType == WeaponStats.AttackType.Melee)
            attackSystem = new PlayerMeleeAttackSystem(simulation.WorldState);

        BuildDirectionList();
    }

    void BuildDirectionList()
    {
        directions.Clear();
        bool up = weapon.attackUp;
        bool down = weapon.attackDown;
        bool left = weapon.attackLeft;
        bool right = weapon.attackRight;

        if (up) directions.Add(Vector2.up);
        if (weapon.allowDiagonals && up && right) directions.Add((Vector2.up + Vector2.right).normalized);
        if (right) directions.Add(Vector2.right);
        if (weapon.allowDiagonals && down && right) directions.Add((Vector2.down + Vector2.right).normalized);
        if (down) directions.Add(Vector2.down);
        if (weapon.allowDiagonals && down && left) directions.Add((Vector2.down + Vector2.left).normalized);
        if (left) directions.Add(Vector2.left);
        if (weapon.allowDiagonals && up && left) directions.Add((Vector2.up + Vector2.left).normalized);

        if (directions.Count == 0)
            directions.Add(Vector2.right);
    }

    void Update()
    {
        if (weapon == null) return;

        timer -= Time.deltaTime;
        if (timer > 0) return;

        timer = weapon.cooldown;
        FireAttack();
    }

    void FireAttack()
    {
        switch (weapon.attackType)
        {
            case WeaponStats.AttackType.Melee:
                FireMelee();
                break;
            case WeaponStats.AttackType.Ranged:
                FireRanged();
                break;
        }
    }

    void FireMelee()
    {
        if (attackSystem == null) return;

        Vector2 playerPos = transform.position;
        int dirCount = directions.Count;
        int projectileCount = weapon.projectileCount;

        for (int p = 0; p < projectileCount; p++)
        {
            int dirIndex = (weapon.directionMode == WeaponStats.AttackDirectionMode.Clockwise) 
                ? p % dirCount 
                : lastDirIndex++ % dirCount;

            Vector2 dir = directions[dirIndex];
            attackSystem.Attack(playerPos, weapon, dir);
            SpawnAttackVisual(playerPos, dir);
        }
    }

    void FireRanged()
    {
        if (ProjectilePool.Instance == null)
        {
            Debug.LogError("ProjectilePool no existe en la escena");
            return;
        }

        if (simulation == null || simulation.WorldState == null)
        {
           Debug.LogError("SwarmSimulationController missing");
           return;
        }
        Vector2 playerPos = transform.position;
        int dirCount = directions.Count;
        int projectileCount = weapon.projectileCount;

        for (int p = 0; p < projectileCount; p++)
        {
            int dirIndex = (weapon.directionMode == WeaponStats.AttackDirectionMode.Clockwise)
                ? p % dirCount
                : lastDirIndex++ % dirCount;

            Vector2 dir = directions[dirIndex];

            Projectile proj = ProjectilePool.Instance.GetProjectile();

            proj.Activate(
                dir,
                weapon.damage,
                weapon.maxDistance,
                weapon.maxLifetime,
                weapon.pierceEnemies,
                simulation.WorldState,
                weapon.attackVisualPrefab,
                weapon.frames,
                weapon.frameRate,
                weapon.projectileSize,
                playerPos
            );
        }
    }
    void SpawnAttackVisual(Vector2 playerPos, Vector2 dir)
    {
        if (weapon.attackVisualPrefab == null) return;
        float offset = weapon.radius * 0.5f;
        Vector3 spawnPos = playerPos + dir * offset;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        GameObject obj = Instantiate(weapon.attackVisualPrefab, spawnPos, Quaternion.Euler(0, 0, angle));
        AttackVisual visual = obj.GetComponent<AttackVisual>();
        if (visual != null)
            visual.Init(weapon.frames, weapon.frameRate);
    }
}