using UnityEngine;
using SwarmCore2D.Core;

namespace SwarmCore2D.Simulation
{
    public class SwarmWorld
    {
        public SwarmState state;
        public EntityAllocator allocator;

        public SwarmWorld()
        {
            state = new SwarmState();
            allocator = new EntityAllocator();
        }

        public SwarmEntity Spawn(Vector2 position, int type = 0)
        {
            var entity = allocator.Spawn();

            if (!entity.IsValid)
                return SwarmEntity.Null;

            int id = entity.id;

            state.positions[id] = position;
            state.velocities[id] = Vector2.zero;

            state.radius[id] = 0.5f;
            state.mass[id] = 1f;

            state.type[id] = type;

            state.Activate(id);

            return entity;
        }

        public void Despawn(SwarmEntity e)
        {
            int id = e.id;

            state.Deactivate(id);
            allocator.Despawn(e);
        }
    }
}