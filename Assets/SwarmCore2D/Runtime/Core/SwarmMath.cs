using UnityEngine;

namespace SwarmCore2D.Core
{
    /// <summary>
    /// Math utilities optimized for swarm simulation.
    /// </summary>
    public static class SwarmMath
    {
        public static float Length(Vector2 v)
        {
            return Mathf.Sqrt(v.x * v.x + v.y * v.y);
        }

        public static float LengthSq(Vector2 v)
        {
            return v.x * v.x + v.y * v.y;
        }

        public static Vector2 Normalize(Vector2 v)
        {
            float len = Length(v);

            if (len < SwarmConstants.PositionEpsilon)
                return Vector2.zero;

            return v / len;
        }

        public static Vector2 ClampMagnitude(Vector2 v, float max)
        {
            float sq = LengthSq(v);

            if (sq > max * max)
            {
                float len = Mathf.Sqrt(sq);
                return v / len * max;
            }

            return v;
        }

        public static float Distance(Vector2 a, Vector2 b)
        {
            return Length(a - b);
        }

        public static float DistanceSq(Vector2 a, Vector2 b)
        {
            return LengthSq(a - b);
        }

        public static Vector2 SafeNormalize(Vector2 v)
        {
            float sq = LengthSq(v);

            if (sq < SwarmConstants.PositionEpsilon)
                return Vector2.zero;

            return v / Mathf.Sqrt(sq);
        }
    }
}