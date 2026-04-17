using System;
using SwarmCore2D.Core;

namespace SwarmCore2D.Simulation
{
    /// <summary>
    /// Handles deterministic allocation and recycling of entity IDs.
    /// Prevents memory allocations and allows large scale entity spawning.
    /// </summary>
    public class EntityAllocator
    {
        int nextId;
        int aliveCount;

        int[] freeStack;
        int freeTop;

        bool[] alive;

        readonly int capacity;

        public int Capacity => capacity;

        public int AliveCount => aliveCount;

        public EntityAllocator() : this(SwarmConstants.MaxEntities) { }

        public EntityAllocator(int cap)
        {
            capacity = cap;

            freeStack = new int[capacity];
            alive = new bool[capacity];

            freeTop = 0;
            nextId = 0;
            aliveCount = 0;
        }

        public SwarmEntity Spawn()
        {
            int id;

            if (freeTop > 0)
            {
                id = freeStack[--freeTop];
            }
            else
            {
                if (nextId >= Capacity)
                    return SwarmEntity.Null;

                id = nextId++;
            }

            alive[id] = true;
            aliveCount++;

            return new SwarmEntity(id);
        }

        public void Despawn(SwarmEntity entity)
        {
            int id = entity.id;

            if (id < 0 || id >= Capacity)
                return;

            if (!alive[id])
                return;

            alive[id] = false;

            freeStack[freeTop++] = id;

            aliveCount--;
        }

        public bool IsAlive(SwarmEntity entity)
        {
            int id = entity.id;

            if (id < 0 || id >= Capacity)
                return false;

            return alive[id];
        }

        public void Clear()
        {
            Array.Clear(alive, 0, alive.Length);

            freeTop = 0;
            nextId = 0;
            aliveCount = 0;
        }
    }
}
