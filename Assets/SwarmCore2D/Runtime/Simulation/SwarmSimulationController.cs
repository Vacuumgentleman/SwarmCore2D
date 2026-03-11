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

        public SwarmState WorldState => world.state;

        void Awake()
        {
            world = new SwarmWorld();

            spawner = new EnemySpawnerSystem();
            chase = new EnemyChaseSystem();
            separation = new EnemySeparationSystem();

            if (swarmRenderer != null)
                swarmRenderer.Initialize(world.state);
        }

        void Update()
        {
            SwarmTime.Step();

            if (player == null)
                return;

            Vector2 playerPos = player.position;

            // spawn enemigos
            spawner.Update(world, playerPos);

            // movimiento hacia el jugador
            chase.Update(world.state, playerPos);

            // evitar que se acumulen
            separation.Update(world.state);
        }
    }
}