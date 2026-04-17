using UnityEngine;
using System.Collections.Generic;

namespace SwarmCore2D.Drops
{
    public class DropRenderer : MonoBehaviour
    {
        public Mesh mesh;

        Matrix4x4[] matrices    = new Matrix4x4[DropPool.Capacity];
        float[]     frameBuffer = new float[DropPool.Capacity];
        float[]     flipBuffer  = new float[DropPool.Capacity];
        Vector4[]   tintBuffer  = new Vector4[DropPool.Capacity];

        MaterialPropertyBlock props;
        InstancedBatcher batcher;

        Dictionary<Material, List<int>> groups = new Dictionary<Material, List<int>>();

        void Start()
        {
            props   = new MaterialPropertyBlock();
            batcher = new InstancedBatcher(mesh);
        }

        void LateUpdate()
        {
            var pool = DropSystem.Instance?.Pool;
            if (pool == null || pool.activeCount == 0) return;

            GroupByMaterial(pool);
            DrawGroups(pool);
        }

        void GroupByMaterial(DropPool pool)
        {
            groups.Clear();

            for (int a = 0; a < pool.activeCount; a++)
            {
                int id = pool.activeList[a];
                var d = pool.data[id];

                if (d == null || d.material == null) continue;

                if (!groups.ContainsKey(d.material))
                    groups[d.material] = new List<int>();

                groups[d.material].Add(id);
            }
        }

        void DrawGroups(DropPool pool)
        {
            foreach (var pair in groups)
            {
                Material mat = pair.Key;
                var list = pair.Value;
                int count = list.Count;

                for (int i = 0; i < count; i++)
                {
                    int id = list[i];
                    var d  = pool.data[id];

                    float scale = d.visualScale > 0f ? d.visualScale : 0.5f;
                    Vector2 pos = pool.positions[id];

                    matrices[i] = Matrix4x4.TRS(
                        new Vector3(pos.x, pos.y, 0f),
                        Quaternion.identity,
                        new Vector3(scale, scale, 1f)
                    );

                    frameBuffer[i] = pool.frame[id];
                    flipBuffer[i]  = 0f;
                    tintBuffer[i]  = new Vector4(d.tint.r, d.tint.g, d.tint.b, d.tint.a);
                }

                batcher.Draw(mat, matrices, frameBuffer, flipBuffer, tintBuffer, count, props);
            }
        }
    }
}
