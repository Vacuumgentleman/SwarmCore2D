using UnityEngine;
using SwarmCore2D.Core;

namespace SwarmCore2D.Simulation
{
    /// <summary>
    /// Central data container for the swarm simulation.
    /// Stores all entity data using Structure-of-Arrays layout.
    /// </summary>
    public class SwarmState
    {
        public readonly int Capacity;

        // core data
        public Vector2[] positions;
        public Vector2[] velocities;

        public float[] radius;
        public float[] mass;

        public int[] type;

        // optional flags
        public bool[] active;

        public SwarmState()
        {
            Capacity = SwarmConstants.MaxEntities;

            positions = new Vector2[Capacity];
            velocities = new Vector2[Capacity];

            radius = new float[Capacity];
            mass = new float[Capacity];

            type = new int[Capacity];

            active = new bool[Capacity];
        }

        public void Activate(int id)
        {
            if (id < 0 || id >= Capacity)
                return;

            active[id] = true;
        }

        public void Deactivate(int id)
        {
            if (id < 0 || id >= Capacity)
                return;

            active[id] = false;
        }

        public bool IsActive(int id)
        {
            if (id < 0 || id >= Capacity)
                return false;

            return active[id];
        }

        public void Clear()
        {
            System.Array.Clear(positions, 0, Capacity);
            System.Array.Clear(velocities, 0, Capacity);

            System.Array.Clear(radius, 0, Capacity);
            System.Array.Clear(mass, 0, Capacity);

            System.Array.Clear(type, 0, Capacity);
            System.Array.Clear(active, 0, Capacity);
        }
    }
}