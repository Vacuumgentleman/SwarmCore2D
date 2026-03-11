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

        float[] animSpeed;

        public int count;

        const int FrameCount = 7;
        const float BaseAnimSpeed = 8f;

        public RenderData()
        {
            int cap = SwarmConstants.MaxEntities;

            matrices = new Matrix4x4[cap];
            frames = new float[cap];
            flips = new float[cap];
            animSpeed = new float[cap];

            for (int i = 0; i < cap; i++)
            {
                animSpeed[i] = Random.Range(0.8f, 1.2f);
            }

            count = 0;
        }

        public void Build(SwarmState state)
        {
            count = 0;

            int capacity = state.Capacity;

            float time = Time.time;

            for (int i = 0; i < capacity; i++)
            {
                if (!state.active[i])
                    continue;

                Vector2 pos = state.positions[i];
                float r = state.radius[i];

                matrices[count] =
                    Matrix4x4.TRS(
                        new Vector3(pos.x, pos.y, 0),
                        Quaternion.identity,
                        new Vector3(r, r, 1f)
                    );

                float anim = time * BaseAnimSpeed * animSpeed[i];

                frames[count] = (int)anim % FrameCount;

                flips[count] = state.facingLeft[i] ? 1f : 0f;

                count++;
            }
        }
    }
}