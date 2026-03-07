using System;

namespace SwarmCore2D.Simulation
{
    /// <summary>
    /// Lightweight entity handle used by the swarm simulation.
    /// It only contains an ID referencing data stored in SwarmState arrays.
    /// </summary>
    [Serializable]
    public struct SwarmEntity
    {
        public int id;

        public SwarmEntity(int id)
        {
            this.id = id;
        }

        public bool IsValid => id >= 0;

        public static readonly SwarmEntity Null = new SwarmEntity(-1);

        public override string ToString()
        {
            return $"Entity({id})";
        }
    }
}