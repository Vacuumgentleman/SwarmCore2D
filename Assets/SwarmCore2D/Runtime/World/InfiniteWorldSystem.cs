using UnityEngine;
using SwarmCore2D.Simulation;

namespace SwarmCore2D.World
{
    /// <summary>
    /// Handles infinite world behaviour and optional bounds.
    /// </summary>
    public class InfiniteWorldSystem
    {
        WorldBounds bounds;
        WorldRecenter recenter;

        Transform player;

        SwarmState state;

        public InfiniteWorldSystem(
            SwarmState state,
            Transform player
        )
        {
            this.state = state;
            this.player = player;

            bounds = new WorldBounds();
            recenter = new WorldRecenter();
        }

        public void EnableBounds(float size)
        {
            bounds.enabled = true;

            bounds.minX = -size;
            bounds.maxX = size;
            bounds.minY = -size;
            bounds.maxY = size;
        }

        public void DisableBounds()
        {
            bounds.enabled = false;
        }

        public void Update()
        {
            if (player == null)
                return;

            Vector2 playerPos = player.position;

            if (recenter.ShouldRecenter(playerPos))
            {
                recenter.Apply(state, playerPos);

                player.position = Vector3.zero;
            }
        }

        public Vector2 ClampPosition(Vector2 position)
        {
            return bounds.Clamp(position);
        }
    }
}