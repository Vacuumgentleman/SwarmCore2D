using UnityEngine;
using SwarmCore2D.Simulation;

namespace SwarmCore2D.Rendering
{
    public class SwarmRenderer : MonoBehaviour
    {
        [Header("Rendering")]
        public Mesh mesh;
        public Material material;

        [Header("Sprite")]
        [Range(0.1f, 5f)]
        public float spriteScale = 0.6f;

        RenderData renderData;
        InstancedBatcher batcher;

        SwarmState state;

        Matrix4x4[] matrices;

        public void Initialize(SwarmState state)
        {
            this.state = state;

            renderData = new RenderData();
            batcher = new InstancedBatcher(mesh, material);

            matrices = new Matrix4x4[10000];
        }

        void LateUpdate()
        {
            if (state == null)
                return;

            renderData.Build(state);

            UpdateMatrices(renderData);

            batcher.Draw(
                matrices,
                renderData.frames,
                renderData.flips,
                renderData.count
            );
        }

        void UpdateMatrices(RenderData data)
        {
            Vector3 scale = Vector3.one * spriteScale;

            for (int i = 0; i < data.count; i++)
            {
                Vector3 pos = data.matrices[i].GetColumn(3);

                matrices[i].SetTRS(
                    pos,
                    Quaternion.identity,
                    scale
                );
            }
        }
    }
}