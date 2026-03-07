using UnityEngine;
using SwarmCore2D.Core;
using SwarmCore2D.Simulation;

namespace SwarmCore2D.Rendering
{
    /// <summary>
    /// Collects render matrices from simulation state.
    /// </summary>
    public class RenderData
    {
        public Matrix4x4[] matrices;

        public int count;

        public RenderData()
        {
            matrices = new Matrix4x4[SwarmConstants.MaxEntities];
            count = 0;
        }

        public void Build(SwarmState state)
        {
            count = 0;

            var positions = state.positions;
            var radius = state.radius;
            var active = state.active;

            int capacity = state.Capacity;

            for (int i = 0; i < capacity; i++)
            {
                if (!active[i])
                    continue;

                Vector2 pos = positions[i];
                float r = radius[i];

                matrices[count++] =
                    Matrix4x4.TRS(
                        new Vector3(pos.x, pos.y, 0),
                        Quaternion.identity,
                        new Vector3(r, r, 1f)
                    );
            }
        }
    }
}