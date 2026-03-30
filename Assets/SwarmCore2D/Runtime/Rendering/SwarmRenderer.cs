using UnityEngine;
using System.Collections.Generic;
using SwarmCore2D.Simulation;

namespace SwarmCore2D.Rendering
{
    public class SwarmRenderer : MonoBehaviour
    {
        public Mesh mesh;

        RenderData renderData;
        InstancedBatcher batcher;

        SwarmState state;

        Matrix4x4[] matrices;
        MaterialPropertyBlock props;

        Dictionary<Material, List<int>> groups;

        public void Initialize(SwarmState state)
        {
            this.state = state;

            renderData = new RenderData();
            batcher = new InstancedBatcher(mesh);

            matrices = new Matrix4x4[10000];
            props = new MaterialPropertyBlock();

            groups = new Dictionary<Material, List<int>>();
        }

        void LateUpdate()
        {
            if (state == null)
                return;

            renderData.Build(state);

            GroupByMaterial();

            DrawGroups();
        }

        void GroupByMaterial()
        {
            groups.Clear();

            for (int i = 0; i < renderData.count; i++)
            {
                int type = renderData.enemyType[i];
                var data = EnemyDatabase.Instance.Get(type);

                if (data == null || data.material == null)
                    continue;

                if (!groups.ContainsKey(data.material))
                    groups[data.material] = new List<int>();

                groups[data.material].Add(i);
            }
        }

        void DrawGroups()
        {
            foreach (var pair in groups)
            {
                Material mat = pair.Key;
                var list = pair.Value;

                int count = list.Count;

                for (int i = 0; i < count; i++)
                {
                    int index = list[i];

                    Vector3 pos = renderData.matrices[index].GetColumn(3);

                    matrices[i].SetTRS(
                        pos,
                        Quaternion.identity,
                        Vector3.one
                    );
                }

                float[] frames = new float[count];
                float[] flips = new float[count];
                Vector4[] tint = new Vector4[count];

                for (int i = 0; i < count; i++)
                {
                    int idx = list[i];

                    frames[i] = renderData.frames[idx];
                    flips[i] = renderData.flips[idx];
                    tint[i] = renderData.tint[idx];
                }

                batcher.Draw(
                    mat,
                    matrices,
                    frames,
                    flips,
                    tint,
                    count,
                    props
                );
            }
        }
    }
}