using UnityEngine;
using SwarmCore2D.Simulation;
using SwarmCore2D.Core;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    Rigidbody2D rb;

    public ProjectilePool pool;

    Vector2 direction;
    float speed = 10f;

    float damage;
    float maxDistance;
    float lifetime;
    bool pierce;

    Vector2 startPos;

    SwarmState state;

    EnemyHealthSystem healthSystem = new EnemyHealthSystem();

    float hitRadius = 0.4f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    public void Activate(
        Vector2 dir,
        float dmg,
        float maxDist,
        float maxLife,
        bool pierceEnemies,
        SwarmState swarmState,
        GameObject visualPrefab,
        Sprite[] frames,
        float frameRate,
        float size,
        Vector3 spawnPos)
    {
        transform.position = spawnPos;

        direction = dir.normalized;
        damage = dmg;
        maxDistance = maxDist == 0 ? Mathf.Infinity : maxDist;
        lifetime = maxLife;
        pierce = pierceEnemies;
        startPos = spawnPos;

        state = swarmState;

        transform.localScale = Vector3.one * size;
    }

    void Update()
    {
        float dt = Time.deltaTime;

        Vector2 pos = rb.position + direction * speed * dt;
        rb.MovePosition(pos);

        lifetime -= dt;

        if (lifetime <= 0 ||
            Vector2.Distance(startPos, pos) >= maxDistance)
        {
            Deactivate();
            return;
        }

        CheckCollision();
    }

    void CheckCollision()
    {
        if (state == null)
            return;

        int count = state.activeCount;

        for (int i = 0; i < count; i++)
        {
            int id = state.activeList[i];

            if (state.type[id] != 1)
                continue;

            float dist =
                Vector2.Distance(transform.position, state.positions[id]);

            if (dist <= state.radius[id] + hitRadius)
            {
                healthSystem.Damage(state, id, damage);

                if (!pierce)
                {
                    Deactivate();
                    return;
                }
            }
        }
    }

    void Deactivate()
    {
        pool.ReturnProjectile(this);
    }
}