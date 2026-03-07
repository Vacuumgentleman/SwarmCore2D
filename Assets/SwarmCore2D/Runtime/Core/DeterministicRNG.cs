using UnityEngine;

namespace SwarmCore2D.Core
{
    /// <summary>
    /// Deterministic random number generator.
    /// Uses XORSHIFT32 algorithm.
    /// Same seed = same results across platforms.
    /// </summary>
    public struct DeterministicRNG
    {
        private uint state;

        public DeterministicRNG(uint seed)
        {
            if (seed == 0)
                seed = 1;

            state = seed;
        }

        /// <summary>
        /// Returns next random uint.
        /// </summary>
        public uint NextUInt()
        {
            uint x = state;

            x ^= x << 13;
            x ^= x >> 17;
            x ^= x << 5;

            state = x;

            return x;
        }

        /// <summary>
        /// Returns float in range [0,1]
        /// </summary>
        public float NextFloat()
        {
            return (NextUInt() & 0xFFFFFF) / (float)0x1000000;
        }

        /// <summary>
        /// Returns float in range [min,max]
        /// </summary>
        public float Range(float min, float max)
        {
            return min + (max - min) * NextFloat();
        }

        /// <summary>
        /// Returns int in range [min,max)
        /// </summary>
        public int Range(int min, int max)
        {
            return min + (int)(NextFloat() * (max - min));
        }

        /// <summary>
        /// Random inside unit circle.
        /// </summary>
        public Vector2 InsideUnitCircle()
        {
            float angle = NextFloat() * Mathf.PI * 2f;
            float radius = Mathf.Sqrt(NextFloat());

            return new Vector2(
                Mathf.Cos(angle) * radius,
                Mathf.Sin(angle) * radius
            );
        }

        /// <summary>
        /// Random direction normalized.
        /// </summary>
        public Vector2 Direction()
        {
            float angle = NextFloat() * Mathf.PI * 2f;

            return new Vector2(
                Mathf.Cos(angle),
                Mathf.Sin(angle)
            );
        }

        /// <summary>
        /// Reset RNG state.
        /// </summary>
        public void Reset(uint seed)
        {
            if (seed == 0)
                seed = 1;

            state = seed;
        }

        /// <summary>
        /// Returns current state (useful for debugging / replay).
        /// </summary>
        public uint GetState()
        {
            return state;
        }
    }
}