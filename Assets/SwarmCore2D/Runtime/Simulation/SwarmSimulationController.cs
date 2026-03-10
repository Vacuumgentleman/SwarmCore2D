using UnityEngine;
using SwarmCore2D.Core;

namespace SwarmCore2D.Simulation
{
    public class SwarmSimulationController : MonoBehaviour
{
    public Transform player;

    SwarmWorld world;

    EnemySpawnerSystem spawner;
    EnemyChaseSystem chase;

    public SwarmWorld World => world;
    public SwarmState WorldState => world.state;

    void Awake()
    {
        world = new SwarmWorld();

        spawner = new EnemySpawnerSystem();
        chase = new EnemyChaseSystem();
    }

    void Update()
    {
        SwarmTime.Step();

        Vector2 playerPos = player.position;

        spawner.Update(world, playerPos);
        chase.Update(world.state, playerPos);
    }
}
}