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
    int directionIndex = 0;

    void Start()
    {
        if (simulation == null)
            simulation = FindFirstObjectByType<SwarmSimulationController>();

        if (simulation != null)
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

        if (up && right)
            directions.Add((Vector2.up + Vector2.right).normalized);

        if (right) directions.Add(Vector2.right);

        if (down && right)
            directions.Add((Vector2.down + Vector2.right).normalized);

        if (down) directions.Add(Vector2.down);

        if (down && left)
            directions.Add((Vector2.down + Vector2.left).normalized);

        if (left) directions.Add(Vector2.left);

        if (up && left)
            directions.Add((Vector2.up + Vector2.left).normalized);

        if (directions.Count == 0)
            directions.Add(Vector2.right);
    }

    void Update()
    {
        if (weapon == null || attackSystem == null)
            return;

        timer -= Time.deltaTime;

        if (timer > 0)
            return;

        timer = weapon.cooldown;

        Vector2 playerPos = transform.position;

        Vector2 dir = directions[directionIndex];

        attackSystem.Attack(
            playerPos,
            weapon,
            dir
        );

        SpawnAttackVisual(playerPos, dir);

        directionIndex++;

        if (directionIndex >= directions.Count)
            directionIndex = 0;
    }

    void SpawnAttackVisual(Vector2 playerPos, Vector2 dir)
    {
        if (weapon.attackVisualPrefab == null)
            return;

        float offset = weapon.radius * 0.5f;

        Vector3 spawnPos = playerPos + dir * offset;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        GameObject obj = Instantiate(
            weapon.attackVisualPrefab,
            spawnPos,
            Quaternion.Euler(0, 0, angle)
        );

        AttackVisual visual = obj.GetComponent<AttackVisual>();

        if (visual != null)
        {
            visual.Init(
                weapon.frames,
                weapon.frameRate
            );
        }
    }

    void OnDrawGizmos()
    {
        if (weapon == null)
            return;

        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            transform.position,
            weapon.radius
        );
    }
}