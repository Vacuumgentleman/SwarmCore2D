using System;

namespace SwarmCore2D.Simulation
{
    /// <summary>
    /// Internal deterministic event system for swarm simulation.
    /// Used for spawning, despawning, and interactions.
    /// </summary>
    public class SwarmEvents
    {
        public event Action<SwarmEntity> OnSpawn;
        public event Action<SwarmEntity> OnDespawn;

        public event Action<SwarmEntity, SwarmEntity> OnCollision;

        public void RaiseSpawn(SwarmEntity entity)
        {
            OnSpawn?.Invoke(entity);
        }

        public void RaiseDespawn(SwarmEntity entity)
        {
            OnDespawn?.Invoke(entity);
        }

        public void RaiseCollision(SwarmEntity a, SwarmEntity b)
        {
            OnCollision?.Invoke(a, b);
        }
    }
}