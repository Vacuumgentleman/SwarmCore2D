using UnityEngine;
using SwarmCore2D.Core;
using SwarmCore2D.Simulation;

namespace SwarmCore2D.Rendering
{
    public class RenderData
    {
        public Matrix4x4[] matrices;
        public float[] frames;
        public float[] flips;

        public int count;

        public RenderData()
        {
            int cap = SwarmConstants.MaxEntities;

            matrices = new Matrix4x4[cap];
            frames = new float[cap];
            flips = new float[cap];

            count = 0;
        }

        public void Build(SwarmState state)
        {
            count = 0;

            int activeCount = state.activeCount;

            for (int a = 0; a < activeCount; a++)
            {
                int i = state.activeList[a];

                Vector2 pos = state.positions[i];
                float r = state.radius[i];

                matrices[count] =
                    Matrix4x4.TRS(
                        new Vector3(pos.x, pos.y, 0),
                        Quaternion.identity,
                        new Vector3(r, r, 1f)
                    );

                frames[count] = state.frame[i];
                flips[count] = state.facingLeft[i] ? 1f : 0f;

                count++;
            }
        }
    }
}