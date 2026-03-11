using UnityEngine;
using SwarmCore2D.Core;

namespace SwarmCore2D.Simulation
{
    public class SwarmState
    {
        public readonly int Capacity;

        public Vector2[] positions;
        public Vector2[] velocities;

        public float[] radius;
        public float[] mass;

        public int[] type;

        // animation
        public int[] frame;
        public bool[] facingLeft;
        public float[] animOffset;

        public bool[] active;

        public SwarmState()
        {
            Capacity = SwarmConstants.MaxEntities;

            positions = new Vector2[Capacity];
            velocities = new Vector2[Capacity];

            radius = new float[Capacity];
            mass = new float[Capacity];

            type = new int[Capacity];

            frame = new int[Capacity];
            facingLeft = new bool[Capacity];
            animOffset = new float[Capacity];

            active = new bool[Capacity];
        }

        public void Activate(int id)
        {
            if (id < 0 || id >= Capacity)
                return;

            active[id] = true;
            animOffset[id] = Random.value * 10f; // desfase animación
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

            System.Array.Clear(frame, 0, Capacity);
            System.Array.Clear(facingLeft, 0, Capacity);
            System.Array.Clear(animOffset, 0, Capacity);

            System.Array.Clear(active, 0, Capacity);
        }
    }
}