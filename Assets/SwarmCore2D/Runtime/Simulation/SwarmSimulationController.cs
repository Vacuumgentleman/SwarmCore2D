using UnityEngine;
using SwarmCore2D.Core;
using SwarmCore2D.Rendering;
using SwarmCore2D.Combat;

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

        ProjectileSystem projectileSystem;

        public SwarmState WorldState => world.state;

        public ProjectileSystem ProjectileSystem => projectileSystem;

        void Awake()
        {
            world = new SwarmWorld();

            spawner = new EnemySpawnerSystem();
            chase = new EnemyChaseSystem();
            separation = new EnemySeparationSystem();

            grid = new SpatialHashGrid(1.2f);

            projectileSystem = new ProjectileSystem(grid);

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

            // actualizar grid para colisiones
            grid.Clear();

            int count = world.state.activeCount;

            for (int i = 0; i < count; i++)
            {
                int id = world.state.activeList[i];

                grid.Add(
                    id,
                    world.state.positions[id]
                );
            }

            // actualizar proyectiles
            projectileSystem.Update(world.state);
        }
    }
}