using UnityEngine;

namespace SwarmCore2D.World
{
    /// <summary>
    /// Optional world limits.
    /// Used when infinite world is disabled.
    /// </summary>
    public class WorldBounds
    {
        public bool enabled;

        public float minX;
        public float maxX;
        public float minY;
        public float maxY;

        public WorldBounds()
        {
            enabled = false;

            minX = -50;
            maxX = 50;
            minY = -50;
            maxY = 50;
        }

        public Vector2 Clamp(Vector2 position)
        {
            if (!enabled)
                return position;

            position.x = Mathf.Clamp(position.x, minX, maxX);
            position.y = Mathf.Clamp(position.y, minY, maxY);

            return position;
        }

        public bool IsInside(Vector2 position)
        {
            if (!enabled)
                return true;

            return
                position.x >= minX &&
                position.x <= maxX &&
                position.y >= minY &&
                position.y <= maxY;
        }
    }
}