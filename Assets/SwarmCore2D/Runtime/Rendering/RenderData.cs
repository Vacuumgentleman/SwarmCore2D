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
        public float[] hitFlash;
        public Vector4[] tint;

        public int[] enemyType;

        public int count;

        public RenderData()
        {
            int cap = SwarmConstants.MaxEntities;

            matrices = new Matrix4x4[cap];
            frames = new float[cap];
            flips = new float[cap];
            hitFlash = new float[cap];
            tint = new Vector4[cap];

            enemyType = new int[cap];

            count = 0;
        }

        public void Build(SwarmState state)
        {
            count = 0;

            int activeCount = state.activeCount;

            for (int a = 0; a < activeCount; a++)
            {
                int i = state.activeList[a];

                int type = state.enemyType[i];
                var data = EnemyDatabase.Instance.Get(type);

                if (data == null)
                    continue;

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
                hitFlash[count] = state.hitFlash[i];

                if (state.hitFlash[i] > 0f)
                    tint[count] = new Vector4(1f, 0.3f, 0.3f, 1f);
                else
                    tint[count] = Vector4.one;

                enemyType[count] = type;

                count++;
            }
        }
    }
}