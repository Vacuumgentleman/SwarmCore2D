using UnityEngine;
using SwarmCore2D.Core;
using SwarmCore2D.Rendering;

namespace SwarmCore2D.Simulation
{
    public class SwarmSimulationController : MonoBehaviour
    {
        [Header("Scene References")]
        public Transform player;
        public SwarmRenderer swarmRenderer;

        SwarmWorld world;

        EnemySpawnerSystem spawner;
        EnemyChaseSystem chase;
        EnemySeparationSystem separation;

        SpatialHashGrid grid;

        public SwarmState WorldState => world.state;

        void Awake()
        {
            world = new SwarmWorld();

            spawner = new EnemySpawnerSystem();
            chase = new EnemyChaseSystem();
            separation = new EnemySeparationSystem();

            // grid usado por los sistemas de enemigos
            grid = new SpatialHashGrid(1.2f);

            if (swarmRenderer != null)
                swarmRenderer.Initialize(world.state);
        }

        void Update()
        {
            SwarmTime.Step();

            if (player == null)
                return;

            Vector2 playerPos = player.position;

            spawner.Update(world, playerPos);

            chase.Update(world.state, playerPos);

            separation.Update(world.state);
        }
    }
}