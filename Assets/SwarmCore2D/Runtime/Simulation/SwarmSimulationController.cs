using UnityEngine;
using SwarmCore2D.Core;
using SwarmCore2D.Rendering;
using SwarmCore2D.Combat;
using SwarmCore2D.World;

namespace SwarmCore2D.Simulation
{
    public class SwarmSimulationController : MonoBehaviour
    {
        [Header("Scene References")]
        public Transform player;
        public SwarmRenderer swarmRenderer;
        public ProjectileRenderer projectileRenderer;

        public WorldRenderer worldRenderer;

        SwarmWorld world;

        EnemySpawnerSystem spawner;
        EnemyChaseSystem chase;
        EnemySeparationSystem separation;

        EnemyContactDamageSystem contactDamage;

        SpatialHashGrid grid;

        ProjectileSystem projectileSystem;

        InfiniteWorldSystem infiniteWorld;
        WorldChunkSystem chunkSystem;

        PlayerHealth playerHealth;

        public SwarmState WorldState => world.state;

        public ProjectileSystem ProjectileSystem => projectileSystem;

        void Awake()
        {
            world = new SwarmWorld();

            spawner = new EnemySpawnerSystem();
            chase = new EnemyChaseSystem();
            separation = new EnemySeparationSystem();

            contactDamage = new EnemyContactDamageSystem();

            grid = new SpatialHashGrid(1.2f);

            projectileSystem = new ProjectileSystem(grid);

            infiniteWorld = new InfiniteWorldSystem(world.state, player);
            chunkSystem = new WorldChunkSystem();

            if (player != null)
                playerHealth = player.GetComponent<PlayerHealth>();

            if (worldRenderer != null)
                worldRenderer.Initialize(chunkSystem);

            if (swarmRenderer != null)
                swarmRenderer.Initialize(world.state);

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

            // mundo infinito
            infiniteWorld.Update();

            // chunks del mundo
            chunkSystem.Update(playerPos);

            // spawn enemigos
            spawner.Update(world, playerPos);

            // movimiento enemigos
            chase.Update(world.state, playerPos);

            // separación enemigos
            separation.Update(world.state);

            // daño por contacto al jugador
            if (playerHealth != null)
            {
                contactDamage.Update(
                    world.state,
                    playerPos,
                    playerHealth
                );
            }

            // actualizar hit flash enemigos
            UpdateHitFlash(world.state);

            // actualizar spatial grid
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

        void UpdateHitFlash(SwarmState state)
        {
            float dt = SwarmTime.FixedDelta;

            int count = state.activeCount;

            for (int i = 0; i < count; i++)
            {
                int id = state.activeList[i];

                if (state.hitFlash[id] > 0f)
                    state.hitFlash[id] -= dt;
            }
        }
    }
}