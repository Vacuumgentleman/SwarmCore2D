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
        public ProjectileRenderer projectileRenderer;

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

            // renderer enemigos
            if (swarmRenderer != null)
                swarmRenderer.Initialize(world.state);

            // buscar renderer de proyectiles automáticamente
            if (projectileRenderer == null)
                projectileRenderer = FindFirstObjectByType<ProjectileRenderer>();

            if (projectileRenderer != null)
                projectileRenderer.Initialize(projectileSystem);
            else
                Debug.LogWarning("ProjectileRenderer no encontrado en la escena");
        }

        void Update()
        {
            SwarmTime.Step();

            if (player == null)
                return;

            Vector2 playerPos = player.position;

            // spawn enemigos
            spawner.Update(world, playerPos);

            // movimiento enemigos
            chase.Update(world.state, playerPos);

            // separación
            separation.Update(world.state);

            // rebuild spatial grid
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

            // proyectiles
            projectileSystem.Update(world.state);
        }
    }
}